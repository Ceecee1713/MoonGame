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
    private GameObject moonPuzzleUIPopUp;
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
    private int _index = 0; //Index to go through the dialogue message array (individual messages) from "dialogueData" 
    private int _textBranchIndex = -1;

    private bool _fadeOutCanvas = false;
    private bool _doNotRepeat = false; 
    private bool _concludeMoonPuzzle = false;
    private bool _hasActivatedButtonOptions = false; 
    private bool _finishedTypingMessage = false; //Prevent or allow going through individual messages when they're not fully typed out
    private bool _allowGoingThroughMessages = false; //Prevent or allow going through new dialogue branches entirely

    private const float TIME_TO_WAIT_BEFORE_FADING_MOON_PUZZLE_POP_UP = 2.5f;
    private const int MAX_COUNTER_AMOUNT_FOR_WRONG_BUTTON_CHOICES = 2;
    private const float TYPING_SPEED = 0.015f;

    void Start()
    {
        EventBus.Instance.Subscribe<StartNewTextAdventure>(StartNewTextAdventure);
        EventBus.Instance.Subscribe<AdvanceTextAdventure>(NextTextAdvetureDialogue);

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

    public void DisableButtonOptions() //Caled when having clicked on an incorrect button
    {
        WrongButtonChoicesCounter++;
        returnButton.SetActive(true); 
        ButtonOptions.SetActive(false); 
    }

    private void FailedMoonPuzzle() //Edit
    {
        _concludeMoonPuzzle = true;
        Debug.Log("You lose the entire game!");
        //Send event to show end game screen and disable this UI
    }

    public void FinishTextAdventure() //Caled by "TextAdventureButton" (Dialogue Buttons) 
    {
        _concludeMoonPuzzle = true;
    }

    public void RestartTextAdventureDialogue() //Called by "ReturnTextAdventureButton" (Dialogue Button)  
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

    private void NextTextAdvetureDialogue(AdvanceTextAdventure advanceTextAdventure) //Called by "PlayerInputController" (keybind Enter/left mouse click)
    {
        if(_finishedTypingMessage != true || _doNotRepeat == true)
            return;

        if(_index+1 == _messageLength && _concludeMoonPuzzle == false)
            return;

        if(_finishedTypingMessage == true && _allowGoingThroughMessages == true)
        {
            if(_concludeMoonPuzzle == false)
            {
                _index++;
                StopAllCoroutines();
                StartCoroutine(TypeMessage(_currentQuestionDialogue.Messages[_index]));
            }

            else //No longer show text, stop the text adventure (completed the moon puzzle SUCCESSFULLY)
            {
                _doNotRepeat = true;
                StopAllCoroutines();
                StartCoroutine(ShowMoonPuzzleFragmentUIPopUp());
            }
        }
    }

    public void PromptDialogueFromButton(DialogueData dialogueData) //Caled by "TextAdventureButton" (Dialogue Buttons) when having chosen correct button choice
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

    IEnumerator ShowMoonPuzzleFragmentUIPopUp()
    {
        //Show moon fragment UI Pop Up
        _fadeOutCanvas = false;
        moonPuzzleUIPopUp.SetActive(true);
        EventBus.Instance.Publish(new DisplayMoonFragmentImage(TextAdventureDialogue.TextBranches[_textBranchIndex].MoonFragmentSprite));
        EventBus.Instance.Publish(new FadeSingleCanvas(moonPuzzleUIPopUp, _fadeOutCanvas));

        yield return new WaitForSeconds(TIME_TO_WAIT_BEFORE_FADING_MOON_PUZZLE_POP_UP);

        //No longer moon fragment UI Pop Up and no longer show text adventure UI
        _fadeOutCanvas = true;
        EventBus.Instance.Publish(new FadeSingleCanvas(moonPuzzleUIPopUp, _fadeOutCanvas));
        EventBus.Instance.Publish(new FreezePlayer(false));
        EventBus.Instance.Publish(new FadeSingleCanvas(textAdventureUI, _fadeOutCanvas));

        yield return null;
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
            ButtonOptions.SetActive(true); 
            buttonOneText.text = _currentQuestionDialogue.ButtonOneText;
            buttonTwoText.text = _currentQuestionDialogue.ButtonTwoText;
            buttonThreeText.text = _currentQuestionDialogue.ButtonThreeText;

            EventBus.Instance.Publish(new SetTextAdventureQuestion(_currentQuestionDialogue, _textBranchIndex));

            _hasActivatedButtonOptions = true; //Prevent looping of "if" statement being called
            _allowGoingThroughMessages = false; //Prevent going through dialogue entirely
        }

        _finishedTypingMessage = true;
    }
}
