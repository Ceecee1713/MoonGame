using UnityEngine;

public class OpenCluebookMainUI : MonoBehaviour
{
    [SerializeField]
    private AudioClip buttonClickSFX;

    [Header ("UI Information")]
    [SerializeField]
    private GameObject cluebookUI;
    [SerializeField]
    private GameObject dialogueMainUI;
    [SerializeField]
    private GameObject craftingUI;
    [SerializeField]
    private GameObject pauseMenuUI;

    private bool _isAChestOpen;

    void Start()
    {
        EventBus.Instance.Subscribe<ChestIsOpen>(ChangeInput);
    }

    public void OpenCluebookClick()
    {
        if(craftingUI.activeSelf == true || pauseMenuUI.activeSelf == true || dialogueMainUI.activeSelf == true || _isAChestOpen == true)
            return;

        AudioManager.Instance.PlaySoundEffect(buttonClickSFX);
        cluebookUI.SetActive(true);
    }

    private void ChangeInput(ChestIsOpen chestIsOpen)
    {
        _isAChestOpen = chestIsOpen.IsAChestOpen;
    }
}
