using UnityEngine;

public class MoonVisibility : MonoBehaviour
{
    [Header ("Moon Dialogue - UI")]
    [SerializeField]
    private GameObject dialogueCanvas;
    [SerializeField]
    private string moonStatueDialogue = "You've lit up a piece of the Moon Statue! Continue on your journey to restore the Moon's brillance!";
    [SerializeField]
    private float delayBeforeShowingMoonMessage = 1.5f;

    [Header ("Moon Statue Pieces Information")]
    [SerializeField]
    private GameObject [] moonFragments = new GameObject [3];
    [SerializeField]
    private Material litUpMoonMaterial;
    [SerializeField]
    private Material darkMoonMaterial;

    private int moonCounter = 0;

    private const bool NEW_EXPLORATION_PHASE = true;

    void Start()
    {
        EventBus.Instance.Subscribe<NewMoonFragmentObtained>(ObtainedNewMoonFragment);
    }

    //Called when starting new exploration phase after completing a moon puzzle text adventure
    private void ObtainedNewMoonFragment(NewMoonFragmentObtained newMoonFragmentObtained)
    {
        if(moonCounter >= moonFragments.Length)
            return; 

        MeshRenderer moonFragmentRenderer = moonFragments[moonCounter].GetComponent<MeshRenderer>();
        moonFragmentRenderer.material = litUpMoonMaterial;
        moonCounter++;

        if(moonCounter != moonFragments.Length)
            Invoke("ShowMoonStatueMessage",  delayBeforeShowingMoonMessage);
    }

    private void ShowMoonStatueMessage()
    {
        dialogueCanvas.SetActive(true);
        EventBus.Instance.Publish(new TypeOutSingleDialogue(moonStatueDialogue, NEW_EXPLORATION_PHASE));
    }
}
