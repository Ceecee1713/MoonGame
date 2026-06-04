using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the moon puzzle text adventure as well as publishing events if the player succeeds a moon puzzle, all three moon puzzles,
/// or fails one of the puzzles and promopts a game over  
/// </summary>
/// 
/// <remarks>
/// This script controls the moon puzzle dialogue text for the text adventure UI. It controls all handling of all moon puzzles.
/// 
/// The player can fail to answer one of three questions from each moon puzzle. If they fail to answer at any time during any of the moon puzzles
/// twice, it's game over. It uses a counter to count the amount of failed attempts. In addition, this script utilizes a small health system
/// keeping score of the player's attempts with heart images
/// 
/// See <see cref="MoonPuzzleTextAdventure"/> for how the collection of multiple text adventure BRANCHES are set up.
/// See <see cref="MoonPuzzleDialogueData"/> for how each individual moon puzzle QUESTION branch is set up. 
/// 
/// This script's way of typing dialogue is the same as "StorytellingDialogueText"
/// Make sure they both type dialogue the same in their IEnumerators as well as the number of "MAX_LINES" is the same across both scripts
/// 
/// ______________________________________________________________________________________________________________________
/// 
/// This script works together with the scripts: "CanvasManager", "PlayerInputController", "PlayerStateMachine", "ItemDrop", "GoalText", "PlayerHealth",
/// "InteractableItem", "ExplorationTimer", "Npc", "SafeZone", "FirstSafeZone", "CorriosonZone", "MoonVisibility", "GameManager", "MoonTextAdventureButton"
/// 
/// See <see cref="CanvasManager"/> - Swapping UI canvases in and out and fading a single UI canvas
/// See <see cref="PlayerInputController"/> - listening to "AdvanceThroughTextAdventure" event that "PlayerInputController" published
/// See <see cref="PlayerStateMachine"/> - freezing the player 
/// See <see cref="ItemDrop"/> - Destroying game object attached to the script
/// See <see cref="GoalText"/> - Setting a new goal text on screen and/or keeping the same new goal text on screen
/// See <see cref="PlayerHealth"/> - Reset player health to full
/// See <see cref="InteractableItem"/> - Resetting the activeness of certain game objects 
/// See <see cref="ExplorationTimer"/> - Pausing exploration timer's countdown
/// See <see cref="Npc"/> - Destroying game object attached based on conditions
/// See <see cref="SafeZone"/> - Destroying game object attached based on conditions
/// See <see cref="FirstSafeZone"/> - Destroying game object attached based on conditions
/// See <see cref="CorriosonZone"/> - Destroying game object attached based on conditions and/or change corrioson zones
/// 
/// See <see cref="MoonVisibility"/> - Light up a moon fragment of moon statue, spin the moon of the moon statue and/or display dialogue
/// on dialogue UI layered ontop of main player UI
/// 
/// See <see cref="GameManager"/> - Show end game dialogue, change lights on decorative moon statues, illuminate streetlights and/or
/// set third corrioson zone's values
/// 
/// See <see cref="MoonTextAdventureButton"/> - Setting the next question dialogue to be said 
/// after all the current question's dialogue has been said and allowing access to public variables and methods
/// 
/// </remarks>

public class MoonPuzzleDialogueText : MonoBehaviour
{
    /// <summary> Collection of all the text adventure branches </summary>
    public MoonPuzzleTextAdventure TextAdventureDialogue;

    /// <summary> Amount of times player fails to answer a question correctly in moon puzzle </summary>
    public int WrongButtonChoicesCounter = 0;

    [Header ("Audio")]
    [SerializeField]
    private AudioClip nextMessageSFX;
    [SerializeField]
    private AudioClip clearDialogueSFX;

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
    private TextMeshProUGUI dialogueText; //Text that'll write out all dialogue of each moon puzzle's questions
    [SerializeField]
    private Image heartImage; //Player's health for text adventure
    [SerializeField]
    private Sprite emptyHeartSprite; //Swapping sprite for the "heartImage" when a failed attempt is made 

    [Header ("Button Displays")]
    [SerializeField]
    private GameObject tryAgainButton; //Button that appears when failed attempt is made to restart moon puzzle question
    [SerializeField]
    private GameObject ButtonOptions; //Parent object that displays all three choice buttons for the moon puzzle question
    [SerializeField]
    private TextMeshProUGUI buttonOneText, buttonTwoText, buttonThreeText; //Text of all three choice buttons

    private MoonPuzzleDialogueData _currentQuestionDialogue; //Moon puzzle dialogue for each individual moon puzzle QUESTION

    private Sprite _fullHeartSprite;

    private int _messageLength; //Length of dialogue from the dialogue message array (individual messages) from "_currentQuestionDialogue" 
    private int _index = 0; //Index to go through the dialogue message array (individual messages) from "_currentQuestionDialogue" 
    private int _textBranchIndex = -1; //Iterates through each moon puzzle branch's dialogue (branches meaning one complete moon puzzle area)
    private int _currentLineCount = 0; //For typing out dialogue to mimick paragraph look, dependent on "MAX_LINES"
    private int _completedMoonPuzzlesCounter = 0;

    private bool _fadeOutCanvas = false; //Used for swapping canvases with the "CanvasManager"
    private bool _failedMoonPuzzle = false;
    private bool _doNotRepeat = false; //Prevent or allow resetting moon puzzle values and show losing game screen
    private bool _concludeMoonPuzzle = false;
    private bool _hasActivatedButtonOptions = false; //Flag if parent object that displays all three choice buttons for moon puzzle question is active or not
    private bool _finishedTypingMessage = false; //Prevent or allow going through individual messages when they're not fully typed out
    private bool _allowGoingThroughMessages = false; //Prevent or allow going through new moon puzzle question dialogues 

    private const bool START_MOON_PUZZLE = false;
    private const bool START_PRAYER_PHASE = false; 

    private const int TOTAL_NUMBER_OF_MOON_PUZZLES = 3;
    private const int MAX_COUNTER_AMOUNT_FOR_WRONG_BUTTON_CHOICES = 2;
    private const int MAX_LINES = 3; //For typing out dialogue to mimick paragraph look

    private const float TIME_TO_WAIT_FOR_FADING_CANVASES = 1.5f;
    private const float TYPING_SPEED = 0.01f;

    void Start()
    {
        EventBus.Instance.Subscribe<StartNewTextAdventure>(StartTextAdventure);
        EventBus.Instance.Subscribe<AdvanceThroughTextAdventure>(NextTextAdvetureDialogue);

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
        tryAgainButton.SetActive(false);
    }

    void Update()
    {
        if(WrongButtonChoicesCounter >= MAX_COUNTER_AMOUNT_FOR_WRONG_BUTTON_CHOICES && _doNotRepeat == false) //Game Over
        {
            ResetValues();
            FailedMoonPuzzle();
            _doNotRepeat = true;
        }
    }

    private void FailedMoonPuzzle() //Show game over screen
    {
        _concludeMoonPuzzle = true;
        _failedMoonPuzzle = true;
        blackScreenUI.SetActive(true);
        EventBus.Instance.Publish(new ChangeCanvases(failedGameUI, START_MOON_PUZZLE, START_PRAYER_PHASE)); //Publish to "CanvasManager"
    }

    public void DisableButtonOptions() //Called by "MoonTextAdventureButton"
    {
        WrongButtonChoicesCounter++;
        tryAgainButton.SetActive(true); 
        ButtonOptions.SetActive(false); 
    }

    public void FinishTextAdventure() //Called by "MoonTextAdventureButton"
    {
        _concludeMoonPuzzle = true;
    }

    public void RestartTextAdventureDialogue() //Called by "ReturnMoonTextAdventureButton"
    {
        if(WrongButtonChoicesCounter == MAX_COUNTER_AMOUNT_FOR_WRONG_BUTTON_CHOICES)
            return;

        ResetValues();
        heartImage.sprite = emptyHeartSprite;
        dialogueText.text = "";
        _currentLineCount = 0;
        StartCoroutine(TypeMessage(_currentQuestionDialogue.Messages[_index]));
    }

    //Choose correct moon puzzle branch (correct moon puzzle dialogue), assign first moon puzzle dialogue question and prompt to type out question
    private void StartTextAdventure(StartNewTextAdventure startNewTextAdventure) //Published by "CanvasManager"
    {
        _concludeMoonPuzzle = false;
        WrongButtonChoicesCounter = 0;

        _textBranchIndex++;
        _currentQuestionDialogue = TextAdventureDialogue.TextBranches[_textBranchIndex].FirstQuestionDialogue;
        _messageLength = _currentQuestionDialogue.Messages.Length;

        StopAllCoroutines();
        StartCoroutine(TypeMessage(_currentQuestionDialogue.Messages[_index]));
    }

    //Iterating through moon puzzle's branch's dialogue messages. Published by "PlayerInputController"
    private void NextTextAdvetureDialogue(AdvanceThroughTextAdventure advanceThroughTextAdventure) 
    {
        if(_finishedTypingMessage != true || _doNotRepeat == true || _failedMoonPuzzle == true)
            return;

        if(_index+1 == _messageLength && _concludeMoonPuzzle == false)
            return;

        if(_finishedTypingMessage == true && _allowGoingThroughMessages == true)
        {
            if(_concludeMoonPuzzle == false)
            {
                AudioManager.Instance.PlaySoundEffect(nextMessageSFX);

                _index++;
                StopAllCoroutines();
                StartCoroutine(TypeMessage(_currentQuestionDialogue.Messages[_index]));
            }

            else //Stop the text adventure (completed A moon puzzle SUCCESSFULLY)
            {
                _doNotRepeat = true;
                _completedMoonPuzzlesCounter++;
                StopAllCoroutines();
                StartCoroutine(ShowMoonPuzzleFragmentUIPopUp());
            }
        }
    }

    //After choosing correct button choice: Prompt to type out new question, called by "MoonTextAdventureButton"
    public void PromptDialogueFromButton(MoonPuzzleDialogueData dialogueData) 
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

        AudioManager.Instance.PlaySoundEffect(clearDialogueSFX);
        
        StopAllCoroutines();
        StartCoroutine(TypeMessage(_currentQuestionDialogue.Messages[_index]));
    }

    private IEnumerator ShowMoonPuzzleFragmentUIPopUp()
    {
        _fadeOutCanvas = false;
        moonPuzzleUIPopUp.SetActive(true);
        EventBus.Instance.Publish(new NewExplorationPhase()); //Publish to "PlayerStateMachine", "ItemDrop", "GoalText", "PlayerHealth"
        EventBus.Instance.Publish(new ResetWorldItemsActiveness()); //Publish to "InteractableItem"
        EventBus.Instance.Publish(new PauseExplorationTimer(true)); //Publish to "ExplorationTimer"
        EventBus.Instance.Publish(new NewMoonFragmentObtained()); //Publish to "NPC", "SafeZone", "FirstSafeZone", "CorriosonZone", "MoonVisibility", "GoalText"
        EventBus.Instance.Publish(new FadeSingleCanvas(moonPuzzleUIPopUp, _fadeOutCanvas)); //Publish to "CanvasManager"

        if(_completedMoonPuzzlesCounter == TOTAL_NUMBER_OF_MOON_PUZZLES)
            EventBus.Instance.Publish(new MakeMoonStatueSpin()); //Publish to "MoonVisibility"

        yield return new WaitForSeconds(TIME_TO_WAIT_FOR_FADING_CANVASES);

        //No longer moon fragment UI Pop Up, no longer show text adventure UI, show main player UI
        _fadeOutCanvas = true;
        EventBus.Instance.Publish(new FadeSingleCanvas(moonPuzzleUIPopUp, _fadeOutCanvas)); //Publish to "CanvasManager"

        if(_completedMoonPuzzlesCounter == TOTAL_NUMBER_OF_MOON_PUZZLES)
        {
            EventBus.Instance.Publish(new FadeSingleCanvas(textAdventureUI, _fadeOutCanvas)); //Publish to "CanvasManager"
            EventBus.Instance.Publish(new CompletedAllMoonPuzzles()); //Publish to "GameManager"
        }

        else
            EventBus.Instance.Publish(new ChangeCanvases(mainPlayerUI, START_MOON_PUZZLE, START_PRAYER_PHASE)); //Publish to "CanvasManager"
            
        yield return null;
    }

    private IEnumerator TypeMessage(string message) 
    {
        _finishedTypingMessage = false;
    
        if (_currentLineCount >= MAX_LINES) //Clear text when we've reached max lines
        {
            AudioManager.Instance.PlaySoundEffect(clearDialogueSFX);
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

            EventBus.Instance.Publish(new SetMoonPuzzleQuestions(_currentQuestionDialogue, _textBranchIndex)); //Publish to "MoonTextAdventureButton"

            _hasActivatedButtonOptions = true; //Prevent looping of "if" statement being called
            _allowGoingThroughMessages = false; //Prevent going through dialogue entirely
        }

        _currentLineCount++; 
        _finishedTypingMessage = true;
    }
}
