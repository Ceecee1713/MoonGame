using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField]
    private AudioClip itemDropSFX;

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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            AudioManager.Instance.PlaySoundEffect(itemDropSFX);
    }
}
