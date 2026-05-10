using UnityEngine;

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

    public void OnAcceptClick() //Start Moon Puzzle Text Adventure
    {
        AudioManager.Instance.PlaySoundEffect(buttonClickSFX);
        
        EventBus.Instance.Publish(new ChangeCanvases(textAdventureUI, START_MOON_PUZZLE, START_PRAYER_PHASE));
        warningMoonPuzzleUIScript.MarkMoonAreaAsBeenExplored();
        warningMoonPuzzleUI.SetActive(false);
    }
}
