using UnityEngine;

/// <summary>
/// Manages the player's interaction with the moon statue to prompt a prayer phase (progress to the next day)
/// </summary>
/// 
/// <remarks>
/// This scripts works together with the "DialogueCanvas", "CanvasManager", "PlayerInputController" scripts
/// See <see cref="DialogueCanvas"/> for how they work together - show dialogue on the main player UI
/// See <see cref="CanvasManager"/> for how they work together - switching to the storytelling UI canvas
/// See <see cref="PlayerInputController"/> for how they work together - prompting the "Interact" event this script listens to
/// 
/// See <see cref="StorytellingDialogueData"/> for how dialogue messages are structured.
/// 
/// </remarks>

public class PrayToMoonStatue : MonoBehaviour
{
    [SerializeField]
    private AudioClip prayingSFX;

    [Header ("UI Information")]
    [SerializeField]
    private GameObject storytellingUI;
    [SerializeField]
    private GameObject dialogueUI; //Dialogue canvas layered ontop of the main player UI

    [SerializeField]
    private StorytellingDialogueData moonStatueTutorialDialogue; 

    private bool _allowInput = true; //Prevent or allow for the player to interact with this item
    private bool _showMoonTutorial = false; //Flag whether the moon tutorial dialogue has appeared on the dialogue canvas
    private bool _playerCollisionDetected = false; //Flags if the player is inside this item's collision to be interacted with: if the player's in range or not
    private bool _interactedOnce = false; //Prevent or allow for the player to interact ONCE with this item when it's in collision 

    private const bool START_PRAYER_PHASE = true;
    private const bool START_MOON_PUZZLE = false;
    private const bool NEW_EXPLORATION_PHASE = false;
    private const bool STARTING_THE_GAME = false; 
    
    void Start()
    {
        EventBus.Instance.Subscribe<Interact>(OpenStorytellingUI);
        EventBus.Instance.Subscribe<ActivatePlayerInputs>(AllowPlayerInput);
    } 

    private void AllowPlayerInput(ActivatePlayerInputs activatePlayerInputs)
    {
        _allowInput = activatePlayerInputs.AllowInputs;
    }

    private void OpenStorytellingUI(Interact interact) //When player interacts with this game object. Published by "PlayerInputController"
    {
        if(_interactedOnce == true || _allowInput == false)
            return;

        if(_playerCollisionDetected == true)
        {
            AudioManager.Instance.PlaySoundEffect(prayingSFX);

            if(_showMoonTutorial == false) //Show moon tutorial dialogue ONCE
            {
                _showMoonTutorial = true;
                dialogueUI.SetActive(true);
                
                EventBus.Instance.Publish(new FreezePlayer(true));
                EventBus.Instance.Publish(new TypeDialogueOnMainUI(moonStatueTutorialDialogue, NEW_EXPLORATION_PHASE, STARTING_THE_GAME)); //Publish to "DialogueCanvas"
                return;
            }

            _interactedOnce = true;
            EventBus.Instance.Publish(new FreezePlayer(true));
            EventBus.Instance.Publish(new ChangeCanvases(storytellingUI, START_MOON_PUZZLE, START_PRAYER_PHASE)); //Publish to "CanvasManager"
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            _playerCollisionDetected = true;
            EventBus.Instance.Publish(new InCollision(_playerCollisionDetected));
        }  
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            _playerCollisionDetected = false; 
            _interactedOnce = false;
            EventBus.Instance.Publish(new InCollision(_playerCollisionDetected));
        }
    }
}
