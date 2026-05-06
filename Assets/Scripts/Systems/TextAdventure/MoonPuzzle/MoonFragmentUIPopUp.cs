using UnityEngine;
using UnityEngine.UI;

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
