using UnityEngine;

/// <summary>
/// Manages the cluebook button on the cluebook UI to close the cluebook UI
/// </summary>
/// 
/// <remarks>
/// This script is to be attached to the cluebook game UI
/// 
/// This script works together with "PlayerStateMachine" and "ExplorationTimer" scripts
/// See <see cref="PlayerStateMachine"/> - Publishing "FreezePlayer" to freeze player
/// See <see cref="ExplorationTimer"/> - Publishing "PauseExplorationTimer" to unpause exploration timer countdown
/// 
/// </remarks>

public class CloseCraftingUI : MonoBehaviour
{
    [SerializeField]
    private AudioClip buttonClickSFX;

    [SerializeField]
    private GameObject craftingUI;

    public void CloseUIClick()
    {
        AudioManager.Instance.PlaySoundEffect(buttonClickSFX);
        
        EventBus.Instance.Publish(new FreezePlayer(false)); //Publish to "PlayerStateMachine"
        EventBus.Instance.Publish(new PauseExplorationTimer(false)); //Publish to "ExplorationTimer"
        craftingUI.SetActive(false);
    }
}
