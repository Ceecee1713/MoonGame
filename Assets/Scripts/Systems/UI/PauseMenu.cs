using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the pause menu UI functionality and publishing events to other scripts
/// </summary>
/// 
/// <remarks>
/// This script is to be attached to the pause menu game object
/// 
/// This script works together with the "PlayerInputController", "ExplorationTimer", "PlayerHealth", "PlayerStateMachine" scripts
/// See <see cref="PlayerInputController"/> - Listening to "PauseGame" event that "PlayerInputController" publishes to make pause menu active
/// See <see cref="ExplorationTimer"/> - Publishing "PauseExplorationTimer" to pause/unpause exploration timer countdown
/// See <see cref="PlayerHealth"/> - Publishing "MaintainPlayerHealth" to maintain / not maintain player's current health
/// See <see cref="PlayerStateMachine"/> - Publishing "FreezePlayer" to freeze / unfreeze player 
/// 
/// </remarks>

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject [] uisToCheckFor; //storytellingUI, textAdventureUI, cluebookUI, craftingUI, win and lose UIs

    private bool _allowPlayerInputs = false;

    void Awake()
    {
        EventBus.Instance.Subscribe<PauseGame>(DisplayPauseMenu);
    }

    void Start()
    {
        this.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        EventBus.Instance.Publish(new FreezePlayer(true)); //Publish to "PlayerStateMachine"
        EventBus.Instance.Publish(new MaintainPlayerHealth(true)); //Publish to "PlayerHealth"
        EventBus.Instance.Publish(new PauseExplorationTimer(true)); //Publish to "ExplorationTimer"

        //Prevent Player Inputs
        _allowPlayerInputs = false;
        EventBus.Instance.Publish(new ActivatePlayerInputs(_allowPlayerInputs)); //Multiple publishers and subscribers
    }

    void OnDisable()
    {
        EventBus.Instance.Publish(new FreezePlayer(false)); //Publish to "PlayerStateMachine"
        EventBus.Instance.Publish(new MaintainPlayerHealth(false)); //Publish to "PlayerHealth"
        EventBus.Instance.Publish(new PauseExplorationTimer(false)); //Publish to "ExplorationTimer"

        //Allow Player Inputs
        _allowPlayerInputs = true;
        EventBus.Instance.Publish(new ActivatePlayerInputs(_allowPlayerInputs)); //Multiple publishers and subscribers
    }

    //"PauseGame" is the name of an event. Empty event
    private void DisplayPauseMenu(PauseGame pauseGame) //Published by "PlayerInputController"
    {
        for(int i = 0; i < uisToCheckFor.Length; i++)
        {
            if(uisToCheckFor[i].activeSelf == true)
                return;
        }
            this.gameObject.SetActive(true);
    }
}
