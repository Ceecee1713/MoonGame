using UnityEngine;

public class FinishingUI : MonoBehaviour
{
    void Start()
    { 
    }

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
