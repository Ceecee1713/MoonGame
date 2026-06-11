using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Manages the storytelling text adventure as well as publishing events when starting the tutorial dialogue, starting/ending prayer phase (progressing to next day),
/// showing winning screen, and changing the corrioson area speeds of lowering health (or maintaining them at their current speed)
/// </summary>
/// 
/// <remarks>
/// This script controls the storytelling UI dialogue text for the storytelling UI
/// 
/// This script is used for minor storytelling acorss the game
/// as well as progressing to the next day with the prayer phase (this is prompted by player interacting with moon statue)
/// 
/// See <see cref="StorytellingDialogueData"/> for how each individual storytelling dialogue is set up. 
/// See <see cref="CorriosonValues"/> for how the different speed values that can be given to the corrioson zones to drop player's health
/// 
/// This script's way of typing dialogue is the same as "MoonPuzzleDialogueText"
/// Make sure they both type dialogue the same in their IEnumerators as well as the number of "MAX_LINES" is the same across both scripts
/// See <see cref="MoonPuzzleDialogueText"/>
/// ______________________________________________________________________________________________________________________
/// 
/// This script works together with the scripts: "CanvasManager", "PlayerInputController", "PlayerStateMachine", "ItemDrop", "GoalText", "PlayerHealth",
/// "InteractableItem", "ExplorationTimer", "CorriosonZone", "GameManager"
/// 
/// See <see cref="CanvasManager"/> - Listening to "StartPrayerPhase" that "CanvasManager" publishes to begin dialogue for praying at the moon statue, 
/// and publishing "ChangeCanvases" event to swap UI canvases 
/// 
/// See <see cref="PlayerInputController"/> - Listening to "AdvanceThroughTextAdventure" event "PlayerInputController" publishes to advance through dialogue
/// See <see cref="PlayerStateMachine"/> - Publishing "FreezePlayer" event to freeze/unfreeze the player 
/// See <see cref="ItemDrop"/> - Publishing "NewExplorationPhase" event to destroy game object attached "ItemDrop" script
/// See <see cref="GoalText"/> - Publishing "NewExplorationPhase" event to keep the same new goal text on screen
/// See <see cref="PlayerHealth"/> - Publishing "MaintainPlayerHealth" event to maintain player's current health,
/// and publishing "NewExplorationPhase" event to reset player health to full 
/// 
/// See <see cref="InteractableItem"/> - Publishing "ResetWorldItemsActiveness" event to reset the activeness of certain game objects 
/// See <see cref="ExplorationTimer"/> - Publishing "PauseExplorationTimer" to pause exploration timer's countdown, 
/// and publishing "ResetExplorationTimer" to reset exploration timer countdown 
/// 
/// See <see cref="CorriosonZone"/> - Publishing "ChangeCorriosonValue" and "RestoreCorriosonValue" 
/// to change the speed of how fast corrioson zones drop player's health
/// 
/// See <see cref="GameManager"/> - Listening to "StartEndGameDialogue" that "GameManager" publishes to begin end game dialogue for storytelling UI,
/// and publishing "StartBeginnerTutorial" to show the tutorial dialogue on the dialogue canvas layered ontop of main player UI
/// 
/// </remarks>

public class StorytellingDialogueText : MonoBehaviour
{
    [SerializeField]
    private CorriosonValues corriosonValues;

    [SerializeField]
    private float introductionDelay = 0.25f;

    [Header ("Audio")]
    [SerializeField]
    private AudioClip nextMessageSFX;
    [SerializeField]
    private AudioClip clearDialogueSFX;
    [SerializeField]
    private AudioClip backgroundNoise;
    [SerializeField]
    [Range (0,1)]
    private float desiredVolumeForBackgroundNoise;

    [Header ("Main Dialogues")]
    [SerializeField]
    private StorytellingDialogueData startingGameDialogue;
    [SerializeField]
    private StorytellingDialogueData finishedGameDialogue;
    [SerializeField]
    private StorytellingDialogueData prayerPhaseDialogue;

    [Header ("Prayer Dialogues for Prayer Phase")]
    [SerializeField]
    private StorytellingDialogueData badPrayerDialogue;
    [SerializeField]
    private StorytellingDialogueData goodPrayerDialogue;
    [SerializeField]
    private StorytellingDialogueData noPrayerDialogue;

    [Header ("UI Information")]
    [SerializeField]
    private GameObject mainPlayerUI;
    [SerializeField]
    private GameObject winGameUI;
    [SerializeField]
    private GameObject storytellingUI;
    [SerializeField]
    private TextMeshProUGUI dialogueText;

    private StorytellingDialogueData _currentDialogue;

    private int _randomPrayerNumber = 0;
    private int _messageLength;
    private int _index = 0; //Index to go through the dialogue message array (individual messages) from "_currentDialogue" 
    private int _currentLineCount = 0; //For typing out dialogue to mimick paragraph look, dependent on "MAX_LINES"

    private bool _finishedTypingMessage = false; //Prevent or allow going through individual messages when they're not fully typed out
    private bool _stopProgressingThroughDialogue = false;

    private bool _beginPrayerPhase = false; //Prompt choosing a prayer 
    private bool _playerIsInPrayerPhase = false;
    private bool _finishPrayerPhase = false;
    private bool _finishGame = false;
    private bool _startIntroductoryDialogue = false;

    private const int FIRST_MOON_PUZZLE_AREA_NUMBER = 1;
    private const int SECOND_MOON_PUZZLE_AREA_NUMBER = 2;
    private const int THIRD_MOON_PUZZLE_AREA_NUMBER = 3;
    private const int MAX_LINES = 3; //For typing out dialogue to mimick paragraph look

    private const bool START_MOON_PUZZLE = false;
    private const bool START_PRAYER_PHASE = false; 

    private const float TYPING_SPEED = 0.008f;

    void Start()
    {
        EventBus.Instance.Subscribe<StartPrayerPhase>(StartPrayerPhaseAdventure);
        EventBus.Instance.Subscribe<StartEndGameDialogue>(StartEndGameDialogueAdventure);
        EventBus.Instance.Subscribe<AdvanceThroughTextAdventure>(NextDialogue);

        ResetValues();
        Invoke("StartIntroductoryDialogue", introductionDelay);
    }

    void OnEnable()
    {
        _stopProgressingThroughDialogue = false;
    }

    void OnDisable()
    {
        ResetValues();
    }

    private void ResetValues()
    {
        dialogueText.text = "";

        _index = 0; 
        _currentLineCount = 0;
        _playerIsInPrayerPhase = false;
        _beginPrayerPhase = false;
        _finishPrayerPhase = false;

        StopAllCoroutines();
    }

    private void StartIntroductoryDialogue() 
    {
        AudioManager.Instance.SetVolumeForBackgroundNoise(desiredVolumeForBackgroundNoise);
        AudioManager.Instance.PlayBackgroundNoise(backgroundNoise);

        _startIntroductoryDialogue = true;
        _currentDialogue = startingGameDialogue;
        _messageLength = _currentDialogue.Messages.Length;

        EventBus.Instance.Publish(new FreezePlayer(true)); //Publish to "PlayerStateMachine"
        EventBus.Instance.Publish(new MaintainPlayerHealth(true)); //Publish to "PlayerHealth"
        EventBus.Instance.Publish(new PauseExplorationTimer(true)); //Publish to "ExplorationTimer"

        StopAllCoroutines();
        StartCoroutine(TypeMessage(_currentDialogue.Messages[_index].message));
    }

    //"StartEndGameDialogue" is the name of an event. Empty event
    private void StartEndGameDialogueAdventure(StartEndGameDialogue startEndGameDialogue) //Published by "GameManager"
    {
        _finishGame = true;
        _currentDialogue = finishedGameDialogue;
        _messageLength = _currentDialogue.Messages.Length;

        StopAllCoroutines();
        StartCoroutine(TypeMessage(_currentDialogue.Messages[_index].message));
    }

    //"StartPrayerPhase" is the name of an event. Empty event
    private void StartPrayerPhaseAdventure(StartPrayerPhase startPrayerPhase) //Published by "CanvasManager"
    {
        _playerIsInPrayerPhase = true;
        _beginPrayerPhase = true;
        _currentDialogue = prayerPhaseDialogue;
        _messageLength = _currentDialogue.Messages.Length;

        StopAllCoroutines();
        StartCoroutine(TypeMessage(_currentDialogue.Messages[_index].message));
    }

    //Iterating through storytelling dialogue messages. Published by "PlayerInputController"
    //"AdvanceThroughTextAdventure" is the name of an event. Empty event
    private void NextDialogue(AdvanceThroughTextAdventure advanceTextAdventure)
    {
        if(_finishedTypingMessage != true || _stopProgressingThroughDialogue == true)
            return;

        if(_index+1 == _messageLength && _beginPrayerPhase == true)
        {
            PickPrayer();
            return;
        }

        if(_index+1 == _messageLength && _finishGame == true) //Show win game screen
        {
            _stopProgressingThroughDialogue = true;
            EventBus.Instance.Publish(new ChangeCanvases(winGameUI, START_MOON_PUZZLE, START_PRAYER_PHASE)); //Publish to "CanvasManager"
            return;
        }

        if(_index+1 == _messageLength && _finishPrayerPhase == true) //Return to Exploration Phase with Main Player UI
        {
            _stopProgressingThroughDialogue = true;
            EventBus.Instance.Publish(new FreezePlayer(false)); //Publish to "PlayerStateMachine"
            EventBus.Instance.Publish(new ChangeCanvases(mainPlayerUI, START_MOON_PUZZLE, START_PRAYER_PHASE)); //Publish to "CanvasManager"
            EventBus.Instance.Publish(new NewExplorationPhase()); //Publish to "PlayerStateMachine", "ItemDrop", "GoalText", "PlayerHealth"
            EventBus.Instance.Publish(new ResetWorldItemsActiveness()); //Publish to "InteractableItem"
            EventBus.Instance.Publish(new ResetExplorationTimer()); //Publish to "ExplorationTimer"
            return;
        }

        if(_index+1 == _messageLength && _startIntroductoryDialogue == true) //Show beginning tutorial for the start of the game
        {
            _stopProgressingThroughDialogue = true;
            EventBus.Instance.Publish(new ChangeCanvases(mainPlayerUI, START_MOON_PUZZLE, START_PRAYER_PHASE)); //Publish to "CanvasManager"
            EventBus.Instance.Publish(new StartBeginnerTutorial()); //Publish to "GameManager"
            return;
        }

        if(_index+1 == _messageLength)
            return;

        AudioManager.Instance.PlaySoundEffect(nextMessageSFX);

        _index++;
        StopAllCoroutines();
        StartCoroutine(TypeMessage(_currentDialogue.Messages[_index].message));
    }

    private void PickPrayer() 
    {
        _index = 0; //Reset

        _randomPrayerNumber = Random.Range(1, 3);

        if(_randomPrayerNumber == 1)
        {
            _currentDialogue = badPrayerDialogue;

            //Make corrioson accumalate faster (lower player health faster). All publish to "CorriosonZone"
            EventBus.Instance.Publish(new ChangeCorriosonValue(corriosonValues.SpeedToLowerHealthForBadPrayerEffect, FIRST_MOON_PUZZLE_AREA_NUMBER)); 
            EventBus.Instance.Publish(new ChangeCorriosonValue(corriosonValues.SpeedToLowerHealthForBadPrayerEffect, SECOND_MOON_PUZZLE_AREA_NUMBER));
            EventBus.Instance.Publish(new ChangeCorriosonValue(corriosonValues.SpeedToLowerHealthForBadPrayerEffect, THIRD_MOON_PUZZLE_AREA_NUMBER));
        }
            
        else if(_randomPrayerNumber == 2)
        {
            _currentDialogue = goodPrayerDialogue;

            //Make corrioson accumalate slower (lower player health slower). All publish to "CorriosonZone"
            EventBus.Instance.Publish(new ChangeCorriosonValue(corriosonValues.SpeedToLowerHealthForGoodPrayerEffect, FIRST_MOON_PUZZLE_AREA_NUMBER));
            EventBus.Instance.Publish(new ChangeCorriosonValue(corriosonValues.SpeedToLowerHealthForGoodPrayerEffect, SECOND_MOON_PUZZLE_AREA_NUMBER));
            EventBus.Instance.Publish(new ChangeCorriosonValue(corriosonValues.SpeedToLowerHealthForGoodPrayerEffect, THIRD_MOON_PUZZLE_AREA_NUMBER));
        }
            

        else if(_randomPrayerNumber == 3)
        {
            _currentDialogue = noPrayerDialogue;
            EventBus.Instance.Publish(new RestoreCorriosonValue()); //Publish to "CorriosonZone"
        }
            
        AudioManager.Instance.PlaySoundEffect(nextMessageSFX); 

        _beginPrayerPhase = false;
        _finishPrayerPhase = true;
        _messageLength = _currentDialogue.Messages.Length; 

        StopAllCoroutines();
        StartCoroutine(TypeMessage(_currentDialogue.Messages[_index].message));
    }

    private IEnumerator TypeMessage(string message) 
    {
        _finishedTypingMessage = false;
    
        if (_currentLineCount >= MAX_LINES) //Clear text when we've reached max lines
        {
            if(_playerIsInPrayerPhase == false)
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
        
        _currentLineCount++; 
        _finishedTypingMessage = true;
    }
}
