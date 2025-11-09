using UnityEngine;

public class ReturnTextAdventureButton : MonoBehaviour
{
    [SerializeField]
    private DialogueText moonPuzzleText;

    public void OnRestartTextAdventureClick()
    {
        moonPuzzleText.RestartTextAdventureDialogue();
    }
}
