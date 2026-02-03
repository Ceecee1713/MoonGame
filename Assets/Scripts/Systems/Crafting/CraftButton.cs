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

    private bool _craftingAClue = false;

    public void OnCraftInventoryItemClick()
    {
        craftManager.ResetStatus();
        craftManager.SetInventoryItemToCraft(craftableInventoryItem);

        _craftingAClue = false;

        for(int i = 0; i < craftingMaterials.Count; i++)
            craftManager.CheckInventoryForCraftingMaterials(craftingMaterials[i], craftingMaterials.Count, _craftingAClue); 
    }
}
