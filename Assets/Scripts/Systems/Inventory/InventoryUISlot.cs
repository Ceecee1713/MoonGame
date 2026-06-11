using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;

/// <summary>
/// Manages an inventory slot that'll be visible to the player on screen 
/// </summary>
/// 
/// <remarks>
/// This script is made to be on an UI object that the player can click on
/// and on the same game object as "InventoryUI" as public variables and methods are to be referenced by that script
/// 
/// This script works together with the "InventoryUI", "PlayerSpawner", "ChestUI" scripts
/// See <see cref="ChestUI"/> - add inventory item into chest's inventory
/// See <see cref="InventoryUI"/> -  Accessing public methods and variables from this script
/// through publishing "SelectInventoryItem" and "RemoveItemFromSlot" events
/// 
/// See <see cref="PlayerSpawner"/> for how inventory items are instantiated when they're dropped 
/// and have exited player's inventory - publishing the "SpawnDroppedInventoryItem" event
/// 
/// "ChestSlot" acts similarily to this script with how visuals are managed and setting of inventory items
/// Make sure they both function the same
/// See <see cref="ChestSlot"/> for how they function similarily.
/// 
/// See <see cref="InventoryItemTypes"/> for what makes up an inventory item and how inventory UI slots are made up.
/// 
/// This script works with multiple other scripts that publish and subscribe to "ActivatePlayerInputs" event
/// Please see <see cref="AddItemToInventory"/> to get the full details as it would be too much to write in this script alone
/// 
/// </remarks>

public class InventoryUISlot : MonoBehaviour, IPointerClickHandler
{
    public ItemData InventoryItem;

    [Header ("Inventory Slot Data")]
    [SerializeField]
    private InventorySlot inventorySlotVisuals; 
    
    /// <summary> Game object of the outline image, indicating when the inventory slot is selected.</summary>
    public GameObject OutlineImage; 

    /// <summary> Flags whether this slot has no inventory item assigned.</summary>
    public bool IsEmpty;

    private bool _allowInput = true; //Prevent or allow for the player to click on this game object
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
            EventBus.Instance.Publish(new SpawnDroppedInventoryItem(InventoryItem)); //Publish to "PlayerSpawner"
            RemoveItemFromSlot();
        }
    }

    //Receives a "ChestIsOpen" event with parameters:
    //(bool) IsAChestOpen - (true = a chest has been opened by player interaction, 
    //false = a chest has NOT been opened by player interaction).
    private void ChangeInput(ChestIsOpen chestIsOpen) //Published by "ChestInteraction" 
    {
        _isAChestOpen = chestIsOpen.IsAChestOpen;
    }

    //Receives a "ActivatePlayerInputs" event with parameters:
    //(bool) AllowInputs - (true = allow the player to interact with world objects and UI, 
    //false = do NOT allow the player to interact with world objects and UI).
    private void AllowPlayerInput(ActivatePlayerInputs activatePlayerInputs) //Multiple publishers and subscribers
    {
        _allowInput = activatePlayerInputs.AllowInputs;
    }

    public void OnPointerClick(PointerEventData eventData) 
    {
        if(_allowInput == false)
            return;

        EventBus.Instance.Publish(new SelectInventoryItem(InventoryItem, this)); //Publish to "InventoryUI" 

        if(InventoryItem != null && _isAChestOpen == true)
        {
            ItemData clonedInventoryItem = InventoryItem.Clone();
            EventBus.Instance.Publish(new CheckToAddItemToChest(clonedInventoryItem)); //Publish to "ChestUI"
            EventBus.Instance.Publish(new RemoveItemFromSlot(this)); //Publish to "InventoryUI" 
        }
    }
}
