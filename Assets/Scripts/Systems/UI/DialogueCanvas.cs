using System;
using System.Collections;
using UnityEngine;
using TMPro;

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
    private const float DELAY = 1.5f;

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

    void OnEnable()
    {
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

    private void FinishMessage(AdvanceDialogueOnMainUI advanceDialogueOnMainUI) //Called by "PlayerInputController" (keybind Enter/left mouse click)
    {
        if(_finishedTypingMessage != true)
            return;

        if(_index+1 == _dialogueArrayLength)
        {
            EventBus.Instance.Publish(new FreezePlayer(false));
            EventBus.Instance.Publish(new MaintainPlayerHealth(false));

            if(_newExplorationPhase == true || _startingTheGame == true)
            {
                EventBus.Instance.Publish(new ResetExplorationTimer());
                EventBus.Instance.Publish(new ActivatePlayerInputs(ALLOW_PLAYER_INPUTS));
            }
                

            if(_startingTheGame == true)
                EventBus.Instance.Publish(new ShowBeginnerGoal());

            StopAllCoroutines();
            this.gameObject.SetActive(false);
            return;
        }

        _index++;
        AudioManager.Instance.PlaySoundEffect(nextMessageSFX);
        StopAllCoroutines();
        StartCoroutine(TypeMessage(_currentDialogue.Messages[_index]));
    }

    private void DisplayMessage(TypeDialogueOnMainUI typeDialogueOnMainUI)
    {
        _newExplorationPhase = typeDialogueOnMainUI.NewExplorationPhase;
        _startingTheGame = typeDialogueOnMainUI.StartingTheGame;
        _currentDialogue = typeDialogueOnMainUI.Dialogue;
        _dialogueArrayLength = _currentDialogue.Messages.Length;

        StopAllCoroutines();
        StartCoroutine(TypeMessage(_currentDialogue.Messages[_index]));
    }

    IEnumerator TypeMessage(StorytellingDialogueData.DialogueLine dialogueLine) 
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
