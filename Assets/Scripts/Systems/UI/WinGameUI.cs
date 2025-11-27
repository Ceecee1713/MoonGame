using UnityEngine;

public class WinGameUI : MonoBehaviour
{
    void OnEnable()
    {
        EventBus.Instance.Publish(new FreezePlayer(true));
        EventBus.Instance.Publish(new MaintainPlayerHealth(true));
        EventBus.Instance.Publish(new PauseExplorationTimer(true));
    }

    void OnDisable()
    {

    }
}
