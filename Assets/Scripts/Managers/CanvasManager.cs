using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Manages swapping of and fading UI canvases alphas in the game, including publishing events after UI canvas swapping
/// </summary>
/// 
/// <remarks>
/// No other script should be swapping out or fading UI canvases
/// 
/// This script works together with scripts: "MoonPuzzleDialogueText", "StorytellingDialogueText"
/// See <see cref="MoonPuzzleDialogueText"/> - Listening to "FadeSingleCanvas" event "MoonPuzzleDialogueText" publishes to fade a single UI canvas
/// and publishing "StartNewTextAdventure" to prompt the first dialogue for a moon puzzle text adventure 
/// 
/// See <see cref="StorytellingDialogueText"/> - Publishing "StartPrayerPhase" to prompt the first dialogue of a prayer
/// 
///</remarks>

public class CanvasManager : MonoBehaviour
{
    [Header ("All Fullscreen UI Canvases")]
    [SerializeField]
    private GameObject [] canvases; 
    //Must contain ALL fullscreen UIs that completely cover a screen. Must be assigned in Inspector
    //Includes: Main Player UI, Storytelling UI, Text Adventure UI, Lose Game UI, Win Game UI

    [SerializeField]
    private float fadingTime = 1.0f; //Time to fade UI canvases' alphas in seconds

    private GameObject _currentCanvas; 
    private CanvasGroup _currentCanvasGroup, _newCanvasGroup;

    void Start()
    {
        EventBus.Instance.Subscribe<ChangeCanvases>(ChangeCanvases);
        EventBus.Instance.Subscribe<FadeSingleCanvas>(FadeSingleCanvas);
    }

    //Receives a ChangeCanvases event with parameters:
    //(GameObject) "NewCanvas" - The UI canvas that'll be swapped out with the current one.
    //(bool) "StartMoonPuzzle" - (true = prompt first message of the text adventure dialogue for the moon puzzle UI, 
    //false = do NOT prompt first message of the text adventure dialogue for the moon puzzle UI).
    //(bool) "StartPrayerPhase" - (true = Prompt first message of prayer dialogue to be said when praying to the Moon Statue, 
    //false = do NOT prompt first message of prayer dialogue to be said when praying to the Moon Statue).
    private void ChangeCanvases(ChangeCanvases changeCanvases) //Multiple publishers
    {
        for (int i = 0; i < canvases.Length; i++)
        {
            if (canvases[i].activeSelf == true) //Checking for ONE active canvas
            {
                _currentCanvas = canvases[i];
                _currentCanvasGroup = canvases[i].GetComponent<CanvasGroup>();
                break;
            }
        }

        _newCanvasGroup = changeCanvases.NewCanvas.GetComponent<CanvasGroup>();
        StartCoroutine(SwitchCanvases(_newCanvasGroup, changeCanvases.NewCanvas, changeCanvases.StartMoonPuzzle, changeCanvases.StartPrayerPhase));
    }

    //Receives a FadeSingleCanvas event with parameters:
    //(GameObject) "CurrentCanvas" - The current UI canvas that's active
    //(bool) "FadeOutUI" - (true = fade current UI canvas to 0 alpha, false = fade current UI canvas to 1 alpha)
    private void FadeSingleCanvas(FadeSingleCanvas fadeSingleCanvas) //published by "MoonPuzzleDialogueText"
    {
        _currentCanvas = fadeSingleCanvas.CurrentCanvas;
        _currentCanvasGroup = fadeSingleCanvas.CurrentCanvas.GetComponent<CanvasGroup>();
        StopAllCoroutines();
        StartCoroutine(FadeOneCanvas(fadeSingleCanvas.FadeOutUI));
    }

    private IEnumerator FadeOneCanvas(bool fadeOutUI)
    {
        if(fadeOutUI == true)
        {
            Tween firstTween = _currentCanvasGroup.DOFade(0f, fadingTime);
            yield return firstTween.WaitForCompletion();
            _currentCanvas.SetActive(false);
        }

        else
        {
            Tween firstTween = _currentCanvasGroup.DOFade(1f, fadingTime);
            yield return firstTween.WaitForCompletion();
        }
    }

    private IEnumerator SwitchCanvases(CanvasGroup newCanvasGroup, GameObject newCanvas, bool startMoonPuzzle, bool startPrayerPhase)
    {
        //Lower the currently active UI canvas' alpha completely and set UI activeness to false
        Tween firstTween = _currentCanvasGroup.DOFade(0f, fadingTime);
        yield return firstTween.WaitForCompletion();
        _currentCanvas.SetActive(false);

        //Raise the replacement UI canvas' alpha fully and set UI activeness to true
        newCanvas.SetActive(true);
        Tween secondTween = newCanvasGroup.DOFade(1.0f, fadingTime);
        yield return secondTween.WaitForCompletion();

        if(startMoonPuzzle == true)
            EventBus.Instance.Publish(new StartNewTextAdventure()); //Prompt first message of the text adventure dialogue on the moon puzzle UI
            //Publish to "MoonPuzzleDialogueText"

        if(startPrayerPhase == true)
            EventBus.Instance.Publish(new StartPrayerPhase()); //Prompt first message of prayer dialogue when praying to the Moon Statue on storytelling UI
            //Publish to "StorytellingDialogueText"
    }
}
