using UnityEngine;

public class MoonVisibility : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem moonSparkles;
    [SerializeField]
    private Camera moonCamera;

    [Header ("Animation Information")]
    [SerializeField]
    private Animator moonAnimator;
    [SerializeField] 
    private string animationBoolName;

    [Header ("Moon Dialogue - UI")]
    [SerializeField]
    private GameObject dialogueCanvas;
    [SerializeField]
    private StorytellingDialogueData completedMoonPuzzleDialogue;
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

    private bool _playAnimation = false;

    private const bool NEW_EXPLORATION_PHASE = true;
    private const bool STARTING_THE_GAME = false; 

    void Start()
    {
        EventBus.Instance.Subscribe<NewMoonFragmentObtained>(ObtainedNewMoonFragment);
        EventBus.Instance.Subscribe<StopMoonStatueSpin>(StopMoonSpinAndSparkles);
        EventBus.Instance.Subscribe<MakeMoonStatueSpin>(MakeMoonStatueSpin);

        moonCamera.enabled = false;
        moonSparkles.Stop();
    }
    
    private void MakeMoonStatueSpin(MakeMoonStatueSpin makeMoonStatueSpin) //When all three moon puzzles are complete
    {
        moonCamera.enabled = true;
        _playAnimation = true;
        moonAnimator.SetBool(animationBoolName, _playAnimation);
        moonSparkles.Play();
    }

    private void StopMoonSpinAndSparkles(StopMoonStatueSpin stopMoonStatueSpin) //Published by GameManager
    {
        moonCamera.enabled = false;
        _playAnimation = false;
        moonAnimator.SetBool(animationBoolName, _playAnimation);
        moonSparkles.Stop();
    }

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
        EventBus.Instance.Publish(new TypeDialogueOnMainUI(completedMoonPuzzleDialogue, NEW_EXPLORATION_PHASE, STARTING_THE_GAME));
    }
}
