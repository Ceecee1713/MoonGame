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

        PickUpItem instanceScript = instance.GetComponent<PickUpItem>();
        
        //Setting the instantiated item's paramaters to be the same as the original item's paramaters
        if (instanceScript != null)
            instanceScript.inventoryItem = spawnDroppedInventoryItem.InventoryItem.Clone();
    }
}
