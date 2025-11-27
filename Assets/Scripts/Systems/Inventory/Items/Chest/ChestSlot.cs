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

    private bool _allowInput = false;

    void Start()
    {
        IsEmpty = true;

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
        InventoryItem.NameOfItem = "Nothing";
        InventoryItem.ItemType = InventoryItemTypes.None;
        InventoryItem.ItemObject = null;
        InventoryItem.Quantity = 0;
        InventoryItem.IsThisAStackableItem = false;
    }

    private void AllowPlayerInput(ActivatePlayerInputs activatePlayerInputs)
    {
        _allowInput = activatePlayerInputs.AllowInputs;
    }

    public void OnPointerClick(PointerEventData eventData) 
    {
        if(_allowInput == false)
            return;

        ItemData clonedInventoryItem = InventoryItem.Clone();
        EventBus.Instance.Publish(new AddItemToInventory(clonedInventoryItem));
        RemoveItemFromSlot();
    }
}

