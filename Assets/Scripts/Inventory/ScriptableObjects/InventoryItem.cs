using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "InventoryItem", menuName = "Inventory System/Create an Inventory Item")]
public class InventoryItem : ScriptableObject
{
    public ItemData ItemData;
}
