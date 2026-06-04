using UnityEngine;

/// <summary>
/// Manages the moon puzzle text adventure UI's try again button
/// </summary>
/// 
/// <remarks>
/// This script is to be attached to a choice button on the moon puzzle text adventure UI.
/// The button is to act as the button to restart a moon puzzle question after the player selects the wrong choice button
/// 
/// This script is to be attached to the moon puzzle text adventure UI as well as directly accessing the 
/// "MoonPuzzleDialogueText" script given that they'll be on the same game object.
/// See <see cref="MoonPuzzleDialogueText"/> for how they work together - calling public methods from "MoonPuzzleDialogueText"
/// 
/// </remarks>

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
