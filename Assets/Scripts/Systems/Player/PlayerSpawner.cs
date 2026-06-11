using UnityEngine;

/// <summary>
/// Manages instantiating an inventory item when it's dropped from the player's inventory
/// </summary>
/// 
/// <remarks>
/// See <see cref="InventoryItemTypes"/> for what makes up an inventory item and how inventory UI slots are made up.
/// 
/// This script works together with the "InventoryUISlot" and "InteractableItem" scripts
/// See <see cref="InventoryUISlot"/> - Publihsing the "SpawnDroppedInventoryItem" event
/// See <see cref="InteractableItem"/> - Utilizing public variables from "InteractableItem" while instantiating the dropped inventory item  
/// 
/// </remarks>

public class PlayerSpawner : MonoBehaviour
{
    void Start()
    {
        EventBus.Instance.Subscribe<SpawnDroppedInventoryItem>(SpawnInventoryItem);
    }

    //Receives a "SpawnDroppedInventoryItem" event with parameters:
    //(ItemData) "InventoryItem" - Inventory item to instantiate
    private void SpawnInventoryItem(SpawnDroppedInventoryItem spawnDroppedInventoryItem) //Published by "InventoryUISlot"
    {
        Quaternion prefabRotation = spawnDroppedInventoryItem.InventoryItem.ItemObject.transform.rotation;
        GameObject instance = Instantiate(spawnDroppedInventoryItem.InventoryItem.ItemObject, this.gameObject.transform.position, prefabRotation);

        if(spawnDroppedInventoryItem.InventoryItem.ItemType == InventoryItemTypes.Chest)
            return;

        InteractableItem instanceScript = instance.GetComponent<InteractableItem>();

        if (instanceScript == null)
            instanceScript = instance.AddComponent<InteractableItem>();
        
        instanceScript.inventoryItem = spawnDroppedInventoryItem.InventoryItem.Clone();
        instanceScript.inventoryItem.ItemObject = spawnDroppedInventoryItem.InventoryItem.ItemObject;
        instanceScript.inventoryItem.IsDroppedItem = true;
        instanceScript.DeleteAfterInteraction = true;

        //I'm doing this cloning and making new, blank prefabs of item drops without their scripts, then adding them later on in runtime. 
        //Why I'm doing this is because I was running into issues with instantiation during scene as I would have a parameter assigned
        //For Prefab A be itself (the parameter being a Game Object to be instatiated and that parameter would be Prefab A from the assets folder)
        //But what would happen is when I go to instantiate it here when that item is dropped in runtime, it would reference its 
        //Scene instance and not its prefab and thus that parameter would always be null. I didn't know any other workaround other than
        //To attach a script later on in runtime and use a blank prefab with no script attached (only mesh and colliders)
    }
}
