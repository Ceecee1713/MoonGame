using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;

/// <summary>
/// Manages a CHEST inventory slot that'll be visible to the player on screen 
/// </summary>
/// 
/// <remarks>
/// This script is made to be on an UI object that the player can click on
/// 
/// This script works together with the "InventoryUI" script
/// See <see cref="InventoryUI"/> - Publishing "AddItemToInventory" to add an inventory item back into player inventory
/// 
/// "InventoryUISlot" acts similarily to this script with how visuals are managed and setting of inventory items
/// Make sure they both function the same
/// See <see cref="InventoryUISlot"/> for how they function similarily.
/// 
/// See <see cref="InventoryItemTypes"/> for what makes up an inventory item and how inventory UI slots are made up.
/// 
/// </remarks>

public class ChestSlot : MonoBehaviour, IPointerClickHandler
{
    public ItemData InventoryItem;

    /// <summary> Indicating when "InventoryItem" is null (no inventory item in this slot) </summary>
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

        //Setting inventory slot visuals
        inventorySlotVisuals.ItemImageObject.SetActive(true);
        inventorySlotVisuals.ItemImage.sprite = InventoryItem.SlotImageSprite;
        inventorySlotVisuals.TypeOfItem = InventoryItem.ItemType;
        inventorySlotVisuals.ItemQuantityText.text = "X " + InventoryItem.Quantity;
    }

    public void UpdateItemTextQuantity(int newItemQuantity) 
    {
        inventorySlotVisuals.ItemQuantityText.text = "X " + newItemQuantity;
    }

    public void RemoveItemFromSlot() 
    {
        IsEmpty = true;
        InventoryItem = null;

        //Resetting inventory slot visuals
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
        EventBus.Instance.Publish(new AddItemToInventory(clonedInventoryItem)); //Publish to "InventoryUI" 
        RemoveItemFromSlot();
    }
}

