using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonPuzzleArea : MonoBehaviour
{
    [Header ("Audio")]
    [SerializeField]
    private AudioSource moonPuzzleAreaAudioSource; //Audio Source Game Object MUST be attached and configured in scene, not from Assets
    [SerializeField]
    private float totalTimeDurationToFadeAudioSource;

    [Header ("Warning Moon Puzzle UI Config")]
    [SerializeField]
    private GameObject warningMoonPuzzleUI;
    [SerializeField]
    private WarningMoonPuzzleUI warningMoonPuzzleUIScript;

    private bool _allowInput = true;
    private bool _playerCollisionDetected = false;

    private const float DESIRED_VOLUME = 0.0f;

    private const bool START_MOON_PUZZLE = true;
    
    private const bool START_PRAYER_PHASE = false;

    void Start()
    {
        EventBus.Instance.Subscribe<Interact>(OpenTextAdventureUI);
        EventBus.Instance.Subscribe<ActivatePlayerInputs>(AllowPlayerInput);
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
        }
    }

    void OnDestroy()
    {
        if (EventBus.Instance != null)
        {
            EventBus.Instance.Unsubscribe<Interact>(OpenTextAdventureUI);
            EventBus.Instance.Unsubscribe<ActivatePlayerInputs>(AllowPlayerInput);
        }
    }

    public void FadeAudio() //Called by WarningMoonPuzzleUI
    {
        if (!enabled) 
            return;

        moonPuzzleAreaAudioSource.spatialBlend = 0f;
        AudioManager.Instance.FadeVolumeOfAudioSource(moonPuzzleAreaAudioSource, totalTimeDurationToFadeAudioSource, DESIRED_VOLUME);
        enabled = false;
    }

    private void AllowPlayerInput(ActivatePlayerInputs activatePlayerInputs)
    {
        _allowInput = activatePlayerInputs.AllowInputs;
    }

    private void OpenTextAdventureUI(Interact interact) //When player "interacts" with this game object (keybind E)
    {
        if(_allowInput == false)
            return;

        if(_playerCollisionDetected == true)
        {
            EventBus.Instance.Publish(new FreezePlayer(true));
            EventBus.Instance.Publish(new MaintainPlayerHealth(true));
            EventBus.Instance.Publish(new PauseExplorationTimer(true));
            
            warningMoonPuzzleUI.SetActive(true);
            warningMoonPuzzleUIScript.ShowWarningMessage(this);
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
            _playerCollisionDetected = true;
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
            _playerCollisionDetected = false; 
    }
}
