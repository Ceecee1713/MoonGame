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

    private bool _allowInput = true;
    private bool _isAChestOpen = false;

    void Start()
    {
        IsEmpty = true;

        EventBus.Instance.Subscribe<ChestIsOpen>(ChangeInput);
        EventBus.Instance.Subscribe<ActivatePlayerInputs>(AllowPlayerInput);
    }
    
    public void AddItemToSlot(ItemData newInventoryItem) 
    {
        IsEmpty = false;
        InventoryItem = newInventoryItem.Clone();

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

    public void DropItem()
    {
        if(InventoryItem != null && InventoryItem.ItemObject != null)
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

        EventBus.Instance.Publish(new SelectInventoryItem(InventoryItem, this));

        if(InventoryItem != null && _isAChestOpen == true)
        {
            ItemData clonedInventoryItem = InventoryItem.Clone();
            EventBus.Instance.Publish(new CheckToAddItemToChest(clonedInventoryItem));
            EventBus.Instance.Publish(new RemoveItemFromSlot(this)); //Removing this slot's inventory item (in InventoryUI)
        }
    }
}
