using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    void Start()
    {
        EventBus.Instance.Subscribe<SpawnDroppedInventoryItem>(SpawnInventoryItem);
    }

    private void SpawnInventoryItem(SpawnDroppedInventoryItem spawnDroppedInventoryItem)
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
