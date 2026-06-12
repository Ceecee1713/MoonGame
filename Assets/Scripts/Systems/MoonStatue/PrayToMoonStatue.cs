using UnityEngine;

/// <summary>
/// Manages the player's interaction with the moon statue to prompt a prayer phase (progress to the next day)
/// </summary>
/// 
/// <remarks>
/// This scripts works together with the "DialogueCanvas", "CanvasManager", "PlayerInputController", "PlayerStateMachine", "InventoryUI" scripts
/// See <see cref="DialogueCanvas"/> - Publishing "TypeDialogueOnMainUI" event show dialogue on the main player UI
/// See <see cref="CanvasManager"/> - Publishing "ChangeCanvases" event to switch to the storytelling UI canvas and prompting to show first message of prayer dialogue on storytelling UI
/// See <see cref="PlayerInputController"/> - Listening to the "Interact" event that "PlayerInputController" publishes
/// See <see cref="PlayerStateMachine"/> - Publishing "FreezePlayer" event to freeze player
/// See <see cref="InventoryUI"/> - Publishing "PreventPlayerInteractingWithInventory" event prevent/allow the player to interact with the inventory
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

    //Receives a "ActivatePlayerInputs" event with parameters:
    //(bool) AllowInputs - (true = allow the player to interact with world objects and UI, 
    //false = do NOT allow the player to interact with world objects and UI).
    private void AllowPlayerInput(ActivatePlayerInputs activatePlayerInputs) //Multiple publishers and subscribers
    {
        _allowInput = activatePlayerInputs.AllowInputs;
    }

    //"Interact" is the name of an event. Empty event
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
                
                EventBus.Instance.Publish(new FreezePlayer(true)); //Publish to "PlayerStateMachine"
                EventBus.Instance.Publish(new TypeDialogueOnMainUI(moonStatueTutorialDialogue, NEW_EXPLORATION_PHASE, STARTING_THE_GAME)); //Publish to "DialogueCanvas"
                return;
            }

            _interactedOnce = true;
            EventBus.Instance.Publish(new FreezePlayer(true)); //Publish to "PlayerStateMachine"
            EventBus.Instance.Publish(new ChangeCanvases(storytellingUI, START_MOON_PUZZLE, START_PRAYER_PHASE)); //Publish to "CanvasManager"
            //Prompt first message of prayer dialogue to show when praying to the Moon Statue on storytelling UI
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            _playerCollisionDetected = true;
            EventBus.Instance.Publish(new PreventPlayerInteractingWithInventory(_playerCollisionDetected)); //Publish to "InventoryUI"
        }  
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            _playerCollisionDetected = false; 
            _interactedOnce = false;
            EventBus.Instance.Publish(new PreventPlayerInteractingWithInventory(_playerCollisionDetected)); //Publish to "InventoryUI"
        }
    }
}
