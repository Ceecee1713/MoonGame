using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private GameObject failedGameUI;
    
    [SerializeField]
    private Slider health;

    private bool _pauseCorrioson = false;
    private bool _doNotRepeat = false;
    private bool _recoverHealth = false;
    
    private float _speedToChangeHealth;

    private const float SMALL_TIME_DELAY = 0.2F;

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
        if( _pauseCorrioson == true || _doNotRepeat == true)
            return;

        if(_recoverHealth == false && health.value != 0.0f)
            health.value -= Time.deltaTime * _speedToChangeHealth;

        if(_recoverHealth == true && health.value != 1.0f)
            health.value += Time.deltaTime * _speedToChangeHealth;

        if(health.value == 0.0f) 
        {
            StartCoroutine(ShowFailedGameScreen());
            _doNotRepeat = true;
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
    }

    IEnumerator ShowFailedGameScreen()
    {
        EventBus.Instance.Publish(new FreezePlayer(true));
        EventBus.Instance.Publish(new PauseExplorationTimer(true));
        EventBus.Instance.Publish(new ActivatePlayerInputs(false));

        yield return new WaitForSeconds(SMALL_TIME_DELAY);

        EventBus.Instance.Publish(new ChangeCanvases(failedGameUI, START_MOON_PUZZLE, START_PRAYER_PHASE));
    }
}
