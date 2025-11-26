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
    private InventorySlot inventorySlotData; //Edit

    void Start()
    {
        IsEmpty = true;
    }

    public void AddItemToSlot(ItemData newInventoryItem)
    {
        IsEmpty = false;
        InventoryItem = newInventoryItem;
    }

    public void RemoveItemFromSlot() 
    {
        IsEmpty = true;
        InventoryItem.SlotImageSprite = null;
        InventoryItem.NameOfItem = "Nothing";
        InventoryItem.ItemType = InventoryItemTypes.None;
        InventoryItem.ItemObject = null;
        InventoryItem.Quantity = 0;
        InventoryItem.IsThisAStackableItem = false;
    }

    public void OnPointerClick(PointerEventData eventData) 
    {
        ItemData clonedInventoryItem = InventoryItem.Clone();
        EventBus.Instance.Publish(new AddItemToInventory(clonedInventoryItem));
        RemoveItemFromSlot();
    }
}

