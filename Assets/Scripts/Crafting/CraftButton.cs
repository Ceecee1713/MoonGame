using System.Collections.Generic;
using UnityEngine;

public class CraftButton : MonoBehaviour
{
    [SerializeField]
    private CraftManager craftManager;

    [SerializeField]
    private List <InventoryItem> craftingMaterials;

    public void OnCraftClick()
    {
        for(int i = 0; i < craftingMaterials.Count; i++)
            craftManager.CheckInventoryForCraftingMaterials(craftingMaterials[i]);
    }
}
