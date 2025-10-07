using System.Collections.Generic;
using UnityEngine;

//This contains all the events (data types) that the game uses

public class CheckToAddInventoryItem : IEvent 
{
    public InventoryItem InventoryItem;

    public CheckToAddInventoryItem(InventoryItem inventoryItem)
    {
        InventoryItem = inventoryItem;
    }
}

public class CheckToAddCraftedItem : IEvent 
{
    public InventoryItem InventoryItem;
    public List <InventoryItem> CraftingMaterialItems;

    public CheckToAddCraftedItem(InventoryItem inventoryItem, List <InventoryItem> craftingMaterialItems)
    {
        InventoryItem = inventoryItem;
        CraftingMaterialItems = craftingMaterialItems;
    }
}

public class SelectInventoryItem : IEvent 
{
    public InventoryItem InventoryItem;
    public InventoryUISlot InventoryUISlot;

    public SelectInventoryItem(InventoryItem inventoryItem, InventoryUISlot inventoryUISlot)
    {
        InventoryItem = inventoryItem;
        InventoryUISlot = inventoryUISlot;
    }
}

public class SpawnDroppedInventoryItem : IEvent
{
    public InventoryItem InventoryItem;

    public SpawnDroppedInventoryItem(InventoryItem inventoryItem)
    {
        InventoryItem = inventoryItem;
    }
}

public class Interact : IEvent
{

}

public class DropEquipedInventoryItem : IEvent
{

}
