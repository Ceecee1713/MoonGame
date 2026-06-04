using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Manages the start menu UI functionality - only its display
/// </summary>
/// 
/// <remarks>
/// This script is for the start menu that's in a separate scene. That scene is only for the start menu: StartScene
/// The actual game is held in a separate scene: Game
/// </remarks>

public class LoadStartUI : MonoBehaviour
{
    [Header ("Audio")]
    [SerializeField]
    private AudioClip backgroundMusic;
    [SerializeField]
    [Range (0,1)]
    private float desiredVolumeForMusic;

    private CanvasGroup _canvasGroup;

    private float _durationOfFade = 1.5f;
    
    private const float DELAY = 0.25f;

    void Start()
    {
        _canvasGroup = this.gameObject.GetComponent<CanvasGroup>();
        StartCoroutine(ShowCanvas());
    }

    private IEnumerator ShowCanvas()
    {
        yield return new WaitForSeconds(DELAY);

        AudioManager.Instance.SetVolumeForBackgroundNoise(desiredVolumeForMusic);
        AudioManager.Instance.PlayBackgroundNoise(backgroundMusic);

        Tween firstTween = _canvasGroup.DOFade(1f, _durationOfFade);
        yield return firstTween.WaitForCompletion();
    }
}
