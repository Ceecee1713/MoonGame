using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryData", menuName = "Inventory System/Store all Inventory Items")]
public class InventoryData : ScriptableObject
{
    public List <ItemData> Inventory = new List <ItemData> (8);
}
