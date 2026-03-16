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
                EventBus.Instance.Publish(new ResetExplorationTimer());

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

    IEnumerator TypeMessage(string message) 
    {
        _finishedTypingMessage = false;
        dialogueText.text = ""; //Clearing the "dialogueText".text for the new dialouge to be said
        
        foreach (char letter in message.ToCharArray()) //Conversion of string to a char array to mimick a "typing" effect of dialouge
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(TYPING_SPEED); //Time in between of each character being typed out
        } 

        _finishedTypingMessage = true;
    }
}
