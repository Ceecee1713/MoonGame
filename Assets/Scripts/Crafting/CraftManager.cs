using System.Collections.Generic;
using UnityEngine;

public class CraftManager : MonoBehaviour
{
    [SerializeField]
    private InventoryData inventoryData;

    private List <int> _amountsPerUniqueInventoryItemsToRemove = new List <int>(); 
    private List <ItemData> _materialsForCraftableItem = new List <ItemData>();

    private bool _canCraft = false;
    private bool _quantityRemaining = false;
    private bool _breakLoop = false;

    private int _amountOfMatchingCraftingMaterials = 0;
    private int _remainingQuantity;
    private int _leftoverQuantity; //Holds a reference from "_remainingQuantity"
    private int _amountOfAnInventoryItemNeeded;

    public void ResetStatus() //Called every click on a crafting button (once per click)
    {
        _amountsPerUniqueInventoryItemsToRemove.Clear();
        _materialsForCraftableItem.Clear();

        _canCraft = false;
        _quantityRemaining = false;
        _breakLoop = false;

        _amountOfMatchingCraftingMaterials = 0;
        _remainingQuantity = 0;
        _leftoverQuantity = 0;
        _amountOfAnInventoryItemNeeded = 0;
    }

    //Called "maxAmountOfCraftingMaterialTypes" of times per one crafting button click
    public void CheckInventoryForCraftingMaterials(ItemData craftableInventoryItem, ItemData craftingMaterial, int maxAmountOfCraftingMaterialTypes)
    {
        if(_canCraft == true)
            return;
        
        _breakLoop = false;
        
        //Checking inventory if it has the same inventory item data as "craftingMaterial's" inventory item data (single inventory item comparison check)
        //Interate through inventory for "maxAmountOfCraftingMaterialTypes" amount of times
        for(int i = 0; i < maxAmountOfCraftingMaterialTypes; i++) 
        {
            if(_breakLoop == true)
                break;

            _amountOfAnInventoryItemNeeded = 0;

            for(int j = 0; j < inventoryData.Inventory.Count; j++) 
            {
                //Debug.Log("_amountsPerUniqueInventoryItemsToRemove length: " + _amountsPerUniqueInventoryItemsToRemove.Count);

                if(_breakLoop == true)
                    break;

                if(inventoryData.Inventory[j].ItemType == craftingMaterial.ItemType)
                {
                    if(inventoryData.Inventory[j].IsThisAStackableItem == true && craftingMaterial.IsThisAStackableItem == true)
                    {
                        if(_quantityRemaining == false)
                        {
                            _remainingQuantity = craftingMaterial.Quantity - inventoryData.Inventory[j].Quantity;
                            //Debug.Log("This is the remainder from minusing the total quantity: " + _remainingQuantity);
                        }
                            
                        else
                        {
                            _remainingQuantity = _remainingQuantity - inventoryData.Inventory[j].Quantity;
                            _leftoverQuantity = _remainingQuantity;
                            //Debug.Log("This is the remainder when minusing from the previous remainder: " + _remainingQuantity);
                        }


                        //Checking remainder
                        if(_remainingQuantity > 0)
                        {
                            _quantityRemaining = true;
                            _amountOfAnInventoryItemNeeded++;
                            _leftoverQuantity = _remainingQuantity;
                            //Debug.Log("More than zero and is there quantity remaining?" + _quantityRemaining);
                        }

                        else if(_remainingQuantity == 0) //Removing the inventory items happens in the InventoryUI script
                        {
                            _quantityRemaining = false;
                            _amountOfAnInventoryItemNeeded++;
                            _amountsPerUniqueInventoryItemsToRemove.Add(_amountOfAnInventoryItemNeeded);
                            _materialsForCraftableItem.Add(inventoryData.Inventory[j]); 

                            //Debug.Log("Equal to zero and is there quantity remaining?" + _quantityRemaining);

                            for(int k = 0; k < inventoryData.Inventory.Count; k++)
                            {
                                if(inventoryData.Inventory[k].ItemType == craftingMaterial.ItemType)
                                    _amountOfAnInventoryItemNeeded--;

                                if(_amountOfAnInventoryItemNeeded == 0)
                                {
                                    _amountOfMatchingCraftingMaterials++;
                                    break;
                                }
                            }

                            _breakLoop = true;
                            break;
                        }

                        else if(_remainingQuantity < 0) //Removing the inventory items happens in the InventoryUI script
                        {
                            _quantityRemaining = false;
                            _amountsPerUniqueInventoryItemsToRemove.Add(_amountOfAnInventoryItemNeeded); 
                            _materialsForCraftableItem.Add(inventoryData.Inventory[j]); 

                            //Debug.Log("Less to zero and is there quantity remaining?" + _quantityRemaining);
                            //Debug.Log("This is leftover quantity: " + _leftoverQuantity);

                            for(int k = 0; k < inventoryData.Inventory.Count; k++)
                            {
                                if(inventoryData.Inventory[k].ItemType == craftingMaterial.ItemType)
                                {
                                    if(_amountOfAnInventoryItemNeeded != 0)
                                        _amountOfAnInventoryItemNeeded--;

                                    else //Removing quantity of the inventory slot's item if there's leftover
                                    {
                                        inventoryData.Inventory[k].Quantity = inventoryData.Inventory[k].Quantity + _leftoverQuantity;
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
            _canCraft = true;
            CraftInventoryItem(craftableInventoryItem);
        }
    }

    private void CraftInventoryItem(ItemData craftableInventoryItem)
    {
        //Debug.Log("We got enough materials to craft!");
        EventBus.Instance.Publish(new CheckToAddCraftedItem(craftableInventoryItem, _materialsForCraftableItem, _amountsPerUniqueInventoryItemsToRemove));
    }
}
