using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages a UI game object's alpha value if it has a Canvas Group component onto it
/// </summary>
/// 
/// <remarks>
/// This scrip is to be attached to a UI that only shows an image and text but has no other functionality to its UI.
/// It just acts as a display
/// </remarks>

public class MoonFragmentUIPopUp : MonoBehaviour
{
    private CanvasGroup _canvasGroup;

    void Start()
    {
        _canvasGroup = this.gameObject.GetComponent<CanvasGroup>();
        this.gameObject.SetActive(false);
    }

    void Update()
    {
        if(_canvasGroup.alpha == 0.0f)
            this.gameObject.SetActive(false);
    }
}
