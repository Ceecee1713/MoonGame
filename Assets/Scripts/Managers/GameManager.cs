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
    private float timeDelayBeforeShowingEndGameDialogue = 2.0f;
    [SerializeField]
    private float timeDelayBeforeShowingBeginningTutorial = 2.0f; 

    [HideInInspector]
    public int AreaChangesCount = 0; 

    private const int MAX_NUMBER_OF_AREA_CHANGES = 2; 

    private const float TIME_TO_WAIT_FOR_FADING_CANVASES = 1.5f;

    private const bool STARTING_THE_GAME = true; 

    private const bool START_MOON_PUZZLE = false;
    private const bool START_PRAYER_PHASE = false; 

    void Start()
    {
        EventBus.Instance.Subscribe<CompletedAllMoonPuzzles>(CompletedMoonPuzzle);
        EventBus.Instance.Subscribe<NewMoonFragmentObtained>(MaterialsAndStreetlightChange);
        EventBus.Instance.Subscribe<StartBeginnerTutorial>(StartBeginningTutorial);
    }

    void Update()
    {
        Mathf.Clamp(AreaChangesCount, 0, MAX_NUMBER_OF_AREA_CHANGES);
    }

    //Changes materials for deciphering a clue at crafting table and turn on street lamp lights
    private void MaterialsAndStreetlightChange(NewMoonFragmentObtained newMoonFragmentObtained)
    {
        AreaChangesCount++;
    }

    private void StartBeginningTutorial(StartBeginnerTutorial startBeginnerTutorial) //Published by StorytellingDialogueText
    {
        StopAllCoroutines();
        StartCoroutine(ShowBeginningTutorial());
    }

    //Called BEFORE moon text adventure UI has been disabled, keep in mind
    private void CompletedMoonPuzzle(CompletedAllMoonPuzzles completedAllMoonPuzzles)
    {
        //Special particle effects or extra things
        Invoke("PromptEndGameDialogue", timeDelayBeforeShowingEndGameDialogue);
    }

    IEnumerator ShowEndGameDialogue()
    {
        EventBus.Instance.Publish(new ChangeCanvases(storytellingUI, START_MOON_PUZZLE, START_PRAYER_PHASE));
        yield return new WaitForSeconds(TIME_TO_WAIT_FOR_FADING_CANVASES);
        blackScreenUI.SetActive(true);
        EventBus.Instance.Publish(new StartEndGameDialogue());
        yield return null;
    }

    IEnumerator ShowBeginningTutorial()
    {
        yield return new WaitForSeconds(timeDelayBeforeShowingBeginningTutorial);
        dialogueCanvas.SetActive(true);
        EventBus.Instance.Publish(new TypeDialogueOnMainUI(beginningTutorialDialogue, false, STARTING_THE_GAME));
        yield return null;
    }

    private void PromptEndGameDialogue()
    {
        StopAllCoroutines();
        StartCoroutine(ShowEndGameDialogue());
    }
}
