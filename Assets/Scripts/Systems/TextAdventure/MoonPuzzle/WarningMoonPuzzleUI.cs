using System;
using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Manages the UI screen that appears before starting the moon puzzle text adventure and after interacting with a decorative moon statue
/// </summary>
/// 
/// <remarks>
/// This script is to be attached to the UI screen that asks and warns the player about entering a moon puzzle text adventure.
/// This UI will consist of an accept and reject button to either proceed to the moon puzzle text adventure UI or not
/// This script works together with the accept button script
/// See <see cref="AcceptButton"/> - accessing public methods as they're on the same game object
/// 
/// This script also works together with "MoonPuzzleArea" 
/// See <see cref="MoonPuzzleArea"/> - Publishing "StopMoonPuzzleAreaAudio" to stop audio of the current moon puzzle area, 
/// and listening to "OpenWarningMoonPuzzleUI" that "MoonPuzzleArea" script publishes to display warning moon puzzle UI
/// 
/// This UI screen will be set active by the "MoonPuzzleArea" script 
/// 
/// </remarks>

public class WarningMoonPuzzleUI : MonoBehaviour
{
    [Header ("UI Buttons")]
    [SerializeField]
    private GameObject declineButton;
    [SerializeField]
    private GameObject acceptButton;
    [SerializeField]
    private AcceptButton acceptButtonScript;

    private int _currentMoonPuzzleAreaNumber; 

    void Start()
    {
        EventBus.Instance.Subscribe<OpenWarningMoonPuzzleUI>(DisplayButtonOptions);
        this.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        declineButton.SetActive(false);
        acceptButton.SetActive(false);
    }

    //Receives a "OpenWarningMoonPuzzleUI" event with parameters:
    //(int) CurrentMoonPuzzleAreaNumber - number of the current moon puzzle area player is in
    private void DisplayButtonOptions(OpenWarningMoonPuzzleUI openWarningMoonPuzzleUI) //Published by "MoonPuzzleArea"
    {
        _currentMoonPuzzleAreaNumber = openWarningMoonPuzzleUI.CurrentMoonPuzzleAreaNumber;
        declineButton.SetActive(true);
        acceptButton.SetActive(true);
    }

    public void FadeMoonPuzzleAreaAudio() //Called by "AcceptButton"
    {
        EventBus.Instance.Publish(new StopMoonPuzzleAreaAudio(_currentMoonPuzzleAreaNumber)); //Publish to "MoonPuzzleArea"
    }
}
