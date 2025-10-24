using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftManager : MonoBehaviour
{
    [SerializeField]
    private InventoryData inventoryData;

    private List <int> _amountsPerUniqueInventoryItemsToRemove = new List <int>(); //Each index represents the total number of a unique inventory item to be removed
    private List <ItemData> _materialsForCraftableItem = new List <ItemData>();

    private bool _canCraft = false;
    private bool _quantityRemaining = false;
    private bool _breakLoop = false;

    private int _amountOfMatchingCraftingMaterials = 0; //To be compared to the needed amount of unique materials for craftable item's recipe
    private int _remainingQuantity; 
    private int _amountOfAnInventoryItemNeeded; //Int to be added into "_amountsPerUniqueInventoryItemsToRemove" list 
    //Counts the number of a unique inventory item to be removed (its quantity is fully consumed)

    public void ResetStatus() 
    {
        _amountsPerUniqueInventoryItemsToRemove.Clear();
        _materialsForCraftableItem.Clear();

        _canCraft = false;
        _quantityRemaining = false;
        _breakLoop = false;

        _amountOfMatchingCraftingMaterials = 0;
        _remainingQuantity = 0;
        _amountOfAnInventoryItemNeeded = 0;
    }

    public void CheckInventoryForCraftingMaterials(ItemData craftableInventoryItem, ItemData craftingMaterial, int maxAmountOfCraftingMaterialTypes)
    {
        if(_canCraft == true)
            return;
        
        _breakLoop = false;
        
        //Checking inventory if it has the same inventory item data as "craftingMaterial's" data (single inventory item data comparison check)
        //Interate through inventory for "maxAmountOfCraftingMaterialTypes" amount of times
        for(int i = 0; i < maxAmountOfCraftingMaterialTypes; i++) 
        {
            if(_breakLoop == true)
                break;

            //Debug.Log("New material");
            _amountOfAnInventoryItemNeeded = 0;

            for(int j = 0; j < inventoryData.Inventory.Count; j++) 
            {
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

                        else //There's a remainder left
                        {
                            _remainingQuantity = _remainingQuantity - inventoryData.Inventory[j].Quantity;
                            //Debug.Log("This is the remainder when minusing from the previous remainder: " + _remainingQuantity);
                        }

                        //Checking the remaining quantity left for the inventory item in the inventory
                        if(_remainingQuantity > 0)
                        {
                            _quantityRemaining = true;
                            _amountOfAnInventoryItemNeeded++;
                            //Debug.Log("More than zero and is there quantity remaining?" + _quantityRemaining);
                        }

                        else if(_remainingQuantity == 0) 
                        {
                            //Mark inventory item material down to be removed from the inventory 
                            //Removal happens in "InventoryUI"
                            _quantityRemaining = false;
                            _amountOfAnInventoryItemNeeded++; //Adding one because this inventory item's quantity is fully consumed
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

                        else if(_remainingQuantity < 0) //If there'll be a remainder left for the inventory item's quantity (its quantity not fully consumed)
                        {
                            //Mark inventory item material down to be removed from the inventory 
                            //Removal happens in "InventoryUI"
                            _quantityRemaining = false;
                            _amountsPerUniqueInventoryItemsToRemove.Add(_amountOfAnInventoryItemNeeded); 
                            _materialsForCraftableItem.Add(inventoryData.Inventory[j]); 

                            //Debug.Log("Less than zero and is there quantity remaining? " + _quantityRemaining);
                            //Debug.Log("This is the remaining quantity: " + _remainingQuantity);

                            inventoryData.Inventory[j].Quantity = -_remainingQuantity; //Adjusting quantity of inventory item
                            _amountOfMatchingCraftingMaterials++;
                            _breakLoop = true;
                            break;
                        }
                    }
                } 
            }
        }

        //If the amount of materials that were found in the inventory equal OR is higher than
        //The needed amount for the craftable item's recipe ("CraftButton")
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
