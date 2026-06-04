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
/// See <see cref="AcceptButton"/> for how they work together - accessing public methods as they're on the same game object
/// 
/// This script also works together with "MoonPuzzleArea" 
/// See <see cref="MoonPuzzleArea"/> for how they work together - publishing "StopMoonPuzzleAreaAudio" and 
/// listening to "OpenTextAdventureUI" that "MoonPuzzleArea" script publishes.
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
        EventBus.Instance.Subscribe<OpenTextAdventureUI>(DisplayButtonOptions);
        this.gameObject.SetActive(false);
    }
    
    void OnEnable()
    {
    }

    void OnDisable()
    {
        declineButton.SetActive(false);
        acceptButton.SetActive(false);
    }

    private void DisplayButtonOptions(OpenTextAdventureUI openTextAdventureUI) //Published by "MoonPuzzleArea"
    {
        _currentMoonPuzzleAreaNumber = openTextAdventureUI.CurrentMoonPuzzleAreaNumber;
        declineButton.SetActive(true);
        acceptButton.SetActive(true);
    }

    public void FadeMoonPuzzleAreaAudio() //Called by "AcceptButton"
    {
        EventBus.Instance.Publish(new StopMoonPuzzleAreaAudio(_currentMoonPuzzleAreaNumber)); //Publish to "MoonPuzzleArea"
    }
}
