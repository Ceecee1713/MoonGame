using System;
using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Manages the dialogue UI functionality - typing dialogue only and publishing events to other scripts
/// </summary>
/// 
/// <remarks>
/// This script is to be attached to a UI game object that's for dialogue that'll be layered on top of the main player UI
/// 
///  See <see cref="StorytellingDialogueData"/> for how each individual storytelling dialogue is set up. 
/// 
/// This script works together with the "PlayerInputController", "ExplorationTimer", "PlayerHealth", "GoalText", "PlayerStateMachine" scripts
/// See <see cref="PlayerInputController"/> - Listening to "AdvanceDialogueOnMainUI" that "PlayerInputController" publishes to advance dialogue
/// See <see cref="ExplorationTimer"/> - Publishing "PauseExplorationTimer" to pause/unpause exploration timer countdown
/// See <see cref="PlayerHealth"/> - Publishing "MaintainPlayerHealth" to maintain / not maintain player's current health
/// See <see cref="GoalText"/> - Publishing "ShowBeginnerGoal" to display the beginning goal text when you start the game
/// See <see cref="PlayerStateMachine"/> - Publishing "FreezePlayer" to unfreeze the player
/// 
/// </remarks>

public class DialogueCanvas : MonoBehaviour
{
    [SerializeField]
    private AudioClip nextMessageSFX;

    [SerializeField]
    private TextMeshProUGUI dialogueText;

    private StorytellingDialogueData _currentDialogue;

    private bool _finishedTypingMessage = false;
    private bool _newExplorationPhase = false;
    private bool _startingTheGame = false;

    private int _dialogueArrayLength;
    private int _index = 0; //Index to go through the dialogue message array (individual messages) from "dialogueData" 

    private const float TYPING_SPEED = 0.01f;

    private const bool ALLOW_PLAYER_INPUTS = true; 

    void Awake()
    {
        EventBus.Instance.Subscribe<TypeDialogueOnMainUI>(DisplayMessage);
        EventBus.Instance.Subscribe<AdvanceDialogueOnMainUI>(FinishMessage);
    }

    void Start()
    {
        this.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        ResetValues();
    }

    private void ResetValues()
    {
        dialogueText.text = "";
        _finishedTypingMessage = false;
        _newExplorationPhase = false;
        _index = 0;
    }

    //"AdvanceDialogueOnMainUI" is the name of an event. Empty event
    private void FinishMessage(AdvanceDialogueOnMainUI advanceDialogueOnMainUI) //Published by "PlayerInputController"
    {
        if(_finishedTypingMessage != true)
            return;

        if(_index+1 == _dialogueArrayLength)
        {
            EventBus.Instance.Publish(new FreezePlayer(false)); //Publish to "PlayerStateMachine"
            EventBus.Instance.Publish(new MaintainPlayerHealth(false)); //Publish to "PlayerHealth"

            if(_newExplorationPhase == true || _startingTheGame == true)
            {
                EventBus.Instance.Publish(new ResetExplorationTimer()); //Publish to "ExplorationTimer"
                EventBus.Instance.Publish(new ActivatePlayerInputs(ALLOW_PLAYER_INPUTS)); //Multiple subscribers and publishers
            }

            if(_startingTheGame == true)
                EventBus.Instance.Publish(new ShowBeginnerGoal()); //Publish to "GoalText"

            StopAllCoroutines();
            this.gameObject.SetActive(false);
            return;
        }

        _index++;
        AudioManager.Instance.PlaySoundEffect(nextMessageSFX);
        StopAllCoroutines();
        StartCoroutine(TypeMessage(_currentDialogue.Messages[_index]));
    }

    //Receives a "TypeDialogueOnMainUI" event with parameters:
    //(StorytellingDialogueData) Dialogue - dialogue to advance through and display
    //(bool) NewExplorationPhase - (true = it's a new exploration phase, 
    //false = it is NOT a new exploration phase).
    //(bool) StartingTheGame - (true = it's the start of the game,
    //false = it is NOT the start of the game).
    private void DisplayMessage(TypeDialogueOnMainUI typeDialogueOnMainUI) //Multiple publishers
    {
        _newExplorationPhase = typeDialogueOnMainUI.NewExplorationPhase;
        _startingTheGame = typeDialogueOnMainUI.StartingTheGame;
        _currentDialogue = typeDialogueOnMainUI.Dialogue;
        _dialogueArrayLength = _currentDialogue.Messages.Length;

        StopAllCoroutines();
        StartCoroutine(TypeMessage(_currentDialogue.Messages[_index]));
    }

    private IEnumerator TypeMessage(StorytellingDialogueData.DialogueLine dialogueLine) 
    {
        _finishedTypingMessage = false;

        string message = dialogueLine.message;

        if (dialogueLine.uniqueWordsToColour != null)
        {
            //Convert colour to a hex string that TMP's <color> tag accepts
            string hexColor = ColorUtility.ToHtmlStringRGB(dialogueLine.wordColour);

            foreach (string word in dialogueLine.uniqueWordsToColour)
            {
                if (!string.IsNullOrEmpty(word)) //Wrap each special word in colour tags 
                    message = message.Replace(word, $"<color=#{hexColor}>{word}</color>");
            }
        }

        dialogueText.text = message; //Assign fully made string with all rich text tags (colour tags)
        dialogueText.maxVisibleCharacters = 0; // Hide all characters until the loop reveals them

        //Force TMP to fully parse and lay out the text so characterCount is accurate before the loop
        dialogueText.ForceMeshUpdate();

        //characterCount only counts visible characters, ignoring rich text tags (colour tags) 
        int totalVisibleChars = dialogueText.textInfo.characterCount;

        for (int i = 0; i <= totalVisibleChars; i++) 
        {
            dialogueText.maxVisibleCharacters = i; //Reveal one character at a time to mimic typing
            yield return new WaitForSeconds(TYPING_SPEED);
        }

        _finishedTypingMessage = true;
    }
}
