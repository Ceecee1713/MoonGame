using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;

public class InventoryUISlot : MonoBehaviour, IPointerClickHandler
{
    public ItemData InventoryItem;
    public bool IsEmpty;
    
    public GameObject OutlineImage; //Visuals

    [SerializeField]
    private InventorySlot inventorySlotData;

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

    public void DropItem()
    {
        if(InventoryItem.ItemObject != null)
        {
            EventBus.Instance.Publish(new SpawnDroppedInventoryItem(InventoryItem));
            RemoveItemFromSlot();
        }
    }

    public void OnPointerClick(PointerEventData eventData) 
    {
        EventBus.Instance.Publish(new SelectInventoryItem(InventoryItem, this));
    }
}
