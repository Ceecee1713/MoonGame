using UnityEngine;

public class ReturnMoonTextAdventureButton : MonoBehaviour
{
    [SerializeField]
    private AudioClip buttonClickSFX;

    [SerializeField]
    private GameObject cluebookUI;

    [SerializeField]
    private MoonPuzzleDialogueText moonPuzzleDialogueText;

    public void OnRestartTextAdventureClick()
    {
        if(cluebookUI.activeSelf == true)
            return;

        AudioManager.Instance.PlaySoundEffect(buttonClickSFX);
        moonPuzzleDialogueText.RestartTextAdventureDialogue();
    }
}
