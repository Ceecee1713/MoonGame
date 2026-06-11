using UnityEngine;

/// <summary>
/// Manages the cluebook button on the main player UI to open the cluebook UI
/// </summary>
/// 
/// <remarks>
/// This script works together with these scripts: ChestInteraction, ChestButton
/// See <see cref="ChestInteraction"/> - Listening to "ChestIsOpen" event that "ChestInteraction" publishes to mark if a chest is open or not
/// See <see cref="ChestButton"/> - Listening to "ChestIsOpen" event that "ChestButton" publishes to mark if a chest is open or not
/// 
/// </remarks>

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

    //Receives a "ChestIsOpen" event with parameters:
    //(bool) IsAChestOpen - (true = a chest is opened by player interaction, 
    //false = a chest is NOT opened by player interaction).
    private void ChangeInput(ChestIsOpen chestIsOpen) //Published by "ChestInteraction" or "ChestButton"
    {
        _isAChestOpen = chestIsOpen.IsAChestOpen;
    }
}
