using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueText : MonoBehaviour
{
    public MoonPuzzleTextAdventure TextAdventureDialogue;

    public int WrongButtonChoicesCounter = 0;

    [Header ("UI Information")]
    [SerializeField]
    private GameObject textAdventureUI;
    [SerializeField]
    private TextMeshProUGUI dialogueText;

    [Header ("Button Displays")]
    [SerializeField]
    private GameObject returnButton;
    [SerializeField]
    private GameObject ButtonOptions;
    [SerializeField]
    private TextMeshProUGUI buttonOneText, buttonTwoText, buttonThreeText;

    private DialogueData _currentQuestionDialogue;

    private int _messageLength;
    private int _index = 0; //Index to go through the dialogue message arrays from "dialogueData"
    private int _textBranchIndex = -1;

    private bool _doNotRepeat = false; 
    private bool _concludeMoonPuzzle = false;
    private bool _displayingFirstQuestionDialogue = false;
    private bool _hasActivatedButtonOptions = false; 
    private bool _finishedTypingMessage = false; //Prevent or allow going through messages when they're not fully typed out
    private bool _allowGoingThroughMessages = false; //Prevent or allow going through messages entirely

    private const int MAX_COUNTER_AMOUNT_FOR_WRONG_BUTTON_CHOICES = 2;
    private const float TYPING_SPEED = 0.015f;

    void Start()
    {
        EventBus.Instance.Subscribe<StartNewTextAdventure>(StartNewTextAdventure);
        EventBus.Instance.Subscribe<AdvanceTextAdventure>(NextTextAdvetureDialogue);
        EventBus.Instance.Subscribe<RestartTextAdventure>(RestartTextAdventureDialogue);

        ButtonOptions.SetActive(false);
        textAdventureUI.SetActive(false);
        ResetValues();
    }

    void OnEnable()
    {
        _doNotRepeat = false;
    }

    void OnDisable()
    {
        ResetValues();
    }

    private void ResetValues()
    {
        dialogueText.text = "";
        buttonOneText.text = "";
        buttonTwoText.text = "";
        buttonThreeText.text = "";

        _index = 0; 
        _hasActivatedButtonOptions = false;
        _allowGoingThroughMessages = true;

        StopAllCoroutines();

        ButtonOptions.SetActive(false);
        returnButton.SetActive(false);
    }

    void Update()
    {
        if(WrongButtonChoicesCounter >= MAX_COUNTER_AMOUNT_FOR_WRONG_BUTTON_CHOICES && _doNotRepeat == false)
        {
            ResetValues();
            FailedMoonPuzzle();
            _doNotRepeat = true;
        }
    }

    public void DisableButtonOptions() //Caled by "TextAdventureButton" (Dialogue Buttons) when having clicked on incorrect button
    {
        WrongButtonChoicesCounter++;
        returnButton.SetActive(true); //Can make them fade in to be more clean before becoming active
        ButtonOptions.SetActive(false); //Can make them fade out to be more clean before becoming inactive
    }

    private void FailedMoonPuzzle() 
    {
        _concludeMoonPuzzle = true;
        Debug.Log("You lose the entire game!");
        //Send event to show end game screen
    }

    public void FinishTextAdventure() //Caled by "TextAdventureButton" (Dialogue Buttons)
    {
        _concludeMoonPuzzle = true;
    }

    private void RestartTextAdventureDialogue(RestartTextAdventure restartTextAdventure)
    {
        if(WrongButtonChoicesCounter == MAX_COUNTER_AMOUNT_FOR_WRONG_BUTTON_CHOICES)
            return;

        ResetValues();
        StartCoroutine(TypeMessage(_currentQuestionDialogue.Messages[_index]));
    }

    private void StartNewTextAdventure(StartNewTextAdventure startNewTextAdventure) 
    {
        _concludeMoonPuzzle = false;
        WrongButtonChoicesCounter = 0;

        _textBranchIndex++;
        _currentQuestionDialogue = TextAdventureDialogue.TextBranches[_textBranchIndex].FirstQuestionDialogue;
        _messageLength = _currentQuestionDialogue.Messages.Length;

        StopAllCoroutines();
        StartCoroutine(TypeMessage(_currentQuestionDialogue.Messages[_index]));
    }

    private void NextTextAdvetureDialogue(AdvanceTextAdventure advanceTextAdventure) //Called by "PlayerInputController"
    {
        if(_finishedTypingMessage != true || _doNotRepeat == true)
            return;

        if(_index+1 == _messageLength && _concludeMoonPuzzle == false)
            return;

        if(_finishedTypingMessage == true && _allowGoingThroughMessages == true)
        {
            if(_concludeMoonPuzzle == false)
            {
                if(_displayingFirstQuestionDialogue == true)
                {
                    _index++;
                    StopAllCoroutines();
                    StartCoroutine(TypeMessage(TextAdventureDialogue.TextBranches[_textBranchIndex].FirstQuestionDialogue.Messages[_index]));
                }

                else
                {
                    _index++;
                    StopAllCoroutines();
                    StartCoroutine(TypeMessage(_currentQuestionDialogue.Messages[_index]));
                }
            }

            else //No longer show text, stop the text adventure (completed the moon puzzle successfully)
            {
                Debug.Log("CONCLUDED MOON PUZZLE");
                _doNotRepeat = true;
                textAdventureUI.SetActive(false); //Send event to fade textAdventureUI screen and show moon reward UI
            }
        }
    }

    public void PromptDialogueFromButton(DialogueData dialogueData) //Caled by "TextAdventureButton" (Dialogue Buttons)
    {
        //Resetting values
        _index = 0;
        _hasActivatedButtonOptions = false;
        _allowGoingThroughMessages = true;
        ButtonOptions.SetActive(false);

        _currentQuestionDialogue = dialogueData;
        _messageLength = _currentQuestionDialogue.Messages.Length; 

        StopAllCoroutines();
        StartCoroutine(TypeMessage(_currentQuestionDialogue.Messages[_index]));
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

        if(_concludeMoonPuzzle == false && _index+1 == _messageLength && _hasActivatedButtonOptions == false) //Show button display
        {
            ButtonOptions.SetActive(true); //Can make them fade in to be more clean before becoming active

            //Set the button text for all three text adventure buttons 
            buttonOneText.text = _currentQuestionDialogue.ButtonOneText;
            buttonTwoText.text = _currentQuestionDialogue.ButtonTwoText;
            buttonThreeText.text = _currentQuestionDialogue.ButtonThreeText;

            EventBus.Instance.Publish(new SetTextAdventureQuestion(_currentQuestionDialogue, _textBranchIndex));

            _displayingFirstQuestionDialogue = false;
            _hasActivatedButtonOptions = true; //Prevent looping of "if" statement being called
            _allowGoingThroughMessages = false; //Prevent going through dialogue entirely
        }

        _finishedTypingMessage = true;
    }
}
