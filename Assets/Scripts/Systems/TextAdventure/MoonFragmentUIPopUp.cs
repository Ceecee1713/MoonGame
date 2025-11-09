using UnityEngine;
using UnityEngine.UI;

public class MoonFragmentUIPopUp : MonoBehaviour
{
    [SerializeField]
    private Image moonFragmentImage;

    private CanvasGroup _canvasGroup;

    void Awake()
    {
        EventBus.Instance.Subscribe<DisplayMoonFragmentImage>(SetMoonImageSprite);
    }

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

    private void SetMoonImageSprite(DisplayMoonFragmentImage displayMoonFragmentImage)
    {
        moonFragmentImage.sprite = displayMoonFragmentImage.MoonFragmentSprite;
    }
}
