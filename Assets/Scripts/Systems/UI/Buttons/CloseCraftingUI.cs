using UnityEngine;

public class CloseCraftingUI : MonoBehaviour
{
    [SerializeField]
    private AudioClip buttonClickSFX;

    [SerializeField]
    private GameObject craftingUI;

    public void CloseUIClick()
    {
        AudioManager.Instance.PlaySoundEffect(buttonClickSFX);
        
        EventBus.Instance.Publish(new FreezePlayer(false));
        EventBus.Instance.Publish(new PauseExplorationTimer(false));
        craftingUI.SetActive(false);
    }
}
