using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds the player's current inventory and is synced up with the player's inventory that's on screen.
/// This is to be used for any script that needs an easier way to reference the player's inventory without direct accessing
/// </summary>
/// 
/// /// <remarks>
/// The ordering of items between the player's displayed inventory and what they actually have can become out of sync.
/// Thus, the "InventoryUI" iterates through some of its for-loops with this script backwards to avoid issues of desyncing
///</remarks>

[CreateAssetMenu(fileName = "InventoryData", menuName = "Inventory System/Store all Inventory Items")]
public class InventoryData : ScriptableObject
{
    public List <ItemData> Inventory = new List <ItemData> (8);

    /// <remarks>See <see cref="InventoryItemTypes"/> for inventory items are structured.</remarks>
}
