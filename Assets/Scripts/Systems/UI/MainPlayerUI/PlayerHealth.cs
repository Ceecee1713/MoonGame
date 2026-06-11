using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the slider that controls the player's health and publishes events when it's "game over" when health is dropped fully
/// </summary>
/// 
/// <remarks>
/// This script works together with these scripts: PlayerStateMachine, ExplorationTimer, CanvasManager, MoonPuzzleDialogueText, 
/// StorytellingDialogueText, FirstSafeZone, SafeZone, CorriosonZone
/// 
/// See <see cref="PlayerStateMachine"/> - Publishing "FreezePlayer" to freeze the player
/// See <see cref="ExplorationTimer"/> - Publishing "PauseExplorationTimer" to pause the exploration timer countdown
/// See <see cref="CanvasManager"/> - Publishing "ChangeCanvases" to swap canvases
/// See <see cref="MoonPuzzleDialogueText"/> - Listening to "NewExplorationPhase" event that "MoonPuzzleDialogueText" publishes to reset health
/// See <see cref="StorytellingDialogueText"/> - Listening to "NewExplorationPhase" event that "StorytellingDialogueText" publishes to reset health
/// See <see cref="FirstSafeZone"/> - Listening to "AlterPlayerHealth" event that "FirstSafeZone" publishes to alter player's current health
/// See <see cref="SafeZone"/> - Listening to "AlterPlayerHealth" event that "SafeZone" publishes to alter player's current health
/// See <see cref="CorriosonZone"/> - Listening to "AlterPlayerHealth" event that "CorriosonZone" publishes to alter player's current health
/// 
/// This script works with multiple other scripts that publish and subscribe to "ActivatePlayerInputs"
/// Please see <see cref="AddItemToInventory"/> to get the full details as it would be too much to write in this script alone
/// 
/// This script works with multiple other scripts that publish "MaintainPlayerHealth"
/// Please see <see cref="AddItemToInventory"/> to get the full details as it would be too much to write in this script alone
/// 
/// </remarks>

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private GameObject failedGameUI; 
    
    [SerializeField]
    private Slider health;

    private bool _pauseCorrioson = false; 
    private bool _promptedFailureGameScreen = false;
    private bool _recoverHealth = false;
    
    private float _speedToChangeHealth;

    private const float HEALTH_VALUE_TO_DIE_AT = 0.125f; //0.0f doesn't match the visuals of the health slider
    private const float TIME_DELAY_BEFORE_SWAPPING_UI_SCREENS = 0.2f;

    private const bool ALLOW_PLAYER_INPUT = false;
    private const bool START_MOON_PUZZLE = false;
    private const bool START_PRAYER_PHASE = false; 

    void Start()
    {
        health.interactable = false;

        EventBus.Instance.Subscribe<AlterPlayerHealth>(ChangeHealthValue);
        EventBus.Instance.Subscribe<MaintainPlayerHealth>(ApplyCorrioson);
        EventBus.Instance.Subscribe<NewExplorationPhase>(StartNewExplorationPhase);
    }

    void Update()
    {
        if( _pauseCorrioson == true || _promptedFailureGameScreen == true)
            return;

        if(_recoverHealth == false && health.value > HEALTH_VALUE_TO_DIE_AT)
            health.value -= Time.deltaTime * _speedToChangeHealth;

        if(_recoverHealth == true && health.value != 1.0f)
            health.value += Time.deltaTime * _speedToChangeHealth;

        if(health.value <= HEALTH_VALUE_TO_DIE_AT) 
        {
            StartCoroutine(ShowFailedGameScreen());
            _promptedFailureGameScreen = true;
        }
    }

    //Receives a "AlterPlayerHealth" event with parameters:
    //(bool) RecoverHealth - (true = increase the player's current health
    //false = lower the player's current health)
    //(float) SpeedToChangeHealth - speed to change player's health by
    private void ChangeHealthValue(AlterPlayerHealth alterPlayerHealth) //Published by "FirstSafeZone", "SafeZone" or "CorriosonZone"
    {
        _recoverHealth = alterPlayerHealth.RecoverHealth;
        _speedToChangeHealth = alterPlayerHealth.SpeedToChangeHealth;
    }

    //Receives a "MaintainPlayerHealth" event with parameters:
    //(bool) PauseCorrioson - (true = maintain the player's current health
    //false = allow the player's health to be changed)
    private void ApplyCorrioson(MaintainPlayerHealth maintainPlayerHealth) //Multiple publishers
    {
        _pauseCorrioson = maintainPlayerHealth.PauseCorrioson;
    }

    //"NewExplorationPhase" is the name of an event. Empty event
    private void StartNewExplorationPhase(NewExplorationPhase newExplorationPhase) //Published by "MoonPuzzleDialogueText" or "StorytellingDialogueText"
    {
        health.value = 1.0f; //Reset health to full
        _recoverHealth = true;
    }

    private IEnumerator ShowFailedGameScreen()
    {
        EventBus.Instance.Publish(new FreezePlayer(true)); //Publish to "PlayerStateMachine"
        EventBus.Instance.Publish(new PauseExplorationTimer(true)); //Publish to "ExplorationTimer"
        EventBus.Instance.Publish(new ActivatePlayerInputs(false)); //Multiple subscribers and publishers

        yield return new WaitForSeconds(TIME_DELAY_BEFORE_SWAPPING_UI_SCREENS);

        EventBus.Instance.Publish(new ChangeCanvases(failedGameUI, START_MOON_PUZZLE, START_PRAYER_PHASE)); //Publish to "CanvasManager"
    }
}
