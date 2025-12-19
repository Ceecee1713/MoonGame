using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    void Start()
    {
        EventBus.Instance.Subscribe<NewExplorationPhase>(DeleteSelfOnNewExplorationPhase);
    }

    void OnDestroy()
    {
        if (EventBus.Instance != null)
            EventBus.Instance.Unsubscribe<NewExplorationPhase>(DeleteSelfOnNewExplorationPhase);
    }

    private void DeleteSelfOnNewExplorationPhase(NewExplorationPhase newExplorationPhase)
    {
        Destroy(this.gameObject);
    }
}
