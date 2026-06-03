using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the slider that controls the player's health and publishes events when it's "game over" when health is dropped fully
/// </summary>
/// 
/// <remarks>
/// This script works together with any corrioson zone scripts, safe zone scripts, 
/// //ADD OTHER SCRIPTS
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

    private void ChangeHealthValue(AlterPlayerHealth alterPlayerHealth)
    {
        _recoverHealth = alterPlayerHealth.RecoverHealth;
        _speedToChangeHealth = alterPlayerHealth.SpeedToChangeHealth;
    }

    private void ApplyCorrioson(MaintainPlayerHealth maintainPlayerHealth)
    {
        _pauseCorrioson = maintainPlayerHealth.PauseCorrioson;
    }

    private void StartNewExplorationPhase(NewExplorationPhase newExplorationPhase)
    {
        health.value = 1.0f; //Reset health to full
        _recoverHealth = true;
    }

    IEnumerator ShowFailedGameScreen()
    {
        EventBus.Instance.Publish(new FreezePlayer(true));
        EventBus.Instance.Publish(new PauseExplorationTimer(true));
        EventBus.Instance.Publish(new ActivatePlayerInputs(false));

        yield return new WaitForSeconds(TIME_DELAY_BEFORE_SWAPPING_UI_SCREENS);

        EventBus.Instance.Publish(new ChangeCanvases(failedGameUI, START_MOON_PUZZLE, START_PRAYER_PHASE));
    }
}
