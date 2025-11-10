using System;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

//Types of the all possible inventory items
public enum InventoryItemTypes 
{
    None,
    Stone,
    Beryllium,
    Ruby,
    SpeedPotion
}

//Data of every inventory item
[Serializable]
public class ItemData
{
    public Sprite SlotImageSprite; //Changes the sprite of "SlotImage"
    public string NameOfItem; //Changes the text of "ItemNameText"
    public InventoryItemTypes ItemType; //Changes the value of "TypeOfItem"
    public GameObject ItemObject; //Object to instantiate by the player

    public int Quantity; //Used for stacking quantity of the same type of inventory item
    public bool IsThisAStackableItem;

    public ItemData Clone()
    {
        return new ItemData
        {
            SlotImageSprite = this.SlotImageSprite,
            NameOfItem = this.NameOfItem,
            ItemType = this.ItemType,
            ItemObject = this.ItemObject,
            Quantity = this.Quantity,
            IsThisAStackableItem = this.IsThisAStackableItem
        };
    }
}

//Used by "InventoryUISlot" script
[Serializable]
public struct InventorySlot
{
    public Image SlotImage;
    public TextMeshProUGUI ItemNameText;
    public InventoryItemTypes TypeOfItem; 
}
