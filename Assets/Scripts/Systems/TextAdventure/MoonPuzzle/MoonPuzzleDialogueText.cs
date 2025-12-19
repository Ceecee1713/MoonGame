using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoonPuzzleDialogueText : MonoBehaviour
{
    [SerializeField]
    private CorriosonValues corriosonValues;

    public MoonPuzzleTextAdventure TextAdventureDialogue;

    public int WrongButtonChoicesCounter = 0;

    [Header ("Other UI Information")]
    [SerializeField]
    private GameObject blackScreenUI;
    [SerializeField]
    private GameObject failedGameUI;
    [SerializeField]
    private GameObject mainPlayerUI;
    [SerializeField]
    private GameObject moonPuzzleUIPopUp;

    [Header ("This UI's Information")]
    [SerializeField]
    private GameObject textAdventureUI;
    [SerializeField]
    private TextMeshProUGUI dialogueText;
    [SerializeField]
    private Image heartImage;
    [SerializeField]
    private Sprite emptyHeartSprite;

    [Header ("Button Displays")]
    [SerializeField]
    private GameObject returnButton;
    [SerializeField]
    private GameObject ButtonOptions;
    [SerializeField]
    private TextMeshProUGUI buttonOneText, buttonTwoText, buttonThreeText;

    private MoonPuzzleDialogueData _currentQuestionDialogue;

    private Sprite _fullHeartSprite;

    private int _messageLength;
    private int _index = 0; //Index to go through the dialogue message array (individual messages) from "dialogueData" 
    private int _textBranchIndex = -1;
    private int _currentLineCount = 0;
    private int _moonAreaCounter = 0;
    private int _completedMoonPuzzlesCounter = 0;

    private bool _fadeOutCanvas = false;
    private bool _failedMoonPuzzle = false;
    private bool _doNotRepeat = false; 
    private bool _concludeMoonPuzzle = false;
    private bool _hasActivatedButtonOptions = false; 
    private bool _finishedTypingMessage = false; //Prevent or allow going through individual messages when they're not fully typed out
    private bool _allowGoingThroughMessages = false; //Prevent or allow going through new dialogue branches entirely

    private const bool START_MOON_PUZZLE = false;
    private const bool START_PRAYER_PHASE = false; 

    private const int TOTAL_NUMBER_OF_MOON_PUZZLES = 3;
    private const int MAX_COUNTER_AMOUNT_FOR_WRONG_BUTTON_CHOICES = 2;
    private const int MAX_LINES = 3; 

    private const float TIME_TO_WAIT_FOR_FADING_CANVASES = 1.5f;
    private const float TYPING_SPEED = 0.015f;

    void Start()
    {
        EventBus.Instance.Subscribe<StartNewTextAdventure>(StartNewTextAdventure);
        EventBus.Instance.Subscribe<AdvanceTextAdventure>(NextTextAdvetureDialogue);

        _fullHeartSprite = heartImage.sprite;

        ResetValues();
        ButtonOptions.SetActive(false);
        textAdventureUI.SetActive(false);
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

        heartImage.sprite = _fullHeartSprite;

        _index = 0; 
        _currentLineCount = 0;
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

    private void FailedMoonPuzzle()
    {
        _concludeMoonPuzzle = true;
        _failedMoonPuzzle = true;
        blackScreenUI.SetActive(true);
        EventBus.Instance.Publish(new ChangeCanvases(failedGameUI, START_MOON_PUZZLE, START_PRAYER_PHASE));
    }

    public void DisableButtonOptions() //Caled when having clicked on an incorrect button
    {
        WrongButtonChoicesCounter++;
        returnButton.SetActive(true); 
        ButtonOptions.SetActive(false); 
    }

    public void FinishTextAdventure() //Caled by "TextAdventureButton" (Dialogue Buttons) 
    {
        _concludeMoonPuzzle = true;
    }

    //Called when you choose a wrong button to advance further into moon puzzle 
    public void RestartTextAdventureDialogue() //Called by "ReturnTextAdventureButton" (Dialogue Button)  
    {
        if(WrongButtonChoicesCounter == MAX_COUNTER_AMOUNT_FOR_WRONG_BUTTON_CHOICES)
            return;

        ResetValues();
        heartImage.sprite = emptyHeartSprite;
        dialogueText.text = "";
        _currentLineCount = 0;
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
        if(_finishedTypingMessage != true || _doNotRepeat == true || _failedMoonPuzzle == true)
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

            else //Stop the text adventure, lower corrioson value for an area (completed the moon puzzle SUCCESSFULLY)
            {
                _doNotRepeat = true;
                _moonAreaCounter++;
                _completedMoonPuzzlesCounter++;
                EventBus.Instance.Publish(new ChangeCorriosonValue(corriosonValues.SpeedToLowerHealthWhenAreaIsCleared, _moonAreaCounter));
                
                StopAllCoroutines();
                StartCoroutine(ShowMoonPuzzleFragmentUIPopUp());
            }
        }
    }

    public void PromptDialogueFromButton(MoonPuzzleDialogueData dialogueData) //Caled by "TextAdventureButton" (Dialogue Buttons) when having chosen correct button choice
    {
        //Resetting values
        _index = 0;
        dialogueText.text = "";
        _currentLineCount = 0;
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
        //Show moon fragment UI Pop Up and start a new exploration phase
        _fadeOutCanvas = false;
        moonPuzzleUIPopUp.SetActive(true);
        EventBus.Instance.Publish(new NewExplorationPhase());
        EventBus.Instance.Publish(new ResetWorldItems());
        EventBus.Instance.Publish(new PauseExplorationTimer(true));
        EventBus.Instance.Publish(new NewMoonFragmentObtained()); //Show a dialogue message from moon statue
        EventBus.Instance.Publish(new DisplayMoonFragmentImage(TextAdventureDialogue.TextBranches[_textBranchIndex].MoonFragmentSprite));
        EventBus.Instance.Publish(new FadeSingleCanvas(moonPuzzleUIPopUp, _fadeOutCanvas));

        yield return new WaitForSeconds(TIME_TO_WAIT_FOR_FADING_CANVASES);

        //No longer moon fragment UI Pop Up, no longer show text adventure UI, show main player UI
        _fadeOutCanvas = true;
        EventBus.Instance.Publish(new FadeSingleCanvas(moonPuzzleUIPopUp, _fadeOutCanvas));

        if(_completedMoonPuzzlesCounter == TOTAL_NUMBER_OF_MOON_PUZZLES)
        {
            EventBus.Instance.Publish(new FadeSingleCanvas(textAdventureUI, _fadeOutCanvas));
            EventBus.Instance.Publish(new CompletedAllMoonPuzzles());
        }

        else
            EventBus.Instance.Publish(new ChangeCanvases(mainPlayerUI, START_MOON_PUZZLE, START_PRAYER_PHASE));
            
        yield return null;
    }

    IEnumerator TypeMessage(string message) 
    {
        _finishedTypingMessage = false;
    
        if (_currentLineCount >= MAX_LINES) //Clear text when we've reached max lines
        {
            dialogueText.text = "";
            _currentLineCount = 0;
        }
        
        if (_currentLineCount > 0) //Adding empty lines to mimick a paragraph look
        {
            dialogueText.text += "\n";
            dialogueText.text += "\n";
        }
        
        foreach (char letter in message.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(TYPING_SPEED);
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

        _currentLineCount++; 
        _finishedTypingMessage = true;
    }
}
