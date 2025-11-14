using UnityEngine;

public class ReturnMoonTextAdventureButton : MonoBehaviour
{
    [SerializeField]
    private GameObject cluebookUI;

    [SerializeField]
    private MoonPuzzleDialogueText moonPuzzleText;

    public void OnRestartTextAdventureClick()
    {
        if(cluebookUI.activeSelf == true)
            return;

        moonPuzzleText.RestartTextAdventureDialogue();
    }
}
