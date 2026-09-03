using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages an interactable moon puzzle area - decorative moon statue
/// </summary>
/// 
/// <remarks>
/// See <see cref="InventoryItemTypes"/> for what makes up an inventory item and how inventory UI slots are made up.
/// 
/// This script works together with the scripts: 
/// "WarningMoonPuzzleUI", "PlayerInputController", "PlayerStateMachine", "PlayerHealth" , "ExplorationTimer" , "InventoryUI"
/// 
/// See <see cref="WarningMoonPuzzleUI"/> - Listening to "StopMoonPuzzleAreaAudio" event "WarningMoonPuzzleUI" publishes to
/// prompt fading of audio of the moon puzzle area 
/// and publishing "OpenTextAdventureUI" to "WarningMoonPuzzleUI" to display the warning UI before starting the moon puzzle text adventure
/// 
/// See <see cref="PlayerInputController"/> - Listening to "Interact" event that "PlayerInputController" publishes
/// See <see cref="PlayerStateMachine"/> - Publishing "FreezePlayer" event to prompt freezing a player
/// See <see cref="PlayerHealth"/> - Publishing "MaintainPlayerHealth" event to prompt maintaining player's current health
/// See <see cref="ExplorationTimer"/> - Publishing "PauseExplorationTimer" event to prompt pausing exploration timer countdown
/// See <see cref="InventoryUI"/> - Publishing "PreventPlayerInteractingWithInventory" event to prompt allowing/preventing player input with the inventory system
/// 
/// </remarks>

public class MoonPuzzleArea : MonoBehaviour
{
    [Header ("Audio")]
    [SerializeField]
    private AudioSource moonPuzzleAreaAudioSource; //Audio Source Game Object MUST be attached and configured in scene, not from Assets
    //Audio should be "shimmery" sound effects for particle effects as decorative moon statues will have particle effects

    [SerializeField]
    [Range(1, 3)]
    private int moonPuzzleAreaNumber; 

    [SerializeField]
    private float totalTimeDurationToFadeAudioSource;

    [SerializeField]
    private GameObject warningMoonPuzzleUI;

    private bool _allowInput = true; //Prevent or allow for the player to interact with this item
    private bool _playerCollisionDetected = false; //Flags if the player is inside this item's collision to be interacted with

    private const float DESIRED_VOLUME = 0.0f;

    void Start()
    {
        EventBus.Instance.Subscribe<Interact>(OpenTextAdventureUI);
        EventBus.Instance.Subscribe<ActivatePlayerInputs>(AllowPlayerInput);
        EventBus.Instance.Subscribe<StopMoonPuzzleAreaAudio>(StopMoonStatueAudio);
    } 

    void OnDisable()
    {
        if (EventBus.Exists)
        {
            EventBus.Instance.Unsubscribe<Interact>(OpenTextAdventureUI);
            EventBus.Instance.Unsubscribe<ActivatePlayerInputs>(AllowPlayerInput);
            EventBus.Instance.Unsubscribe<StopMoonPuzzleAreaAudio>(StopMoonStatueAudio);
        }
    }

    void OnDestroy()
    {
        if (EventBus.Exists)
        {
            EventBus.Instance.Unsubscribe<Interact>(OpenTextAdventureUI);
            EventBus.Instance.Unsubscribe<ActivatePlayerInputs>(AllowPlayerInput);
            EventBus.Instance.Unsubscribe<StopMoonPuzzleAreaAudio>(StopMoonStatueAudio);
        }
    }

    //Receives a "StopMoonPuzzleAreaAudio" event with parameters:
    //(int) CurrentMoonPuzzleAreaNumber - the number of the current moon puzzle area player is in
    private void StopMoonStatueAudio(StopMoonPuzzleAreaAudio stopMoonPuzzleAreaAudio) //Published by "WarningMoonPuzzleUI"
    {
        if (!enabled || moonPuzzleAreaNumber != stopMoonPuzzleAreaAudio.CurrentMoonPuzzleAreaNumber) 
            return;

        moonPuzzleAreaAudioSource.spatialBlend = 0f;
        AudioManager.Instance.FadeVolumeOfAudioSource(moonPuzzleAreaAudioSource, totalTimeDurationToFadeAudioSource, DESIRED_VOLUME);
        enabled = false;
    }

    //Receives a "ActivatePlayerInputs" event with parameters:
    //(bool) AllowInputs - (true = allow the player to interact with world objects and UI, 
    //false = do NOT allow the player to interact with world objects and UI).
    private void AllowPlayerInput(ActivatePlayerInputs activatePlayerInputs) //Multiple publishers and subscribers
    {
        _allowInput = activatePlayerInputs.AllowInputs;
    }

    //"Interact" is the name of an event. Empty event
    private void OpenTextAdventureUI(Interact interact) //When player interacts with this game object. Published by "PlayerInputController"
    {
        if(_allowInput == false)
            return;

        if(_playerCollisionDetected == true)
        {
            EventBus.Instance.Publish(new FreezePlayer(true)); //Publish to "PlayerStateMachine"
            EventBus.Instance.Publish(new MaintainPlayerHealth(true)); //Publish to "PlayerHealth" 
            EventBus.Instance.Publish(new PauseExplorationTimer(true)); //Publish to "ExplorationTimer"
            
            warningMoonPuzzleUI.SetActive(true);
            EventBus.Instance.Publish(new OpenWarningMoonPuzzleUI(moonPuzzleAreaNumber)); //Publish to "WarningMoonPuzzleUI"
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            _playerCollisionDetected = true;
            EventBus.Instance.Publish(new PreventPlayerInteractingWithInventory(_playerCollisionDetected)); //Publish to "InventoryUI"
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            _playerCollisionDetected = false; 
            EventBus.Instance.Publish(new PreventPlayerInteractingWithInventory(_playerCollisionDetected)); //Publish to "InventoryUI"
        }
    }
}
