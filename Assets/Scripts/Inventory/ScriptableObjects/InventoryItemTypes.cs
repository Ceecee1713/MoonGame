using System;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

//Represent types of the all possible inventory items
public enum InventoryItemTypes 
{
    Stone,
    Beryllium
}

//Used by "InventoryItem" script 
//Data of every inventory item
[Serializable]
public struct ItemData
{
    public Sprite SlotImageSprite; //Changes the sprite of "SlotImage"
    public string NameOfItem; //Changes the text of "ItemNameText"
    public InventoryItemTypes ItemType; 
}

//Used by "InventoryUISlot" script
[Serializable]
public struct InventorySlot
{
    public Image SlotImage;
    public TextMeshProUGUI ItemNameText;
    public InventoryItemTypes typeOfItem; 
}
