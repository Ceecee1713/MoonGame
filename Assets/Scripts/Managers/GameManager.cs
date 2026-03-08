using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header ("UI Information")]
    [SerializeField]
    private GameObject blackScreenUI;
    [SerializeField]
    private GameObject storytellingUI;

    [SerializeField]
    private float timeDelayBeforeShowingEndGameDialogue = 2.0f;

    [HideInInspector]
    public int areaChangesCount = 0; //Accessed by "DecipherClueButton" and "StreetLamps" script

    private const int MAX_NUMBER_OF_AREA_CHANGES = 2; 

    private const float TIME_TO_WAIT_FOR_FADING_CANVASES = 1.5f;

    private const bool START_MOON_PUZZLE = false;
    private const bool START_PRAYER_PHASE = false; 

    void Start()
    {
        EventBus.Instance.Subscribe<CompletedAllMoonPuzzles>(CompletedMoonPuzzle);
        EventBus.Instance.Subscribe<NewMoonFragmentObtained>(ChangeMaterialsForDecipheringClues);
    }

    void Update()
    {
        Mathf.Clamp(areaChangesCount, 0, MAX_NUMBER_OF_AREA_CHANGES);

        if (Input.GetKeyDown(KeyCode.Space))
            areaChangesCount++;
    }

    //Changes materials for deciphering clues in the crafting table UI
    private void ChangeMaterialsForDecipheringClues(NewMoonFragmentObtained newMoonFragmentObtained)
    {
        areaChangesCount++;
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

    private void PromptEndGameDialogue()
    {
        StopAllCoroutines();
        StartCoroutine(ShowEndGameDialogue());
    }
}
