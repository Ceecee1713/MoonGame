using UnityEngine;

/// <summary>
/// Manages functionality of an NPC with prompting dialogue across dialogue UI, freezing player and their inputs and 
/// prompting to add a clue fragment to the cluebook
/// </summary>
/// 
/// <remarks>
/// See <see cref="StorytellingDialogueData"/> for how dialogue messages are structured.
/// 
/// This script works together with the "DialogueCanvas", "MoonPuzzleDialogueText", "CluebookManager", "PlayerInputController", 
/// "PlayerStateMachine", "InventoryUI", "PlayerHealth" scripts
/// 
/// See <see cref="MoonPuzzleDialogueText"/> - Listening to "TypeDialogueOnMainUI" event that "MoonPuzzleDialogueText" publishes to destroy self 
/// See <see cref="DialogueCanvas"/> - publishing "TypeDialogueOnMainUI" event to display dialogue on the dialogue canvas
/// See <see cref="CluebookManager"/> - publishing "FoundClueFragment" event to add clue fragment to cluebook
/// See <see cref="PlayerInputController"/> - Listening to "Interact" event that "PlayerInputController" publishes 
/// See <see cref="PlayerStateMachine"/> - publishing "FreezePlayer" event to freeze player in place
/// See <see cref="InventoryUI"/> - publishing "PreventPlayerInteractingWithInventory" event to allow/prevent player input with the inventory system
/// See <see cref="PlayerHealth"/> - publishing "MaintainPlayerHealth" event to maintain player's current health
/// 
/// This script works with multiple other scripts that publish and subscribe to  "ActivatePlayerInputs" event
/// Please see <see cref="AddItemToInventory"/> to get the full details as it would be too much to write in this script alone
/// 
/// </remarks>

public class NPC : MonoBehaviour
{
    [SerializeField]
    private GameObject dialogueCanvas;

    [SerializeField]
    private StorytellingDialogueData npcDialogue;

    [SerializeField]
    [Range(1, 3)]
    private int areaNumberForNPC;

    private int _numberOfMoonPuzzlesCompleted = 0;

    private string _npcMessage;

    private const int FIRST_NPC_MESSAGE_INDEX = 0;

    private bool _allowInput = true; //Prevent or allow for the player to interact with this item
    private bool _playerStayingInCollision = false; //Flags if the player's remaining inside the item's collision
    private bool _playerInCollision = false; //Flags if the player is inside this item's collision to be interacted with: if the player's in range or not

    private const bool NEW_EXPLORATION_PHASE = false;
    private const bool STARTING_THE_GAME = false; 

    void Start()
    {
        _npcMessage = npcDialogue.Messages[FIRST_NPC_MESSAGE_INDEX].message;

        EventBus.Instance.Subscribe<Interact>(CheckToShowDialogue);
        EventBus.Instance.Subscribe<ActivatePlayerInputs>(AllowPlayerInput);
        EventBus.Instance.Subscribe<NewMoonFragmentObtained>(DestroyAfterMoonPuzzleCompletion);
    }

    void OnDestroy()
    {
        if (EventBus.Instance != null)
        {
            EventBus.Instance.Unsubscribe<Interact>(CheckToShowDialogue);
            EventBus.Instance.Unsubscribe<ActivatePlayerInputs>(AllowPlayerInput);
            EventBus.Instance.Unsubscribe<NewMoonFragmentObtained>(DestroyAfterMoonPuzzleCompletion);
        }
    }

    //"NewMoonFragmentObtained" is the name of an event. Empty event
    private void DestroyAfterMoonPuzzleCompletion(NewMoonFragmentObtained newMoonFragmentObtained) //Published by "MoonPuzzleDialogueText"
    {
        _numberOfMoonPuzzlesCompleted++;

        if(areaNumberForNPC == _numberOfMoonPuzzlesCompleted)
            Destroy(this.gameObject);
    }

    //Receives a "ActivatePlayerInputs" event with parameters:
    //(bool) AllowInputs - (true = allow the player to interact with world objects and UI, 
    //false = do NOT allow the player to interact with world objects and UI).
    private void AllowPlayerInput(ActivatePlayerInputs activatePlayerInputs) //Multiple publishers and subscribers
    {
        _allowInput = activatePlayerInputs.AllowInputs;
    }

    //"Interact" is the name of an event. Empty event
    private void CheckToShowDialogue(Interact pickingUpItem) //When player interacts with this game object. Published by "PlayerInputController"
    
    {
        if(_allowInput == false)
            return;

        if(_playerStayingInCollision == true)
        {
            dialogueCanvas.SetActive(true);

            EventBus.Instance.Publish(new FreezePlayer(true)); //Publish to "PlayerStateMachine"
            EventBus.Instance.Publish(new MaintainPlayerHealth(true)); //Publish to "PlayerHealth"
            EventBus.Instance.Publish(new TypeDialogueOnMainUI(npcDialogue, NEW_EXPLORATION_PHASE, STARTING_THE_GAME)); //Publish to "DialogueCanvas"
            EventBus.Instance.Publish(new FoundClueFragment(_npcMessage)); //Publish to "CluebookManager" 
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            _playerInCollision = true;
            EventBus.Instance.Publish(new PreventPlayerInteractingWithInventory(_playerInCollision)); //Publish to "InventoryUI"
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
            _playerStayingInCollision = true;
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            _playerInCollision = false;
            _playerStayingInCollision = false; 
            EventBus.Instance.Publish(new PreventPlayerInteractingWithInventory(_playerInCollision)); //Publish to "InventoryUI"
        }
    }
}
