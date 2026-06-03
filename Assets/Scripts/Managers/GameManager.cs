using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the publishing of multiple events as well as setting UI canvases active for the related events. 
/// In addition, responsible for keeping track of how many puzzle areas have been completed.
/// </summary>
/// 
/// <remarks>
/// This script has one variable that's accessible by scripts that have GameManager as a Serializable field: "AreaChangesCount", 
/// though the value cannot be set or altered by these scripts. Only get. Only "DecipherClueButton" gets this variable.
/// MAKE SURE FOR EACH OF THOSE REFERENCING SCRIPTS that the comparison of values matches with the maximum value of "AreaChangesCount" here.
/// See <see cref="DecipherClueButton"/> for any oddities.
///
/// No other script should be keeping track of how many puzzle areas have been completed.
/// </remarks>

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private StorytellingDialogueData beginningTutorialDialogue;

    [Header ("UI Information")]
    [SerializeField]
    private GameObject blackScreenUI;
    [SerializeField]
    private GameObject storytellingUI;
    [SerializeField]
    private GameObject dialogueCanvas; //Dialogue Canvas that's layered on top of the Main Player UI

    [Header ("Seconds Time Delays")]
    [SerializeField]
    private float timeDelayBeforeShowingBeginningTutorial = 2.0f; 
    [SerializeField]
    private float timeDelayBeforeShowingEndGameDialogue = 2.0f;

    private float timeDelayBeforeStartingEndGameDialogue; 
    
    [SerializeField, HideInInspector]
    private int _areaChangesCount = 0;

    public int AreaChangesCount //For deciphering a clue at the crafting table
    {
        get => _areaChangesCount;
        private set => _areaChangesCount = Mathf.Clamp(value, 0, MAX_NUMBER_OF_AREA_CHANGES);
    }

    private void OnValidate()
    {
        _areaChangesCount = Mathf.Clamp(_areaChangesCount, 0, MAX_NUMBER_OF_AREA_CHANGES);
    }

    private const float TIME_DELAY_ACCOUNTING_FOR_FADING_CANVASES = 1.0f; //Account for fading screen time when changing canvases

    private const int MAX_NUMBER_OF_AREA_CHANGES = 2; //Number of puzzle areas to be completed, EXCLUDING the last puzzle. (total number of puzzle areas - 1)

    private const bool STARTING_THE_GAME = true; 

    private const bool START_MOON_PUZZLE = false;
    private const bool START_PRAYER_PHASE = false; 

    void Start()
    {
        EventBus.Instance.Subscribe<CompletedAllMoonPuzzles>(CompletedMoonPuzzles);
        EventBus.Instance.Subscribe<NewMoonFragmentObtained>(MaterialsAndStreetlightChange);
        EventBus.Instance.Subscribe<StartBeginnerTutorial>(StartBeginningTutorial);

        timeDelayBeforeStartingEndGameDialogue = timeDelayBeforeShowingEndGameDialogue - TIME_DELAY_ACCOUNTING_FOR_FADING_CANVASES;
    }

    //Illuminate street lights and change the materials of decorative moon statues 
    private void MaterialsAndStreetlightChange(NewMoonFragmentObtained newMoonFragmentObtained) //Published by "MoonPuzzleDialogueText"
    {
        AreaChangesCount++;
        EventBus.Instance.Publish(new NewAreaChange(AreaChangesCount)); //Publish to "LightPropMoon" and "StreetLamps"
        
        if(AreaChangesCount == MAX_NUMBER_OF_AREA_CHANGES)
            EventBus.Instance.Publish(new ChangeThirdCorriosonAreaValue()); //Publish to "CorriosonZone" 
    }

    private void StartBeginningTutorial(StartBeginnerTutorial startBeginnerTutorial) //Published by "StorytellingDialogueText"
    {
        StopAllCoroutines();
        StartCoroutine(ShowBeginningTutorial());
    }

    //Called BEFORE moon text adventure UI has been disabled
    private void CompletedMoonPuzzles(CompletedAllMoonPuzzles completedAllMoonPuzzles) //Published by "MoonPuzzleDialogueText"
    {
        Invoke("PromptEndGameDialogue", timeDelayBeforeShowingEndGameDialogue);
    }

    private void PromptEndGameDialogue()
    {
        StopAllCoroutines();
        StartCoroutine(ShowEndGameDialogue());
    }

    //Fade the screen into the storytelling UI and start end game dialogue on the UI
    private IEnumerator ShowEndGameDialogue()
    {
        EventBus.Instance.Publish(new ChangeCanvases(storytellingUI, START_MOON_PUZZLE, START_PRAYER_PHASE));
        yield return new WaitForSeconds(timeDelayBeforeStartingEndGameDialogue);
        blackScreenUI.SetActive(true);
        EventBus.Instance.Publish(new StartEndGameDialogue()); //Publish to "StorytellingDialogueText"
        EventBus.Instance.Publish(new StopMoonStatueSpin()); //Publish to "MoonVisibility"
        yield return null;
    }

    //Show the tutorial on the dialogue canvas layered on top of the Main Player UI to run through player's objective
    private IEnumerator ShowBeginningTutorial()
    {
        yield return new WaitForSeconds(timeDelayBeforeShowingBeginningTutorial);
        dialogueCanvas.SetActive(true);
        EventBus.Instance.Publish(new TypeDialogueOnMainUI(beginningTutorialDialogue, false, STARTING_THE_GAME)); //Publish to "DialogueCanvas"
        yield return null;
    }
}
