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
    private bool _craftingAClue = false;

    void Start()
    {
        EventBus.Instance.Subscribe<StopCraftingTemporarily>(AllowButtonInteractionToCraft);
    }

    //Called when a warning message pops up saying crafting materials are insufficient
    private void AllowButtonInteractionToCraft(StopCraftingTemporarily stopCraftingTemporarily)
    {
        _stopCrafting = stopCraftingTemporarily.ShowingWarningMessage;
    }

    public void OnCraftInventoryItemClick()
    {
        if(_stopCrafting == true)
            return; 

        craftManager.ResetStatus();
        craftManager.SetInventoryItemToCraft(craftableInventoryItem);

        _craftingAClue = false;

        for(int i = 0; i < craftingMaterials.Count; i++)
            craftManager.CheckInventoryForCraftingMaterials(craftingMaterials[i], craftingMaterials.Count, _craftingAClue); 
    }

    public void OnDecipherClueClick()
    {
        if(_stopCrafting == true)
            return; 

        EventBus.Instance.Publish(new CheckForCompleteClues());
        craftManager.ResetStatus();

        _craftingAClue = true;

        for(int i = 0; i < craftingMaterials.Count; i++)
            craftManager.CheckInventoryForCraftingMaterials(craftingMaterials[i], craftingMaterials.Count, _craftingAClue);
    }
}
