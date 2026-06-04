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
/// This script works together with the "InventoryUI", "PlayerInputController", "PlayerSpawner" scripts
/// See <see cref="InventoryUI"/> for how they work together - Adding the inventory item to player inventory
/// See <see cref="PlayerInputController"/> for how they work together - publishing the Interact event this script listens to
/// See <see cref="PlayerSpawner"/> for how they work together - Instantiating an inventory item
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

    private const bool START_MOON_PUZZLE = true;
    
    private const bool START_PRAYER_PHASE = false;

    void Start()
    {
        EventBus.Instance.Subscribe<Interact>(OpenTextAdventureUI);
        EventBus.Instance.Subscribe<ActivatePlayerInputs>(AllowPlayerInput);
        EventBus.Instance.Subscribe<StopMoonPuzzleAreaAudio>(StopMoonStatueAudio);
    } 

    void OnEnable()
    {
    }

    void OnDisable()
    {
        if (EventBus.Instance != null)
        {
            EventBus.Instance.Unsubscribe<Interact>(OpenTextAdventureUI);
            EventBus.Instance.Unsubscribe<ActivatePlayerInputs>(AllowPlayerInput);
            EventBus.Instance.Unsubscribe<StopMoonPuzzleAreaAudio>(StopMoonStatueAudio);
        }
    }

    void OnDestroy()
    {
        if (EventBus.Instance != null)
        {
            EventBus.Instance.Unsubscribe<Interact>(OpenTextAdventureUI);
            EventBus.Instance.Unsubscribe<ActivatePlayerInputs>(AllowPlayerInput);
            EventBus.Instance.Unsubscribe<StopMoonPuzzleAreaAudio>(StopMoonStatueAudio);
        }
    }

    private void StopMoonStatueAudio(StopMoonPuzzleAreaAudio stopMoonPuzzleAreaAudio) //Published by "WarningMoonPuzzleUI"
    {
        if (!enabled || moonPuzzleAreaNumber != stopMoonPuzzleAreaAudio.CurrentMoonPuzzleAreaNumber) 
            return;

        moonPuzzleAreaAudioSource.spatialBlend = 0f;
        AudioManager.Instance.FadeVolumeOfAudioSource(moonPuzzleAreaAudioSource, totalTimeDurationToFadeAudioSource, DESIRED_VOLUME);
        enabled = false;
    }

    private void AllowPlayerInput(ActivatePlayerInputs activatePlayerInputs)
    {
        _allowInput = activatePlayerInputs.AllowInputs;
    }

    private void OpenTextAdventureUI(Interact interact) //When player interacts with this game object
    {
        if(_allowInput == false)
            return;

        if(_playerCollisionDetected == true)
        {
            EventBus.Instance.Publish(new FreezePlayer(true));
            EventBus.Instance.Publish(new MaintainPlayerHealth(true));
            EventBus.Instance.Publish(new PauseExplorationTimer(true));
            
            warningMoonPuzzleUI.SetActive(true);
            //warningMoonPuzzleUIScript.ShowWarningMessage(this);
            EventBus.Instance.Publish(new OpenTextAdventureUI(moonPuzzleAreaNumber)); //Publish to "WarningMoonPuzzleUI"
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            _playerCollisionDetected = true;
            EventBus.Instance.Publish(new InCollision(_playerCollisionDetected));
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            _playerCollisionDetected = false; 
            EventBus.Instance.Publish(new InCollision(_playerCollisionDetected));
        }
    }
}
