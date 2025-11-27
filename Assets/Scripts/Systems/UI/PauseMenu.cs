using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    void Awake()
    {
        EventBus.Instance.Subscribe<PauseGame>(DisplayPauseMenu);
    }

    void Start()
    {
        this.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        EventBus.Instance.Publish(new FreezePlayer(true));
        EventBus.Instance.Publish(new MaintainPlayerHealth(true));
        EventBus.Instance.Publish(new PauseExplorationTimer(true));
    }

    void OnDisable()
    {
        EventBus.Instance.Publish(new FreezePlayer(false));
        EventBus.Instance.Publish(new MaintainPlayerHealth(false));
        EventBus.Instance.Publish(new PauseExplorationTimer(false));
    }

    private void DisplayPauseMenu(PauseGame pauseGame)
    {
        this.gameObject.SetActive(true);
    }
}
