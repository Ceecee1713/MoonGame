using System;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

//Types of the all possible inventory items
public enum InventoryItemTypes 
{
    Stone,
    Beryllium,
    Ruby
}

//Used by "InventoryItem" script 
//Data of every inventory item
[Serializable]
public struct ItemData
{
    public Sprite SlotImageSprite; //Changes the sprite of "SlotImage"
    public string NameOfItem; //Changes the text of "ItemNameText"
    public InventoryItemTypes ItemType; //Changes the value of "TypeOfItem"
    public GameObject ItemObject; //Object to instantiate by the player

    public int Quantity; //Used for stacking quantity of the same type of inventory item
    public bool IsThisAStackableItem;
}

//Used by "InventoryUISlot" script
[Serializable]
public struct InventorySlot
{
    public Image SlotImage;
    public TextMeshProUGUI ItemNameText;
    public InventoryItemTypes TypeOfItem; 
}
