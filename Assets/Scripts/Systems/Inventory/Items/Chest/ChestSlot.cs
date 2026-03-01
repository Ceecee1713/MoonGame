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
    private InventorySlot inventorySlotVisuals; 

    void Start()
    {
        IsEmpty = true;
    }

    public void AddItemToSlot(ItemData newInventoryItem) 
    {
        IsEmpty = false;
        InventoryItem = newInventoryItem;

        inventorySlotVisuals.ItemImageObject.SetActive(true);
        inventorySlotVisuals.ItemImage.sprite = InventoryItem.SlotImageSprite;
        inventorySlotVisuals.TypeOfItem = InventoryItem.ItemType;
        inventorySlotVisuals.ItemQuantityText.text = "X " + InventoryItem.Quantity;
    }

    public void RemoveItemFromSlot() 
    {
        IsEmpty = true;
        InventoryItem = null;

        inventorySlotVisuals.ItemImage.sprite = null;
        inventorySlotVisuals.TypeOfItem = InventoryItemTypes.None;
        inventorySlotVisuals.ItemQuantityText.text = " ";
        inventorySlotVisuals.ItemImageObject.SetActive(false);
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

