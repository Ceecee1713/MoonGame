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

    private int _maxStackAmount = 3;

    //Used for removing inventory items upon adding a craftable inventory item to the inventory
    private int _amountOfSingleFullyConsumedMaterialToRemove;
    private int _numberToMatchAmountOfFullyConsumedMaterial;

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
        if(_selectedInventoryUISlot != null) 
            _equipedInventoryItem = _selectedInventoryUISlot.InventoryItem;
    }

    private void CheckInventorySlot(CheckToAddInventoryItem checkToAddInventoryItem) //To add an inventory item (not from crafting)
    {
        AddInventoryItem(checkToAddInventoryItem.InventoryItem);

        /*
        for(int i = 0; i < inventorySlots.Length; i++) 
        {
            if(inventorySlots[i].InventoryItem.IsThisAStackableItem == true && checkToAddInventoryItem.InventoryItem.IsThisAStackableItem == true)
            {
                if(inventorySlots[i].InventoryItem.ItemType == checkToAddInventoryItem.InventoryItem.ItemType)
                {
                    //If the inventory slot's item's quantity isn't above "_maxStackAmount" (allow items to stack)
                    if(inventorySlots[i].InventoryItem.Quantity < _maxStackAmount) 
                    {
                        inventorySlots[i].InventoryItem.Quantity++;
                        return;
                    }
                } 
            }
        }

        //Add new inventory item in any empty inventory slot, whether item is stackable or not
        for(int i = 0; i < inventorySlots.Length; i++) 
        {
            if(inventorySlots[i].IsEmpty == true)
            {
                inventorySlots[i].AddItemToSlot(checkToAddInventoryItem.InventoryItem);
                inventoryData.Inventory.Add(checkToAddInventoryItem.InventoryItem);
                break;
            }
        }
        */
    }

    private void AddInventoryItem(ItemData itemToCheck)
    {
        for(int i = 0; i < inventorySlots.Length; i++) 
        {
            if(inventorySlots[i].InventoryItem.IsThisAStackableItem == true && itemToCheck.IsThisAStackableItem == true)
            {
                if(inventorySlots[i].InventoryItem.ItemType == itemToCheck.ItemType)
                {
                    //If the inventory slot's item's quantity isn't above "_maxStackAmount" (allow items to stack)
                    if(inventorySlots[i].InventoryItem.Quantity < _maxStackAmount) 
                    {
                        inventorySlots[i].InventoryItem.Quantity++;
                        return;
                    }
                } 
            }
        }

        //Add new inventory item in any empty inventory slot, whether item is stackable or not
        for(int i = 0; i < inventorySlots.Length; i++) 
        {
            if(inventorySlots[i].IsEmpty == true)
            {
                inventorySlots[i].AddItemToSlot(itemToCheck);
                inventoryData.Inventory.Add(itemToCheck);
                break;
            }
        }
    }

    private void CheckToAddCraftedItem(CheckToAddCraftedItem checkToAddCraftedItem) 
    {
        AddInventoryItem(checkToAddCraftedItem.InventoryItem);

        /*
        for(int i = 0; i < inventorySlots.Length; i++) 
        {
            if(inventorySlots[i].InventoryItem.IsThisAStackableItem == true && checkToAddCraftedItem.InventoryItem.IsThisAStackableItem == true)
            {
                if(inventorySlots[i].InventoryItem.ItemType == checkToAddCraftedItem.InventoryItem.ItemType)
                {
                    //If the inventory slot's item's quantity isn't above "_maxStackAmount" (allow items to stack)
                    if(inventorySlots[i].InventoryItem.Quantity < _maxStackAmount) 
                    {
                        inventorySlots[i].InventoryItem.Quantity++;
                        return;
                    }
                } 
            }
        }

        //Add new crafted inventory item in any empty inventory slot
        for(int i = 0; i < inventorySlots.Length; i++) 
        {
            if(inventorySlots[i].IsEmpty == true)
            {
                inventorySlots[i].AddItemToSlot(checkToAddCraftedItem.InventoryItem);
                inventoryData.Inventory.Add(checkToAddCraftedItem.InventoryItem);
                break;
            }
        }
        */

        if(checkToAddCraftedItem.AmountsPerStackableItemToRemove.Count == 0) 
            return;

        //Removal of inventory items that were used as crafting materials for the CraftManager:
        for(int i = 0; i < checkToAddCraftedItem.AmountsPerStackableItemToRemove.Count; i++)
        {
            _amountOfSingleFullyConsumedMaterialToRemove = checkToAddCraftedItem.AmountsPerStackableItemToRemove[i];
            _numberToMatchAmountOfFullyConsumedMaterial = 0;

            var targetInventoryItemType = checkToAddCraftedItem.CraftingMaterialItems[i].ItemType;
            
            //Debug.Log("This is the number we gotta match to: " + _amountOfSingleFullyConsumedMaterialToRemove);
            //Debug.Log("Looking for material type: " + targetInventoryItemType);
            
            for(int j = 0; j < inventorySlots.Length; j++)
            {
                if(_numberToMatchAmountOfFullyConsumedMaterial >= _amountOfSingleFullyConsumedMaterialToRemove)
                    break;
                
                if (inventorySlots[j].InventoryItem.ItemType == targetInventoryItemType)
                {
                    inventoryData.Inventory.Remove(inventorySlots[j].InventoryItem);
                    inventorySlots[j].RemoveItemFromSlot();
                    _numberToMatchAmountOfFullyConsumedMaterial++;
                }
            }
        }
    }

    private void EquipInventoryItem(SelectInventoryItem selectInventoryItem) //When selecting on an inventory slot with mouse
    {
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            if(inventorySlots[i] == selectInventoryItem.InventoryUISlot)
            {
                if(_selectedInventoryUISlot != null) 
                {
                    //Disable visuals of the previously selected inventory slot
                    _previousInventoryUISlot = _selectedInventoryUISlot;
                    _previousInventoryUISlot.OutlineImage.SetActive(false);
                }

                _selectedInventoryUISlot = selectInventoryItem.InventoryUISlot; 
                
                //Enable visuals of selected inventory slot
                _selectedInventoryUISlot.OutlineImage.SetActive(true); 
                _equipedInventoryItem = selectInventoryItem.InventoryUISlot.InventoryItem;
                break;
            }
        }
    }

    private void DropEquipedInventoryItem(DropEquipedInventoryItem dropEquipedInventoryItem)
    {
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            //If selected UI slot is within the "inventorySlots" array and selected inventory slot has an inventory item
            if(inventorySlots[i] == _selectedInventoryUISlot)
            {
                //Remove inventory item from inventory, its inventory slot and instiantiate item in world space
                inventoryData.Inventory.Remove(inventorySlots[i].InventoryItem);
                inventorySlots[i].DropItem();
                break;
            }
        }
    }
}
