using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private InventoryData inventoryData;

    [SerializeField]
    private InventoryUISlot [] inventorySlots = new InventoryUISlot [8];

    [SerializeField] //Delete later
    private ItemData _equipedInventoryItem;

    [SerializeField] //Delete later
    private InventoryUISlot _selectedInventoryUISlot;

    [SerializeField] //Delete later
    private InventoryUISlot _previousInventoryUISlot;

    private int _maxAmountOfStackableItems = 3;
    private bool _canCraftItem = false;

    private int _amountNeededForMaterial;
    private int _numberToMatchAmountNeededForMaterial;

    void Start()
    {
        inventoryData.Inventory.Clear();

        EventBus.Instance.Subscribe<CheckToAddInventoryItem>(CheckInventorySlot);
        EventBus.Instance.Subscribe<CheckToAddCraftedItem>(CheckToAddCraftedItem);
        EventBus.Instance.Subscribe<SelectInventoryItem>(EquipInventoryItem);
        EventBus.Instance.Subscribe<DropEquipedInventoryItem>(DropEquipedInventoryItem);
    }

    void Update()
    {
        if(_selectedInventoryUISlot != null) //Update inventory item from "_selectedInventoryUISlot"
            _equipedInventoryItem = _selectedInventoryUISlot.InventoryItem;
    }

    private void CheckInventorySlot(CheckToAddInventoryItem checkToAddInventoryItem) //To add an inventory item (not from crafting)
    {
        for(int i = 0; i < inventorySlots.Length; i++) 
        {
            //If new inventory item and current inventory slot that's being checked are stackable items
            if(inventorySlots[i].InventoryItem.IsThisAStackableItem == true && checkToAddInventoryItem.InventoryItem.IsThisAStackableItem == true)
            {
                //If new inventory item and current inventory slot that's being checked are the same item type 
                if(inventorySlots[i].InventoryItem.ItemType == checkToAddInventoryItem.InventoryItem.ItemType)
                {
                    //If the inventory slot's item's quantity isn't above the "_maxAmountOfStackableItems" (for stackable items to stack)
                    if(inventorySlots[i].InventoryItem.Quantity < _maxAmountOfStackableItems) 
                    {
                        inventorySlots[i].InventoryItem.Quantity++;
                        return;
                    }
                } 
            }
        }

        for(int i = 0; i < inventorySlots.Length; i++) //Add new inventory item in any empty inventory slot, whether item is stackable or not
        {
            if(inventorySlots[i].IsEmpty == true)
            {
                inventorySlots[i].AddItemToSlot(checkToAddInventoryItem.InventoryItem);
                inventoryData.Inventory.Add(checkToAddInventoryItem.InventoryItem);
                break;
            }
        }
    }

    private void CheckToAddCraftedItem(CheckToAddCraftedItem checkToAddCraftedItem) //To add crafted inventory item. Note that craftable items DO NOT stack
    {
        for(int i = 0; i < inventorySlots.Length; i++) //Add new crafted inventory item in any empty inventory slot
        {
            if(inventorySlots[i].IsEmpty == true)
            {
                inventorySlots[i].AddItemToSlot(checkToAddCraftedItem.InventoryItem);
                inventoryData.Inventory.Add(checkToAddCraftedItem.InventoryItem);
                _canCraftItem = true;
                break;
            }
        }

        if(_canCraftItem == false)
        {
            Debug.Log("Cannot add crafted item into inventory.");
            //Add event to show warning message and freeze mouse input for a set duration
            return;
        }


        //The removal of inventory items that were used as crafting materials for the CraftManager:

        /*
        //Checking for non-stacking items
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            for(int j = 0; j < checkToAddCraftedItem.CraftingMaterialItems.Count; j++)
            {
                if(checkToAddCraftedItem.CraftingMaterialItems.Contains(inventorySlots[i].InventoryItem))
                {
                    inventoryData.Inventory.Remove(inventorySlots[i].InventoryItem);
                    inventorySlots[i].RemoveItemFromSlot(); //Clear item data from the inventory slot
                }
            }
        }
        */

        //Checking for stackable items
        if(checkToAddCraftedItem.AmountsPerStackableItemToRemove.Count == 0) //No stackable materials needed to consume
        {
            _canCraftItem = false;
            return;
        }

        for(int i = 0; i < checkToAddCraftedItem.AmountsPerStackableItemToRemove.Count; i++)
        {
            _amountNeededForMaterial = checkToAddCraftedItem.AmountsPerStackableItemToRemove[i];
            _numberToMatchAmountNeededForMaterial = 0;
            Debug.Log("This is the number we gotta match to: " + _amountNeededForMaterial);

            for(int j = 0; j < inventorySlots.Length; j++)
            {
                if(_numberToMatchAmountNeededForMaterial >= _amountNeededForMaterial)
                    break;

                //Comparing inventory item types from the inventory slot array to ANY INDEX in the "checkToAddCraftedItem.CraftingMaterialItems" List
                if (checkToAddCraftedItem.CraftingMaterialItems.Any(InventoryItem => InventoryItem.ItemType == inventorySlots[j].InventoryItem.ItemType))
                {
                    inventoryData.Inventory.Remove(inventorySlots[j].InventoryItem); //Remove inventory item from the inventory data
                    inventorySlots[j].RemoveItemFromSlot(); //Clear item data from "j" inventory slot
                    _numberToMatchAmountNeededForMaterial++;
                    Debug.Log(_numberToMatchAmountNeededForMaterial);
                }
            }
        }

        _canCraftItem = false; //Reset value
    }

    private void EquipInventoryItem(SelectInventoryItem selectInventoryItem) //When selecting on an inventory slot with mouse
    {
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            if(inventorySlots[i] == selectInventoryItem.InventoryUISlot)
            {
                if(_selectedInventoryUISlot != null) //Disable visuals of the previously selected inventory slot
                {
                    _previousInventoryUISlot = _selectedInventoryUISlot;
                    _previousInventoryUISlot.OutlineImage.SetActive(false);
                }

                _selectedInventoryUISlot = selectInventoryItem.InventoryUISlot; 
                _selectedInventoryUISlot.OutlineImage.SetActive(true); //Enable visuals of selected inventory slot
                _equipedInventoryItem = selectInventoryItem.InventoryUISlot.InventoryItem;
                break;
            }
        }
    }

    private void DropEquipedInventoryItem(DropEquipedInventoryItem dropEquipedInventoryItem)
    {
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            //If the selected UI slot is within the "inventorySlots" array, the selected inventory slot has an inventory item inside it 
            //AND the equiped inventory item isn't empty
            if(inventorySlots[i] == _selectedInventoryUISlot) //&& _equipedInventoryItem != null)
            {
                //Remove the item from inventory data, its inventory slot and instiantiate the inventory item in world space
                inventoryData.Inventory.Remove(inventorySlots[i].InventoryItem);
                inventorySlots[i].DropItem();
                //_equipedInventoryItem = null;
                break;
            }
        }
    }
}
