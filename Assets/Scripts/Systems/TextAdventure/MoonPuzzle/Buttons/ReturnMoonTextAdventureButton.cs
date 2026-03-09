using UnityEngine;

public class ReturnMoonTextAdventureButton : MonoBehaviour
{
    [SerializeField]
    private GameObject cluebookUI;

    [SerializeField]
    private MoonPuzzleDialogueText moonPuzzleDialogueText;

    public void OnRestartTextAdventureClick()
    {
        if(cluebookUI.activeSelf == true)
            return;

        moonPuzzleDialogueText.RestartTextAdventureDialogue();
    }
}
