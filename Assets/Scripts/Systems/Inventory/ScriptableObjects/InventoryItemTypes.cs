using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

//Types of the all possible inventory items
public enum InventoryItemTypes 
{
    None,
    Chest,
    Stone,
    Beryllium,
    Ruby,
    SpeedPotion, 
    Sapphire,
    Gold,
    Wood,
    Fern,
    Berries,
    TornDictionary,
    UsedDictionary,
    SophisticatedDictionary
}

public static class EnumExtensions
{
    public static string ToDisplayName(this Enum value)
    {
        return Regex.Replace(value.ToString(), "(?<!^)([A-Z])", " $1");
    }
}

/*
The regex (?<!^)([A-Z]) means:

(?<!^) — not at the start of the string
([A-Z]) — find any uppercase letter
" $1" — insert a space before it
*/

//Data of every inventory item
[Serializable]
public class ItemData
{
    public Sprite SlotImageSprite; //Changes the sprite of "ItemImage"
    public InventoryItemTypes ItemType; //Changes the value of "TypeOfItem"
    public GameObject ItemObject; //Object to instantiate by the player
    public bool IsDroppedItem = false; //Tracking if this inventory item was dropped by player

    public int Quantity; //Used for stacking quantity of the same type of inventory item
    public bool IsThisAStackableItem;

    public ItemData Clone()
    {
        return new ItemData
        {
            SlotImageSprite = this.SlotImageSprite,
            ItemType = this.ItemType,
            ItemObject = this.ItemObject,
            Quantity = this.Quantity,
            IsThisAStackableItem = this.IsThisAStackableItem,
            IsDroppedItem = this.IsDroppedItem
        };
    }
}

//Used by "InventoryUISlot" script
[Serializable]
public struct InventorySlot
{
    public Image ItemImage;
    public GameObject ItemImageObject;
    public InventoryItemTypes TypeOfItem; 
    public TextMeshProUGUI ItemQuantityText; 
}
