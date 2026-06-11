using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the cluebook UI functionality - only when it's set active and/or inactive (publishing events to other scripts)
/// </summary>
/// 
/// <remarks>
/// 
/// This script works together with the "PlayerHealth", "ExplorationTimer", "PlayerStateMachine",  scripts
/// See <see cref="PlayerHealth"/> - Publishing "MaintainPlayerHealth" to maintain / not maintain player's current health
/// See <see cref="ExplorationTimer"/> - Publishing "PauseExplorationTimer" to pause/unpause exploration timer countdown
/// See <see cref="PlayerStateMachine"/> - Publishing "FreezePlayer" to freeze/unfreeze player
/// 
/// This script works with multiple other scripts that publish and subscribe to "ActivatePlayerInputs"
/// Please see <see cref="AddItemToInventory"/> to get the full details as it would be too much to write in this script alone
/// 
/// </remarks>

public class CluebookUI : MonoBehaviour
{
    [Header ("UI Information")]
    [SerializeField]
    private GameObject textAdventureUI;
    [SerializeField]
    private GameObject warningMoonPopUpUI;

    private bool _allowPlayerInputs = false;

    void OnEnable()
    {
        _allowPlayerInputs = false;

        EventBus.Instance.Publish(new ActivatePlayerInputs(_allowPlayerInputs)); //Multiple subscribers and publishers
        EventBus.Instance.Publish(new FreezePlayer(true)); //Publish to "PlayerStateMachine" 
        EventBus.Instance.Publish(new MaintainPlayerHealth(true)); //Publish to "PlayerHealth"
        EventBus.Instance.Publish(new PauseExplorationTimer(true)); //Publish to "ExplorationTimer"
    }

    void OnDisable()
    {
        if(warningMoonPopUpUI.activeSelf == true)
            return;

        if(textAdventureUI.activeSelf == false)
        {
            EventBus.Instance.Publish(new FreezePlayer(false)); //Publish to "PlayerStateMachine" 
            EventBus.Instance.Publish(new MaintainPlayerHealth(false)); //Publish to "PlayerHealth"
            EventBus.Instance.Publish(new PauseExplorationTimer(false)); //Publish to "ExplorationTimer"

            _allowPlayerInputs = true;
            EventBus.Instance.Publish(new ActivatePlayerInputs(_allowPlayerInputs)); //Multiple subscribers and publishers
        }
    }
}
