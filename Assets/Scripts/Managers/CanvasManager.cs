using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class CanvasManager : MonoBehaviour
{
    [Header ("Main Canvases")]
    [SerializeField]
    private GameObject [] canvases; //Must contain ALL UI canvases (excluding pause menu, plain black screen)

    [Header ("Fading Times")]
    [SerializeField]
    private float normalFadingTime = 1.0f;
    [SerializeField]
    private float shortenedFadingTime = 0.45f;

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
            if (canvases[i].activeSelf == true) //Checking if a canvas is active (only one canvas)
            {
                _currentCanvas = canvases[i];
                _currentCanvasGroup = canvases[i].GetComponent<CanvasGroup>();
                break;
            }
        }

        _newCanvasGroup = changeCanvases.NewCanvas.GetComponent<CanvasGroup>();
        StartCoroutine(SwitchCanvases(_newCanvasGroup, changeCanvases.NewCanvas, changeCanvases.SolvedMoonPuzzle, changeCanvases.PromptTextAdventure));
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
            Tween firstTween = _currentCanvasGroup.DOFade(0f, normalFadingTime);
            yield return firstTween.WaitForCompletion();
            _currentCanvas.SetActive(false);
        }

        else
        {
            Tween firstTween = _currentCanvasGroup.DOFade(1f, normalFadingTime);
            yield return firstTween.WaitForCompletion();
        }
    }

    IEnumerator SwitchCanvases(CanvasGroup newCanvasGroup, GameObject newCanvas, bool solvedMoonPuzzle, bool promptTextAdventure)
    {
        //Fade out of current active canvas
        Tween firstTween = _currentCanvasGroup.DOFade(0f, normalFadingTime);
        yield return firstTween.WaitForCompletion();
        _currentCanvas.SetActive(false);

        if(solvedMoonPuzzle == true)
        {
            newCanvas.SetActive(true);
            Tween secondTween = newCanvasGroup.DOFade(1.0f, shortenedFadingTime);
            yield return secondTween.WaitForCompletion();
        }

        else
        {
            newCanvas.SetActive(true);
            Tween secondTween = newCanvasGroup.DOFade(1.0f, normalFadingTime);
            yield return secondTween.WaitForCompletion();
        }

        if(promptTextAdventure == true)
        {
            //Prompt the first message of dialogue to be said
            EventBus.Instance.Publish(new StartNewTextAdventure());
        }
    }
}
