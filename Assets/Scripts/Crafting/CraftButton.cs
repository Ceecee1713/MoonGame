using System.Collections.Generic;
using UnityEngine;

public class CraftButton : MonoBehaviour
{
    [SerializeField]
    private CraftManager craftManager;

    [SerializeField]
    private ItemData craftableInventoryItem;

    [SerializeField]
    private List <ItemData> craftingMaterials;

    public void OnCraftClick()
    {
        craftManager.ResetStatus();

        for(int i = 0; i < craftingMaterials.Count; i++)
            craftManager.CheckInventoryForCraftingMaterials(craftableInventoryItem, craftingMaterials[i], craftingMaterials.Count);
    }
}
