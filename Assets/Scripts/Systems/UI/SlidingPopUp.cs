using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Manages the sliding up/down of a UI game object when enabled/disabled
/// </summary>
/// 
/// <remarks>
/// This script is to be attached to the UI game objects that act as dialogue text boxes
/// </remarks>

public class SlidingPopUp : MonoBehaviour
{
    [SerializeField]
    private RectTransform popUpRectTransform;

    [SerializeField] 
    private float distanceToMove = 50.0f;

    [SerializeField] 
    private float duration = 0.5f;

    private Vector2 _originalPosition;
    private Vector2 _upwardTargetPosition;

    private bool _hasBeenDisabledOnStart = false;
    private bool _isRaised = false; 

    private Tween tween;

    void Awake()
    {
        _originalPosition = new Vector2 (popUpRectTransform.anchoredPosition.x, popUpRectTransform.anchoredPosition.y);
        _upwardTargetPosition = popUpRectTransform.anchoredPosition + new Vector2(0f, distanceToMove);
    }

    void OnEnable()
    {
        if(_hasBeenDisabledOnStart == true)
        {
            tween?.Kill();
            tween = popUpRectTransform.DOAnchorPos(_upwardTargetPosition, duration).SetEase(Ease.OutQuad).SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            _isRaised = true;
        }
    }

    void OnDisable()
    {
        _hasBeenDisabledOnStart = true;
        
        if(_isRaised == true)
        {
            tween?.Kill();
            tween = popUpRectTransform.DOAnchorPos(_originalPosition, duration).SetEase(Ease.OutQuad).SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            _isRaised = false;
        }
    }
}
