using UnityEngine;

/// <summary>
/// Manages the accept button for the UI screen that appears before starting the moon puzzle text adventure and after interacting with a decorative moon statue
/// </summary>
/// 
/// <remarks>
/// This script is to be attached a button on the UI screen that asks and warns the player about entering a moon puzzle text adventure.
/// This script will be attached to a reject button to NOT proceed to the moon puzzle text adventure UI
/// 
/// This script works together with "MaintainPlayerHealth" and "PauseExplorationTimer" scripts
/// See <see cref="MaintainPlayerHealth"/> for how they work together - allow player's health to be altered
/// See <see cref="PauseExplorationTimer"/> for how they work together - unpause exploration timer
/// 
/// </remarks>

public class DeclineButton : MonoBehaviour
{
    [SerializeField]
    private AudioClip buttonClickSFX;

    [SerializeField]
    private GameObject warningMoonPuzzleUI;
    
    public void OnDeclineCilck()
    {
        AudioManager.Instance.PlaySoundEffect(buttonClickSFX);
        
        EventBus.Instance.Publish(new FreezePlayer(false));
        EventBus.Instance.Publish(new MaintainPlayerHealth(false));
        EventBus.Instance.Publish(new PauseExplorationTimer(false));
        warningMoonPuzzleUI.SetActive(false);
    }
}
