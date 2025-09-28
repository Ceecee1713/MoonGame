using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private InventoryUISlot [] inventorySlots = new InventoryUISlot [8];

    [SerializeField]
    private InventoryItem _equipedInventoryItem;

    [SerializeField]
    private InventoryUISlot _selectedInventoryUISlot;

    void Start()
    {
        EventBus.Instance.Subscribe<CheckToAddInventoryItem>(CheckInventorySlot);
        EventBus.Instance.Subscribe<SelectInventoryItem>(EquipInventoryItem);
        EventBus.Instance.Subscribe<DropEquipedInventoryItem>(DropEquipedInventoryItem);
    }

    private void CheckInventorySlot(CheckToAddInventoryItem checkToAddInventoryItem)
    {
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            if(inventorySlots[i].InventoryItem == null)
            {
                inventorySlots[i].AddItemToSlot(checkToAddInventoryItem.InventoryItem);
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
                _selectedInventoryUISlot = selectInventoryItem.InventoryUISlot;
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
                inventorySlots[i].DropItem();
                _equipedInventoryItem = null;
                break;
            }
        }
    }
}
