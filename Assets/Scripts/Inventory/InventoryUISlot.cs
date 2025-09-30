using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;

public class InventoryUISlot : MonoBehaviour, IPointerClickHandler
{
    public InventoryItem InventoryItem;
    public GameObject OutlineImage; //Visuals

    [SerializeField]
    private InventorySlot inventorySlotData;

    public void AddItemToSlot(InventoryItem newInventoryItem)
    {
        if(InventoryItem == null)
        {
            InventoryItem = newInventoryItem;
            
            Debug.Log("I've added a new inventory item");

            //inventorySlotData.SlotImage.sprite = newInventoryItem.SlotImageSprite;
            //inventorySlotData.ItemNameText = newInventoryItem.NameOfItem;
            //inventorySlotData.typeOfItem = newInventoryItem.ItemType;
        }
    }

    public void DropItem()
    {
        if(InventoryItem != null)
        {
            EventBus.Instance.Publish(new SpawnDroppedInventoryItem(InventoryItem));
            InventoryItem = null;

            Debug.Log("I've dropped an inventory item");

            //inventorySlotData.SlotImage.sprite = newInventoryItem.SlotImageSprite;
            //inventorySlotData.ItemNameText = newInventoryItem.NameOfItem;
            //inventorySlotData.typeOfItem = newInventoryItem.ItemType;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        EventBus.Instance.Publish(new SelectInventoryItem(InventoryItem, this));
        Debug.Log("I clicked on the slot");
    }
}
