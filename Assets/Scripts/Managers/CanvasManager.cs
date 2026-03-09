using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class CanvasManager : MonoBehaviour
{
    [Header ("All Fullscreen UI Canvases")]
    [SerializeField]
    private GameObject [] canvases; //Must contain ALL fullscreen UI canvases (UIs that completely cover a screen. Look at PhaseShifint_1 scene for reference) 

    [SerializeField]
    private float fadingTime = 1.0f;

    private bool _showTextAdventure;
    private GameObject _currentCanvas;
    private CanvasGroup _currentCanvasGroup, _newCanvasGroup;

    void Start()
    {
        EventBus.Instance.Subscribe<ChangeCanvases>(ChangeCanvases);
        EventBus.Instance.Subscribe<FadeSingleCanvas>(FadeSingleCanvas);
    }

    private void ChangeCanvases(ChangeCanvases changeCanvases)
    {
        for (int i = 0; i < canvases.Length; i++)
        {
            if (canvases[i].activeSelf == true) //Checking if only one canvas is active
            {
                _currentCanvas = canvases[i];
                _currentCanvasGroup = canvases[i].GetComponent<CanvasGroup>();
                break;
            }
        }

        _newCanvasGroup = changeCanvases.NewCanvas.GetComponent<CanvasGroup>();
        StartCoroutine(SwitchCanvases(_newCanvasGroup, changeCanvases.NewCanvas, changeCanvases.StartMoonPuzzle, changeCanvases.StartPrayerPhase));
    }

    private void FadeSingleCanvas(FadeSingleCanvas fadeSingleCanvas)
    {
        _currentCanvas = fadeSingleCanvas.CurrentCanvas;
        _currentCanvasGroup = fadeSingleCanvas.CurrentCanvas.GetComponent<CanvasGroup>();
        StopAllCoroutines();
        StartCoroutine(FadeOneCanvas(fadeSingleCanvas.FadeOutUI));
    }

    IEnumerator FadeOneCanvas(bool fadeOutUI)
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

    IEnumerator SwitchCanvases(CanvasGroup newCanvasGroup, GameObject newCanvas, bool startMoonPuzzle, bool startPrayerPhase)
    {
        //Fade out of current active canvas
        Tween firstTween = _currentCanvasGroup.DOFade(0f, fadingTime);
        yield return firstTween.WaitForCompletion();
        _currentCanvas.SetActive(false);

        newCanvas.SetActive(true);
        Tween secondTween = newCanvasGroup.DOFade(1.0f, fadingTime);
        yield return secondTween.WaitForCompletion();

        if(startMoonPuzzle == true)
            EventBus.Instance.Publish(new StartNewTextAdventure()); //Prompt the first message of dialogue to be said

        if(startPrayerPhase == true)
            EventBus.Instance.Publish(new StartPrayerPhase()); //Prompt the first message of dialogue to be said
    }
}
