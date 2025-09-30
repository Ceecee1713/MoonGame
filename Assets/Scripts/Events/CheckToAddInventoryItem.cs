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

public class Interact : IEvent
{

}

public class DropEquipedInventoryItem : IEvent
{

}

public class SpawnDroppedInventoryItem : IEvent
{
    public InventoryItem InventoryItem;

    public SpawnDroppedInventoryItem(InventoryItem inventoryItem)
    {
        InventoryItem = inventoryItem;
    }
}
