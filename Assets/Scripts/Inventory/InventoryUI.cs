using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private InventoryData inventoryData;

    [SerializeField]
    private InventoryUISlot [] inventorySlots = new InventoryUISlot [8];

    [SerializeField] //Delete later
    private InventoryItem _equipedInventoryItem;

    [SerializeField] //Delete later
    private InventoryUISlot _selectedInventoryUISlot;

    [SerializeField] //Delete later
    private InventoryUISlot _previousInventoryUISlot;

    private int _maxAmountOfStackableItems = 64;

    void Start()
    {
        EventBus.Instance.Subscribe<CheckToAddInventoryItem>(CheckInventorySlot);
        EventBus.Instance.Subscribe<SelectInventoryItem>(EquipInventoryItem);
        EventBus.Instance.Subscribe<DropEquipedInventoryItem>(DropEquipedInventoryItem);
    }

    void Update()
    {
        if(_selectedInventoryUISlot != null) //Update inventory item from "_selectedInventoryUISlot"
            _equipedInventoryItem = _selectedInventoryUISlot.InventoryItem;
    }

    private void CheckInventorySlot(CheckToAddInventoryItem checkToAddInventoryItem)
    {
        for(int i = 0; i < inventorySlots.Length; i++) //Check if new inventory item's stackable and if any inventory slot has the same inventory item
        {
            //If both are labelled as stackable items
            if(inventorySlots[i].InventoryItem != null && inventorySlots[i].InventoryItem.ItemData.IsThisAStackableItem == true && checkToAddInventoryItem.InventoryItem.ItemData.IsThisAStackableItem == true)
            {
                //If both are the same item type and the inventory slot's quantity of that item isn't above the "_maxAmountOfStackableItems"
                if(inventorySlots[i].InventoryItem.ItemData.ItemType == checkToAddInventoryItem.InventoryItem.ItemData.ItemType && 
                    inventorySlots[i].InventoryItem.ItemData.Quantity <= _maxAmountOfStackableItems)
                {
                    inventorySlots[i].InventoryItem.ItemData.Quantity++;
                    return;
                }
            }
        }

        for(int i = 0; i < inventorySlots.Length; i++) //Add new inventory item in any empty inventory slot
        {
            if(inventorySlots[i].InventoryItem == null)
            {
                inventorySlots[i].AddItemToSlot(checkToAddInventoryItem.InventoryItem);
                inventoryData.Inventory.Add(checkToAddInventoryItem.InventoryItem);
                break;
            }
        }
    }

    private void EquipInventoryItem(SelectInventoryItem selectInventoryItem)
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
                _selectedInventoryUISlot.OutlineImage.SetActive(true); //Enable visuals
                _equipedInventoryItem = selectInventoryItem.InventoryUISlot.InventoryItem;
                break;
            }
        }
    }

    private void DropEquipedInventoryItem(DropEquipedInventoryItem dropEquipedInventoryItem)
    {
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            if(inventorySlots[i] == _selectedInventoryUISlot && _selectedInventoryUISlot.InventoryItem != null && _equipedInventoryItem != null)
            {
                inventoryData.Inventory.Remove(_equipedInventoryItem);
                inventorySlots[i].DropItem();
                _equipedInventoryItem = null;
                break;
            }
        }
    }
}
