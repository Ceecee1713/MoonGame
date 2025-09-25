using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;

public class InventoryUISlot : MonoBehaviour, IPointerClickHandler
{
    public InventoryItem InventoryItem;

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

        else
            Debug.Log("This slot is not empty");
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }
}
