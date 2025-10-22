using System.Collections.Generic;
using UnityEngine;

public class CraftManager : MonoBehaviour
{
    [SerializeField]
    private InventoryData inventoryData;

    private List <int> checkedInventoryDataIndexes = new List <int>(); //For single items to prevent unnessacary inventory slot array checks
    private List <int> amountOfItemsToRemove = new List <int>(); //For stackable items
    private List <ItemData> allCraftingMaterials = new List <ItemData>();

    private bool _canCraft = false;
    private bool _quantityRemaining = false;
    private bool _breakLoop = false;

    private int _amountOfMatchingCraftingMaterials = 0;
    private int _remainingQuantity;
    private int _leftoverQuantity;
    private int _amountOfItemsNeeded;

    public void ResetStatus() //Called every click on a crafting button (once per click)
    {
        checkedInventoryDataIndexes.Clear();
        amountOfItemsToRemove.Clear();
        allCraftingMaterials.Clear();

        _amountOfMatchingCraftingMaterials = 0;
        _remainingQuantity = 0;
        _leftoverQuantity = 0;
        _amountOfItemsNeeded = 0;

        _canCraft = false;
        _quantityRemaining = false;
        _breakLoop = false;
    }

    public void CheckInventoryForCraftingMaterials(ItemData craftableInventoryItem, ItemData craftingMaterial, int maxAmountOfCraftingMaterialTypes)
    {
        if(_canCraft == true)
            return;
        
        /*
        //For single, non-stackable items
        for(int i = 0; i < inventoryData.Inventory.Count; i++) 
        {
            for(int j = 0; j < maxAmountOfCraftingMaterialTypes; j++)
            {
                if(inventoryData.Inventory[i].ItemType == craftingMaterial.ItemType && !checkedInventoryDataIndexes.Contains(i)) //For single items, not stackable
                {
                    _amountOfMatchingCraftingMaterials++;
                    checkedInventoryDataIndexes.Add(i); //Prevent checked inventory data indexes from being looked at again 
                    allCraftingMaterials.Add(craftingMaterial); 
                }  
            }
        }
        */
        
        //Checking inventory data if it has the same inventory item data as "craftingMaterial's" inventory item data
        //And interate completely through inventory data's list for "maxAmountOfCraftingMaterialTypes" amount of times
        //For stackable items
        for(int i = 0; i < inventoryData.Inventory.Count; i++)
        {
            if(_breakLoop == true)
                break;

            for(int j = 0; j < maxAmountOfCraftingMaterialTypes; j++)
            {
                if(_breakLoop == true)
                    break;

                if(inventoryData.Inventory[i].ItemType == craftingMaterial.ItemType)
                {
                    if(inventoryData.Inventory[i].IsThisAStackableItem == true && craftingMaterial.IsThisAStackableItem == true)
                    {
                        if(_quantityRemaining == false)
                        {
                            _remainingQuantity = craftingMaterial.Quantity - inventoryData.Inventory[i].Quantity;
                            Debug.Log("This is the remainder from minusing the total quantity: " + _remainingQuantity);
                        }
                            
                        else
                        {
                            _remainingQuantity = _remainingQuantity - inventoryData.Inventory[i].Quantity;
                            Debug.Log("This is the remainder when minusing from the previous remainder: " + _remainingQuantity);
                        }


                        //Checking remainder
                        if(_remainingQuantity > 0)
                        {
                            _quantityRemaining = true;
                            _amountOfItemsNeeded++;
                            _leftoverQuantity = _remainingQuantity;
                            Debug.Log("More than zero and is there quantity remaining?" + _quantityRemaining);

                            continue;
                        }

                        else if(_remainingQuantity == 0) //Removing the inventory items happens in the InventoryUI script
                        {
                            _quantityRemaining = false;
                            _amountOfItemsNeeded++;
                            amountOfItemsToRemove.Add(_amountOfItemsNeeded);
                            allCraftingMaterials.Add(inventoryData.Inventory[i]); 

                            Debug.Log("Equal to zero and is there quantity remaining?" + _quantityRemaining);

                            for(int k = 0; k < inventoryData.Inventory.Count; k++)
                            {
                                if(inventoryData.Inventory[k].ItemType == craftingMaterial.ItemType)
                                    _amountOfItemsNeeded--;

                                if(_amountOfItemsNeeded == 0)
                                {
                                    _amountOfMatchingCraftingMaterials++;
                                    break;
                                }
                            }

                            break;
                        }

                        else if(_remainingQuantity < 0) //Removing the inventory items happens in the InventoryUI script
                        {
                            _quantityRemaining = false;
                            amountOfItemsToRemove.Add(_amountOfItemsNeeded); 
                            allCraftingMaterials.Add(inventoryData.Inventory[i]); 

                            Debug.Log("Less to zero and is there quantity remaining?" + _quantityRemaining);

                            for(int k = 0; k < inventoryData.Inventory.Count; k++)
                            {
                                if(inventoryData.Inventory[k].ItemType == craftingMaterial.ItemType)
                                {
                                    if(_amountOfItemsNeeded != 0)
                                        _amountOfItemsNeeded--;

                                    else //Removing quantity of the inventory slot's item if there's leftover
                                    {
                                        inventoryData.Inventory[k].Quantity = inventoryData.Inventory[k].Quantity - _leftoverQuantity;
                                        _amountOfMatchingCraftingMaterials++;
                                        break;
                                    }
                                }
                            }

                            _breakLoop = true;
                            break;
                        }
                    }
                } 
            }
        }

        if(_amountOfMatchingCraftingMaterials >= maxAmountOfCraftingMaterialTypes)
        {
            //Debug.Log(_amountOfMatchingCraftingMaterials);
            _canCraft = true;
            CraftInventoryItem(craftableInventoryItem);
        }
    }

    private void CraftInventoryItem(ItemData craftableInventoryItem)
    {
        Debug.Log("We got enough materials to craft!");
        EventBus.Instance.Publish(new CheckToAddCraftedItem(craftableInventoryItem, allCraftingMaterials, amountOfItemsToRemove));
    }
}
