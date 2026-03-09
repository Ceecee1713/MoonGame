using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class LoadStartUI : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    private float _durationOfFade = 1.5f;
    private const float DELAY = 0.25f;

    void Start()
    {
        _canvasGroup = this.gameObject.GetComponent<CanvasGroup>();
        StartCoroutine(ShowCanvas());
    }

    IEnumerator ShowCanvas()
    {
        yield return new WaitForSeconds(DELAY);
        Tween firstTween = _canvasGroup.DOFade(1f, _durationOfFade);
        yield return firstTween.WaitForCompletion();
    }
}
