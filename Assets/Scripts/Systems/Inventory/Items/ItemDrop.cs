using UnityEngine;

/// <summary>
/// Manages an inventory item that's been dropped
/// </summary>
/// 
/// <remarks>
/// 
/// This script works together with the "MoonPuzzleDialogueText", "StorytellingDialogueText" scripts
/// See <see cref="MoonPuzzleDialogueText"/> - Listening to "NewExplorationPhase" event that "MoonPuzzleDialogueText" published, which destroys this game object 
/// See <see cref="StorytellingDialogueText"/> - Listening to "NewExplorationPhase" event that "StorytellingDialogueText" published, which destroys this game object 
/// 
/// </remarks>

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

    //"NewExplorationPhase" is the name of an event. Empty event
    private void DeleteSelfOnNewExplorationPhase(NewExplorationPhase newExplorationPhase) //Published by "MoonPuzzleDialogueText" or "StorytellingDialogueText"
    {
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            AudioManager.Instance.PlaySoundEffect(itemDropSFX);
    }
}
