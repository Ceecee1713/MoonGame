using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;

public class InventoryUISlot : MonoBehaviour, IPointerClickHandler
{
    public ItemData InventoryItem;

    [Header ("Inventory Slot Data")]
    [SerializeField]
    private InventorySlot inventorySlotVisuals; 
    
    public GameObject OutlineImage; 
    public bool IsEmpty;

    private bool _isAChestOpen = false;
    private bool _allowInput = true;

    void Start()
    {
        IsEmpty = true;

        EventBus.Instance.Subscribe<ChestIsOpen>(ChangeInput);
        EventBus.Instance.Subscribe<ActivatePlayerInputs>(AllowPlayerInput);
    }

    public void AddItemToSlot(ItemData newInventoryItem) //Add inventory slot UI visual data (EDITTTT)
    {
        IsEmpty = false;
        InventoryItem = newInventoryItem.Clone();

        inventorySlotVisuals.SlotImageObject.SetActive(true);
        inventorySlotVisuals.SlotImage.sprite = InventoryItem.SlotImageSprite;
        inventorySlotVisuals.TypeOfItem = InventoryItem.ItemType;
        inventorySlotVisuals.ItemQuantityText.text = "X " + InventoryItem.Quantity;
    }

    public void UpdateItemTextQuantity(int newItemQuantity) 
    {
        inventorySlotVisuals.ItemQuantityText.text = "X " + newItemQuantity;
    }

    public void RemoveItemFromSlot() //Add inventory slot UI visual data (EDITTTT)
    {
        IsEmpty = true;
        InventoryItem = null;

        inventorySlotVisuals.SlotImage.sprite = null;
        inventorySlotVisuals.TypeOfItem = InventoryItemTypes.None;
        inventorySlotVisuals.ItemQuantityText.text = " ";
        inventorySlotVisuals.SlotImageObject.SetActive(false);
    }

    public void DropItem()
    {
        if(InventoryItem.ItemObject != null)
        {
            EventBus.Instance.Publish(new SpawnDroppedInventoryItem(InventoryItem));
            RemoveItemFromSlot();
        }
    }

    private void ChangeInput(ChestIsOpen chestIsOpen)
    {
        _isAChestOpen = chestIsOpen.IsAChestOpen;
    }

    private void AllowPlayerInput(ActivatePlayerInputs activatePlayerInputs)
    {
        _allowInput = activatePlayerInputs.AllowInputs;
    }

    public void OnPointerClick(PointerEventData eventData) 
    {
        if(_allowInput == false)
            return;

        if(_isAChestOpen == false)
            EventBus.Instance.Publish(new SelectInventoryItem(InventoryItem, this));

        else
        {
            ItemData clonedInventoryItem = InventoryItem.Clone();
            EventBus.Instance.Publish(new CheckToAddItemToChest(clonedInventoryItem));
            EventBus.Instance.Publish(new RemoveItemFromSlot(this)); //Removing this slot's inventory item from inventory (in InventoryUI)
        }
    }
}
