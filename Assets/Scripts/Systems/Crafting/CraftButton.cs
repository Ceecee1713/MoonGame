using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Assigns the craftable item for the crafting table and its materials.
/// </summary>
/// 
/// <remarks>
/// This script works closely with "CraftManager" script for it to run through each material and see 
/// if the player's inventory has the right amount of that particular material and enough space in their inventory to add a new item.
/// See <see cref="CraftManager"/> for how this script interacts with the CraftManager's various methods.
/// 
/// This script is designed to be on a button game object 
/// This script is on the same game object as the "CraftManager", so it can directly access any of the CraftManager's public methods 
/// 
/// See <see cref="InventoryItemTypes"/> for how inventory items are structured.
/// 
///</remarks>

public class CraftButton : MonoBehaviour
{
    [SerializeField]
    private AudioClip buttonClickSFX;

    [SerializeField]
    private CraftManager craftManager;

    [SerializeField]
    private ItemData craftableInventoryItem; 

    [SerializeField] private List <ItemData> craftingMaterials; //Crafting materials are the same as normal inventory items

    private bool _allowClicking = true; //Prevent or allow for the player to click on the button game object this script is attached to

    private const bool CRAFTING_A_CLUE = false;

    void OnEnable()
    {
    }

    void OnDisable()
    {
        _allowClicking = true;
    }

    public void OnCraftInventoryItemClick() //Method to attach to button
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
