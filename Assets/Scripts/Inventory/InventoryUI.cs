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
        if(_selectedInventoryUISlot != null) 
            _equipedInventoryItem = _selectedInventoryUISlot.InventoryItem;
    }

    private void CheckInventorySlot(CheckToAddInventoryItem checkToAddInventoryItem) //To add an inventory item (not from crafting)
    {
        //Add same type, stackable inventory item into the an inventory slot that matches its type and is also stackable
        for(int i = 0; i < inventorySlots.Length; i++) 
        {
            //If new inventory item and current inventory slot's item are stackable items
            if(inventorySlots[i].InventoryItem.IsThisAStackableItem == true && checkToAddInventoryItem.InventoryItem.IsThisAStackableItem == true)
            {
                //If new inventory item and current inventory slot's item are the same type
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
    }

    private void CheckToAddCraftedItem(CheckToAddCraftedItem checkToAddCraftedItem) 
    {
        //Add new crafted inventory item in any empty inventory slot
        for(int i = 0; i < inventorySlots.Length; i++) 
        {
            if(inventorySlots[i].IsEmpty == true)
            {
                inventorySlots[i].AddItemToSlot(checkToAddCraftedItem.InventoryItem);
                inventoryData.Inventory.Add(checkToAddCraftedItem.InventoryItem);
                _canCraftItem = true;
                break;
            }
        }

        if(_canCraftItem == false || checkToAddCraftedItem.AmountsPerStackableItemToRemove.Count == 0) 
        {
            _canCraftItem = false;
            //Edit later
            return;
        }

        //The removal of inventory items that were used as crafting materials for the CraftManager:
        for(int i = 0; i < checkToAddCraftedItem.AmountsPerStackableItemToRemove.Count; i++)
        {
            _amountNeededForMaterial = checkToAddCraftedItem.AmountsPerStackableItemToRemove[i];
            _numberToMatchAmountNeededForMaterial = 0;
            var targetInventoryItemType = checkToAddCraftedItem.CraftingMaterialItems[i].ItemType;
            
            //Debug.Log("This is the number we gotta match to: " + _amountNeededForMaterial);
            //Debug.Log("Looking for material type: " + targetInventoryItemType);
            
            for(int j = 0; j < inventorySlots.Length; j++)
            {
                if(_numberToMatchAmountNeededForMaterial >= _amountNeededForMaterial)
                    break;
                
                //If the the inventory slot's inventory item's item type matches "targetInventoryItemType"
                if (inventorySlots[j].InventoryItem.ItemType == targetInventoryItemType)
                {
                    inventoryData.Inventory.Remove(inventorySlots[j].InventoryItem);
                    inventorySlots[j].RemoveItemFromSlot();
                    _numberToMatchAmountNeededForMaterial++;
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
