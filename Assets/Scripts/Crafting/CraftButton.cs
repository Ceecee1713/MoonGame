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

    private bool _stopCrafting = false;

    void Start()
    {
        EventBus.Instance.Subscribe<StopCraftingTemporarily>(AllowButtonInteractionToCraft);
    }

    private void AllowButtonInteractionToCraft(StopCraftingTemporarily stopCraftingTemporarily)
    {
        _stopCrafting = stopCraftingTemporarily.ShowingWarningMessage;
    }

    public void OnCraftClick()
    {
        if(_stopCrafting == true)
            return; 

        craftManager.ResetStatus();

        for(int i = 0; i < craftingMaterials.Count; i++)
            craftManager.CheckInventoryForCraftingMaterials(craftableInventoryItem, craftingMaterials[i], craftingMaterials.Count);
    }
}
