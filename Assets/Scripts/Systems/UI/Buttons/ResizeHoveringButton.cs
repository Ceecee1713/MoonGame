using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

/// <summary>
/// Increases a UI game object's scale 
/// </summary>
/// 
/// <remarks>
/// This script is to be attached to UI buttons that when hovered over, their scale increases
/// </remarks>

public class ResizeHoveringButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] 
    private float hoverSizeScale = 1.1f;

    private Tween tween;

    private const float NORMAL_SIZE_SCALE = 1f;
    private const float TWEEN_DURATION = 0.15f;

    public void OnPointerEnter(PointerEventData eventData) //Scale up by "hoverSizeScale" when cursor is hovering over this game object
    {
        tween?.Kill();
        tween = transform.DOScale(hoverSizeScale, TWEEN_DURATION).SetEase(Ease.OutQuad).SetLink(gameObject, LinkBehaviour.KillOnDestroy);;
    }

    public void OnPointerExit(PointerEventData eventData) //Return to normal scale size when cursor is not hovering over this game object
    {
        tween?.Kill();
        tween = transform.DOScale(NORMAL_SIZE_SCALE, TWEEN_DURATION).SetEase(Ease.OutQuad).SetLink(gameObject, LinkBehaviour.KillOnDestroy);;
    }
}
