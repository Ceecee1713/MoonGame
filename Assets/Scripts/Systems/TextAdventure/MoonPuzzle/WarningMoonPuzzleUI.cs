using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class WarningMoonPuzzleUI : MonoBehaviour
{
    [Header ("UI Buttons")]
    [SerializeField]
    private GameObject declineButton;
    [SerializeField]
    private GameObject acceptButton;
    [SerializeField]
    private AcceptButton acceptButtonScript;

    private MoonPuzzleArea moonPuzzleAreaInteractedWith;
    
    void OnEnable()
    {
    }

    void OnDisable()
    {
        declineButton.SetActive(false);
        acceptButton.SetActive(false);
    }

    public void ShowWarningMessage(MoonPuzzleArea moonPuzzleArea)
    {
        moonPuzzleAreaInteractedWith = moonPuzzleArea;
        declineButton.SetActive(true);
        acceptButton.SetActive(true);
    }

    public void MarkMoonAreaAsBeenExplored()
    {
        moonPuzzleAreaInteractedWith.InteractedOnce = true;
    }
}
