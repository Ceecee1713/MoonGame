using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Manages swapping of and fading UI canvases alphas in the game, including publishing events after UI canvas swapping
/// </summary>
/// <remarks>
/// No other script should be swapping out or fading UI canvases
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

    //Checks which ONE UI canvas is active and assign the replacement UI canvas based on a published event
    private void ChangeCanvases(ChangeCanvases changeCanvases) 
    {
        for (int i = 0; i < canvases.Length; i++)
        {
            if (canvases[i].activeSelf == true) 
            {
                _currentCanvas = canvases[i];
                _currentCanvasGroup = canvases[i].GetComponent<CanvasGroup>();
                break;
            }
        }

        _newCanvasGroup = changeCanvases.NewCanvas.GetComponent<CanvasGroup>();
        StartCoroutine(SwitchCanvases(_newCanvasGroup, changeCanvases.NewCanvas, changeCanvases.StartMoonPuzzle, changeCanvases.StartPrayerPhase));
    }

    //Assigns the currently active UI canvas based on a published event
    private void FadeSingleCanvas(FadeSingleCanvas fadeSingleCanvas) 
    {
        _currentCanvas = fadeSingleCanvas.CurrentCanvas;
        _currentCanvasGroup = fadeSingleCanvas.CurrentCanvas.GetComponent<CanvasGroup>();
        StopAllCoroutines();
        StartCoroutine(FadeOneCanvas(fadeSingleCanvas.FadeOutUI));
    }

    //Fades the current canvas out (alpha 0, then deactivate) or in (alpha 1) based on the FadeSingleCanvas event.
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

    //Fades the current canvas out, swaps it for newCanvas, then fades the new canvas in.
    //Publishes StartNewTextAdventure or StartPrayerPhase after the swap if flagged in the ChangeCanvases event.
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
            EventBus.Instance.Publish(new StartNewTextAdventure()); //Prompt first message of the text adventure dialogue for the moon puzzle UI

        if(startPrayerPhase == true)
            EventBus.Instance.Publish(new StartPrayerPhase()); //Prompt first message of prayer dialogue to be said when praying to the Full Moon Statue
    }
}
