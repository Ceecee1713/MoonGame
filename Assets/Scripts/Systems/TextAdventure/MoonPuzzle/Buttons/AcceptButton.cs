using UnityEngine;

public class AcceptButton : MonoBehaviour
{
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
        EventBus.Instance.Publish(new ChangeCanvases(textAdventureUI, START_MOON_PUZZLE, START_PRAYER_PHASE));
        warningMoonPuzzleUIScript.MarkMoonAreaAsBeenExplored();
        warningMoonPuzzleUI.SetActive(false);
    }
}
