using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;

public class ChestSlot : MonoBehaviour, IPointerClickHandler
{
    public ItemData InventoryItem;
    public bool IsEmpty;

    [SerializeField]
    private InventorySlot inventorySlotData; //Visual data for inventory slot

    void Start()
    {
        IsEmpty = true;
    }

    public void AddItemToSlot(ItemData newInventoryItem) //Add inventory slot UI visual data
    {
        IsEmpty = false;
        InventoryItem = newInventoryItem;
    }

    public void RemoveItemFromSlot() //Add inventory slot UI visual data
    {
        IsEmpty = true;
        InventoryItem = null;
    }

    public void OnPointerClick(PointerEventData eventData) 
    {
        if(InventoryItem == null || IsEmpty)
            return;

        ItemData clonedInventoryItem = InventoryItem.Clone();
        clonedInventoryItem.IsDroppedItem = false;
        EventBus.Instance.Publish(new AddItemToInventory(clonedInventoryItem));
        RemoveItemFromSlot();
    }
}

