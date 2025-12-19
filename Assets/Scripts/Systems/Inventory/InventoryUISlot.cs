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
    private InventorySlot inventorySlotData; //Edit

    private bool _isAChestOpen = false;
    private bool _allowInput = false;

    void Start()
    {
        IsEmpty = true;

        EventBus.Instance.Subscribe<ChestIsOpen>(ChangeInput);
        EventBus.Instance.Subscribe<ActivatePlayerInputs>(AllowPlayerInput);
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
        //InventoryItem.NameOfItem = "Nothing";
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
            EventBus.Instance.Publish(new RemoveItemFromSlot(this));
        }
    }
}
