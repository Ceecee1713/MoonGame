using UnityEngine;

/// <summary>
/// Manages the close button on the chest UI pop up
/// This script is to be attached to the close button on the chest UI game object 
/// </summary>
/// 
/// <remarks>
/// 
/// See <see cref="PlayerStateMachine"/> - Publishing "FreezePlayer" to freeze player
/// See <see cref="PlayerHealth"/> - Publishing "MaintainPlayerHealth" to no longer maintain player's current health 
/// See <see cref="InventoryUISlot"/> - Publishing "ChestIsOpen" that a chest is no longer open
/// See <see cref="OpenCluebookMainUI"/> - Publishing "ChestIsOpen" that a chest is no longer open
/// 
/// See <see cref="InventoryItemTypes"/> for what makes up an inventory item.
/// 
/// </remarks>

public class ChestButton : MonoBehaviour
{
    [SerializeField]
    private AudioClip buttonClickSFX;

    [SerializeField]
    private GameObject chestUI;

    public void OnCloseChestUIClick()
    {
        AudioManager.Instance.PlaySoundEffect(buttonClickSFX);
        
        EventBus.Instance.Publish(new FreezePlayer(false)); //Publish to "PlayerStateMachine"
        EventBus.Instance.Publish(new MaintainPlayerHealth(false)); //Publish to "PlayerHealth"
        EventBus.Instance.Publish(new ChestIsOpen(false)); //Publish to "InventoryUISlot" and "OpenCluebookMainUI" 
        chestUI.SetActive(false);
    }
}
