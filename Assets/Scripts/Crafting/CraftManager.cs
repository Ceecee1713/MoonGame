using UnityEngine;

public class CraftManager : MonoBehaviour
{
    [SerializeField]
    private InventoryData inventoryData;

    public void CheckInventoryForCraftingMaterials(InventoryItem craftingMaterial)
    {
        for(int i = 0; i < inventoryData.Inventory.Count; i++)
        {
            Debug.Log(inventoryData.Inventory[i].ItemData.ItemType);
        }
    }
}
