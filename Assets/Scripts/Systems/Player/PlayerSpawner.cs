using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    void Start()
    {
        EventBus.Instance.Subscribe<SpawnDroppedInventoryItem>(SpawnInventoryItem);
    }

    private void SpawnInventoryItem(SpawnDroppedInventoryItem spawnDroppedInventoryItem)
    {
        Instantiate(spawnDroppedInventoryItem.InventoryItem.ItemObject, this.gameObject.transform.position, Quaternion.Euler(0,0,0));
    }
}
