using UnityEngine;

/// <summary>
/// Manages the full moon statue with prompting the progression to the next day (prayer phase) as well as visuals for the moon statue
/// Such as material swapping and a particle effect
/// </summary>
/// 
/// <remarks>
/// This scripts works together with the "MoonPuzzleDialogueText", "GameManager", DialogueCanvas" scripts
/// 
/// See <see cref="MoonPuzzleDialogueText"/> - Listening to "MakeMoonStatueSpin" event that "MoonPuzzleDialogueText" publishes to spin the moon statue and start moon sparkles particles 
/// and listening to "NewMoonFragmentObtained" event that "MoonPuzzleDialogueText" publishes to swap materials on moon statue 
/// 
/// See <see cref="DialogueCanvas"/> - Publishing "TypeDialogueOnMainUI" event to show dialogue on the main player UI
/// See <see cref="GameManager"/> - Listening to "StopMoonStatueSpin" that "GameManager" publishes to stop moon statue spinning and stop moon sparkles particles
/// 
/// See <see cref="StorytellingDialogueData"/> for how dialogue messages are structured.
/// 
/// </remarks>

public class MoonVisibility : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem moonSparkles;
    [SerializeField]
    private Camera moonCamera; //Secondary camera to zoom in on the moon statue

    [Header ("Animation Information")]
    [SerializeField]
    private Animator moonAnimator;
    [SerializeField] 
    private string animationBoolName; //Name of animation bool used as a parameter for the moon spinning animation in the moon statue animator

    [Header ("Moon Dialogue - UI")]
    [SerializeField]
    private GameObject dialogueCanvas; //Dialogue canvas layered ontop of the main player UI
    [SerializeField]
    private StorytellingDialogueData completedMoonPuzzleDialogue; //Dialogue to appear on main player UI after successfully gaining a moon fragment
    [SerializeField]
    private float delayBeforeShowingMoonMessage = 1.5f;

    [Header ("Moon Statue Pieces Information")]
    [SerializeField]
    private GameObject [] moonFragments = new GameObject [3]; //Moon fragments to swap materials of
    [SerializeField]
    private Material litUpMoonMaterial;

    private int _moonCounter = 0;

    private bool _playAnimation = false;

    private const bool NEW_EXPLORATION_PHASE = true;
    private const bool STARTING_THE_GAME = false; 

    void Start()
    {
        EventBus.Instance.Subscribe<NewMoonFragmentObtained>(ObtainedNewMoonFragment);
        EventBus.Instance.Subscribe<StopMoonStatueSpin>(StopMoonStatueSpinAndSparkles);
        EventBus.Instance.Subscribe<MakeMoonStatueSpin>(MakeMoonStatueSpin);

        moonCamera.enabled = false;
        moonSparkles.Stop();
    }
    
    //"MakeMoonStatueSpin" is the name of an event. Empty event
    private void MakeMoonStatueSpin(MakeMoonStatueSpin makeMoonStatueSpin) //When all three moon puzzles are complete. Published by "MoonPuzzleDialogueText"
    {
        moonCamera.enabled = true;
        _playAnimation = true;
        moonAnimator.SetBool(animationBoolName, _playAnimation);
        moonSparkles.Play();
    }

    //"StopMoonStatueSpin" is the name of an event. Empty event
    private void StopMoonStatueSpinAndSparkles(StopMoonStatueSpin stopMoonStatueSpin) //Published by "GameManager"
    {
        moonCamera.enabled = false;
        _playAnimation = false;
        moonAnimator.SetBool(animationBoolName, _playAnimation);
        moonSparkles.Stop();
    }

    //"NewMoonFragmentObtained" is the name of an event. Empty event
    private void ObtainedNewMoonFragment(NewMoonFragmentObtained newMoonFragmentObtained) //Published by "MoonPuzzleDialogueText"
    {
        if(_moonCounter >= moonFragments.Length)
            return; 

        MeshRenderer moonFragmentRenderer = moonFragments[_moonCounter].GetComponent<MeshRenderer>();
        moonFragmentRenderer.material = litUpMoonMaterial;
        _moonCounter++;

        if(_moonCounter != moonFragments.Length)
            Invoke("ShowMoonStatueMessage",  delayBeforeShowingMoonMessage);
    }

    private void ShowMoonStatueMessage() 
    {
        dialogueCanvas.SetActive(true);
        EventBus.Instance.Publish(new TypeDialogueOnMainUI(completedMoonPuzzleDialogue, NEW_EXPLORATION_PHASE, STARTING_THE_GAME)); //Publish to "DialogueCanvas"
    }
}
