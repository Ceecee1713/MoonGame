using System;
using System.Collections;
using UnityEngine;

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
    private GameObject dialogueCanvas;

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
        set => _areaChangesCount = Mathf.Clamp(value, 0, MAX_NUMBER_OF_AREA_CHANGES);
    }

    private void OnValidate()
    {
        _areaChangesCount = Mathf.Clamp(_areaChangesCount, 0, MAX_NUMBER_OF_AREA_CHANGES);
    }

    private const float TIME_DELAY_ACCOUNTING_FOR_FADING_CANVASES = 1.0f; //Account for fading screen time when changing canvases

    private const int MAX_NUMBER_OF_AREA_CHANGES = 2; 

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

    private void MaterialsAndStreetlightChange(NewMoonFragmentObtained newMoonFragmentObtained)
    {
        AreaChangesCount++;
        EventBus.Instance.Publish(new NewAreaChange(AreaChangesCount)); //Publish to LightPropMoon and StreetLamps
        
        if(AreaChangesCount == MAX_NUMBER_OF_AREA_CHANGES)
            EventBus.Instance.Publish(new ChangeThirdCorriosonAreaValue());
    }

    private void StartBeginningTutorial(StartBeginnerTutorial startBeginnerTutorial) //Published by StorytellingDialogueText
    {
        StopAllCoroutines();
        StartCoroutine(ShowBeginningTutorial());
    }

    //Called BEFORE moon text adventure UI has been disabled, keep in mind
    private void CompletedMoonPuzzles(CompletedAllMoonPuzzles completedAllMoonPuzzles)
    {
        Invoke("PromptEndGameDialogue", timeDelayBeforeShowingEndGameDialogue);
    }

    private void PromptEndGameDialogue()
    {
        StopAllCoroutines();
        StartCoroutine(ShowEndGameDialogue());
    }

    IEnumerator ShowEndGameDialogue()
    {
        EventBus.Instance.Publish(new ChangeCanvases(storytellingUI, START_MOON_PUZZLE, START_PRAYER_PHASE));
        yield return new WaitForSeconds(timeDelayBeforeStartingEndGameDialogue);
        blackScreenUI.SetActive(true);
        EventBus.Instance.Publish(new StartEndGameDialogue());
        EventBus.Instance.Publish(new StopMoonStatueSpin()); //Make moon statue stop spinning and particle effect
        yield return null;
    }

    IEnumerator ShowBeginningTutorial()
    {
        yield return new WaitForSeconds(timeDelayBeforeShowingBeginningTutorial);
        dialogueCanvas.SetActive(true);
        EventBus.Instance.Publish(new TypeDialogueOnMainUI(beginningTutorialDialogue, false, STARTING_THE_GAME));
        yield return null;
    }
}
