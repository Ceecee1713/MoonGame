using UnityEngine;

public class DeclineButton : MonoBehaviour
{
    [SerializeField]
    private AudioClip buttonClickSFX;

    [SerializeField]
    private GameObject warningMoonPuzzleUI;
    
    public void OnDeclineCilck()
    {
        AudioManager.Instance.PlaySoundEffect(buttonClickSFX);
        
        EventBus.Instance.Publish(new FreezePlayer(false));
        EventBus.Instance.Publish(new MaintainPlayerHealth(false));
        EventBus.Instance.Publish(new PauseExplorationTimer(false));
        warningMoonPuzzleUI.SetActive(false);
    }
}
