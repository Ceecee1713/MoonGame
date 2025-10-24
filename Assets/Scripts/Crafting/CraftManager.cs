using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftManager : MonoBehaviour
{
    [SerializeField]
    private InventoryData inventoryData;

    private List <int> _amountsPerUniqueInventoryItemsToRemove = new List <int>(); //Each index represents the total number of a unique inventory item to be removed
    private List <ItemData> _materialsForCraftableItem = new List <ItemData>();

    private bool _quantityRemaining = false;
    private bool _breakLoop = false;
    private bool _notEnoughItemQuantity = false;

    private int _amountOfMatchingCraftingMaterials = 0; //To be compared to the needed amount of unique materials for craftable item's recipe
    private int _remainingQuantity; 
    private int _amountOfAnInventoryItemNeeded; //Int to be added into "_amountsPerUniqueInventoryItemsToRemove" list 
    //Counts the number of a unique inventory item to be removed (its quantity is fully consumed)

    public void ResetStatus() 
    {
        _amountsPerUniqueInventoryItemsToRemove.Clear();
        _materialsForCraftableItem.Clear();

        _notEnoughItemQuantity = false;
        _quantityRemaining = false;
        _breakLoop = false;

        _amountOfMatchingCraftingMaterials = 0;
        _remainingQuantity = 0;
        _amountOfAnInventoryItemNeeded = 0;
    }

    /*
    public void CheckInventoryForCraftingMaterials(ItemData craftableInventoryItem, ItemData craftingMaterial, int maxAmountOfCraftingMaterialTypes) //Unoptimized
    {
        if(_notEnoughItemQuantity == true)
            return;
        
        _breakLoop = false;
        
        //Checking inventory if it has the same inventory item data as "craftingMaterial's" data (single inventory item data comparison check)
        //Interate through inventory for "maxAmountOfCraftingMaterialTypes" amount of times
        for(int i = 0; i < maxAmountOfCraftingMaterialTypes; i++) 
        {
            if(_breakLoop == true)
                break;

            Debug.Log("New material");

            if(_remainingQuantity > 0)
            {
                _notEnoughItemQuantity = true;
                return;
            }

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
                            Debug.Log("This is the remainder from minusing the total quantity: " + _remainingQuantity);
                        }

                        else //There's a remainder left
                        {
                            _remainingQuantity = _remainingQuantity - inventoryData.Inventory[j].Quantity;
                            Debug.Log("This is the remainder when minusing from the previous remainder: " + _remainingQuantity);
                        }



                        //Checking the remaining quantity left for the inventory item in the inventory
                        if(_remainingQuantity > 0)
                        {
                            _quantityRemaining = true;
                            _amountOfAnInventoryItemNeeded++;
                            Debug.Log("More than zero and is there quantity remaining?" + _quantityRemaining);
                        }

                        else if(_remainingQuantity == 0) 
                        {
                            //Mark inventory item material down to be removed from the inventory 
                            //Removal happens in "InventoryUI"
                            _quantityRemaining = false;
                            _amountOfAnInventoryItemNeeded++; //Adding one because this inventory item's quantity is fully consumed
                            _amountsPerUniqueInventoryItemsToRemove.Add(_amountOfAnInventoryItemNeeded);
                            _materialsForCraftableItem.Add(inventoryData.Inventory[j]); 

                           Debug.Log("Equal to zero and is there quantity remaining?" + _quantityRemaining);

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

                            Debug.Log("Less than zero and is there quantity remaining? " + _quantityRemaining);
                            Debug.Log("This is the remaining quantity: " + _remainingQuantity);

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
            CraftInventoryItem(craftableInventoryItem);
    }
    */

    private void CannotCraftItem()
    {
        //Add warning
    }
    
    public void CheckInventoryForCraftingMaterials(ItemData craftableInventoryItem, ItemData craftingMaterial, int maxAmountOfCraftingMaterialTypes)
    {
        if(_notEnoughItemQuantity == true)
        {
            CannotCraftItem();
            return;
        }
        
        _breakLoop = false;
        
        //Iterate through inventory for "maxAmountOfCraftingMaterialTypes" amount of times
        for(int i = 0; i < maxAmountOfCraftingMaterialTypes; i++) 
        {
            if(_breakLoop == true)
                break;

            //If there's still remaining quantity needed for previous material when wanting to move on to the next material, exit method
            if(_remainingQuantity > 0) 
            {
                _notEnoughItemQuantity = true;
                CannotCraftItem();
                return;
            }

            _amountOfAnInventoryItemNeeded = 0;

            SearchInventoryForItemMaterial(craftingMaterial);
        }

        //If found all required inventory item materials, craft item
        if(_amountOfMatchingCraftingMaterials >= maxAmountOfCraftingMaterialTypes)
            CraftInventoryItem(craftableInventoryItem);
    }

    //Iterate through inventory slots for specific inventory item material ("craftingMaterial")
    private void SearchInventoryForItemMaterial(ItemData craftingMaterial)
    {
        for(int j = 0; j < inventoryData.Inventory.Count; j++) 
        {
            if(_breakLoop == true)
                break;

            //Skip inventory slots that don't match "craftingMaterial"
            if(!IsMatchingInventoryItemMaterial(j, craftingMaterial))
                continue;

            DetermineInventoryItemQuantity(j, craftingMaterial);
        }
    }

    //Check if the inventory slot's item matches the inventory item data of "craftingMaterial" (same type and are stackable)
    private bool IsMatchingInventoryItemMaterial(int slotIndex, ItemData craftingMaterial)
    {
        return inventoryData.Inventory[slotIndex].ItemType == craftingMaterial.ItemType &&
               inventoryData.Inventory[slotIndex].IsThisAStackableItem == true && 
               craftingMaterial.IsThisAStackableItem == true;
    }

    //Determine the inventory items's quantity 
    private void DetermineInventoryItemQuantity(int slotIndex, ItemData craftingMaterial)
    {
        CalculateRemainingQuantity(slotIndex, craftingMaterial);
        
        if(_remainingQuantity > 0)
            HandleMoreInventoryItemQuantityNeeded(); 

        else if(_remainingQuantity == 0)
            HandleInventoryItemQuantityConsumed(slotIndex, craftingMaterial);

        else 
            HandleRemainingInventoryItemQuantity(slotIndex, craftingMaterial);
    }

    private void CalculateRemainingQuantity(int slotIndex, ItemData craftingMaterial)
    {
        if(_quantityRemaining == false) //First-time calculation 
            _remainingQuantity = craftingMaterial.Quantity - inventoryData.Inventory[slotIndex].Quantity;

        else //Subtract from previous remainder
            _remainingQuantity = _remainingQuantity - inventoryData.Inventory[slotIndex].Quantity;
    }

    private void HandleMoreInventoryItemQuantityNeeded()
    {
        _quantityRemaining = true;
        _amountOfAnInventoryItemNeeded++;
    }

    //Exact quantity needed for the inventory item material, mark inventory item for removal
    private void HandleInventoryItemQuantityConsumed(int slotIndex, ItemData craftingMaterial)
    {
        _quantityRemaining = false;
        _amountOfAnInventoryItemNeeded++; //Adding because this inventory item's quantity is fully consumed
        
        MarkItemForRemoval(slotIndex);
        CountMatchingInventoryItemMaterials(craftingMaterial);
        
        _breakLoop = true;
    }

    private void CountMatchingInventoryItemMaterials(ItemData craftingMaterial)
    {
        for(int k = 0; k < inventoryData.Inventory.Count; k++)
        {
            if(inventoryData.Inventory[k].ItemType == craftingMaterial.ItemType)
                _amountOfAnInventoryItemNeeded--;

            if(_amountOfAnInventoryItemNeeded == 0)
            {
                _amountOfMatchingCraftingMaterials++; //Count for one of the materials for the craftable item's recipe found
                break;
            }
        }
    }

    //Remaining quantity left for the inventory item in inventory, adjust its quantity
    private void HandleRemainingInventoryItemQuantity(int slotIndex, ItemData craftingMaterial)
    {
        _quantityRemaining = false;
        
        MarkItemForRemoval(slotIndex);
        AdjustInventoryQuantity(slotIndex);
        
        _amountOfMatchingCraftingMaterials++; //Count for one of the materials for the craftable item's recipe found
        _breakLoop = true;
    }

    //Mark an inventory item to be removed later (in "InventoryUI")
    private void MarkItemForRemoval(int slotIndex)
    {
        _amountsPerUniqueInventoryItemsToRemove.Add(_amountOfAnInventoryItemNeeded);
        _materialsForCraftableItem.Add(inventoryData.Inventory[slotIndex]);
    }

    //Adjust quantity of an inventory item when there's a remainder
    private void AdjustInventoryQuantity(int slotIndex)
    {
        inventoryData.Inventory[slotIndex].Quantity = -_remainingQuantity;
    }

    private void CraftInventoryItem(ItemData craftableInventoryItem)
    {
        Debug.Log("We got enough materials to craft!");
        EventBus.Instance.Publish(new CheckToAddCraftedItem(craftableInventoryItem, _materialsForCraftableItem, _amountsPerUniqueInventoryItemsToRemove));
    }
}
