using UnityEngine;

/// <summary>
/// Manages the accept button for the UI screen that appears before starting the moon puzzle text adventure and after interacting with a decorative moon statue
/// </summary>
/// 
/// <remarks>
/// This script is to be attached a button on the UI screen that asks and warns the player about entering a moon puzzle text adventure.
/// This script will be attached to an accept button to proceed to the moon puzzle text adventure UI
/// 
/// This script works together with "WarningMoonPuzzleUI" and "CanvasManager" scripts
/// See <see cref="WarningMoonPuzzleUI"/> for how they work together - accessing public methods as they're on the same game object
/// See <see cref="CanvasManager"/> for how they work together - Swapping canvases to show moon puzzle text adventure UI
/// "CanvasManager" also starts the moon puzzle text adventure UI in its own IEnumerator
/// 
/// </remarks>

public class AcceptButton : MonoBehaviour
{
    [SerializeField]
    private AudioClip buttonClickSFX;

    [Header ("UI Information")]
    [SerializeField]
    private GameObject textAdventureUI;
    [SerializeField]
    private GameObject warningMoonPuzzleUI;
    [SerializeField]
    private WarningMoonPuzzleUI warningMoonPuzzleUIScript;

    private const bool START_MOON_PUZZLE = true;
    private const bool START_PRAYER_PHASE = false;

    public void OnAcceptClick() 
    {
        AudioManager.Instance.PlaySoundEffect(buttonClickSFX);
        
        EventBus.Instance.Publish(new ChangeCanvases(textAdventureUI, START_MOON_PUZZLE, START_PRAYER_PHASE)); //Publish to "CanvasManager"
        warningMoonPuzzleUIScript.FadeMoonPuzzleAreaAudio();
        warningMoonPuzzleUI.SetActive(false);
    }
}
