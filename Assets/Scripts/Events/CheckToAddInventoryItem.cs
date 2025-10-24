using System.Collections.Generic;
using UnityEngine;

//This contains all the events (data types) that the game uses

public class CheckToAddInventoryItem : IEvent 
{
    public ItemData InventoryItem;

    public CheckToAddInventoryItem(ItemData inventoryItem)
    {
        InventoryItem = inventoryItem;
    }
}

public class CheckToAddCraftedItem : IEvent 
{
    public ItemData InventoryItem;
    public List <ItemData> CraftingMaterialItems;
    public List <int> AmountsPerStackableItemToRemove;

    public CheckToAddCraftedItem(ItemData inventoryItem, List <ItemData> craftingMaterialItems, List <int> amountsPerStackableItemToRemove)
    {
        InventoryItem = inventoryItem;
        CraftingMaterialItems = craftingMaterialItems;
        AmountsPerStackableItemToRemove = amountsPerStackableItemToRemove;
    }
}

public class SelectInventoryItem : IEvent 
{
    public ItemData InventoryItem;
    public InventoryUISlot InventoryUISlot;

    public SelectInventoryItem(ItemData inventoryItem, InventoryUISlot inventoryUISlot)
    {
        InventoryItem = inventoryItem;
        InventoryUISlot = inventoryUISlot;
    }
}

public class SpawnDroppedInventoryItem : IEvent
{
    public ItemData InventoryItem;

    public SpawnDroppedInventoryItem(ItemData inventoryItem)
    {
        InventoryItem = inventoryItem;
    }
}

public class StopCraftingTemporarily : IEvent
{
    public bool ShowingWarningMessage;

    public StopCraftingTemporarily(bool showingWarningMessage)
    {
        ShowingWarningMessage = showingWarningMessage;
    }
}

public class FreezePlayer : IEvent
{
    public bool PausePlayerMovement;

    public FreezePlayer(bool pausePlayerMovement)
    {
        PausePlayerMovement = pausePlayerMovement;
    }
}

public class Interact : IEvent
{

}

public class DropEquipedInventoryItem : IEvent
{

}
