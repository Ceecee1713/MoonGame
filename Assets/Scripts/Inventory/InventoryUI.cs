using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private InventoryUISlot [] inventorySlots = new InventoryUISlot [8];

    void Start()
    {
        EventBus.Instance.Subscribe<CheckToAddInventoryItem>(CheckInventorySlot);
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
}
