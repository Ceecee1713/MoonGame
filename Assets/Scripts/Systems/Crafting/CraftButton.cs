using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftButton : MonoBehaviour
{
    [SerializeField]
    private AudioClip buttonClickSFX;

    [SerializeField]
    private CraftManager craftManager;

    [SerializeField]
    private ItemData craftableInventoryItem;

    [SerializeField]
    private List <ItemData> craftingMaterials;

    private bool _allowClicking = true;

    private const bool CRAFTING_A_CLUE = false;

    void OnEnable()
    {
    }

    void OnDisable()
    {
        _allowClicking = true;
    }

    public void OnCraftInventoryItemClick()
    {
        if(_allowClicking != true)
        return;

        _allowClicking = false;

        AudioManager.Instance.PlaySoundEffect(buttonClickSFX);
        
        craftManager.ResetStatus();
        craftManager.SetInventoryItemToCraft(craftableInventoryItem);

        
        for(int i = 0; i < craftingMaterials.Count; i++)
            craftManager.CheckInventoryForCraftingMaterials(craftingMaterials[i]);

        craftManager.TryCompleteCraft(craftingMaterials.Count, CRAFTING_A_CLUE);
        craftManager.DelayClickingOfButtons();
    }

    public void AllowClicking()
    {
        _allowClicking = true;
    }
}
