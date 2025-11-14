using UnityEngine;

public class ReturnTextAdventureButton : MonoBehaviour
{
    [SerializeField]
    private GameObject cluebookUI;

    [SerializeField]
    private DialogueText moonPuzzleText;

    public void OnRestartTextAdventureClick()
    {
        if(cluebookUI.activeSelf == true)
            return;

        moonPuzzleText.RestartTextAdventureDialogue();
    }
}
