using UnityEngine;

/// <summary>
/// Manages the player's interaction with making the crafting UI appear and pausing the game
/// This script is to be attached to the crafting table game object the player is meant to interact with
/// </summary>
/// 
/// <remarks>
/// 
/// This script works closely with "PlayerInputController", "InventoryUI", "ExplorationTimer" scripts 
/// See <see cref="PlayerInputController"/> - Listening to "Interact" event that "PlayerInputController" publishes
/// See <see cref="InventoryUI"/> - Publishing "PreventPlayerInteractingWithInventory" to prevent/allow player to interact with player inventory
/// See <see cref="ExplorationTimer"/> - Publishing "PauseExplorationTimer" to pause exploration timer countdown
/// 
/// This script works with multiple other scripts that subscribe and publish "ActivatePlayerInputs" event. 
/// Please see <see cref="AddItemToInventory"/> to get the full details as it would be too much to write in this script alone
/// 
///</remarks>

public class CraftingTable : MonoBehaviour
{
    [SerializeField]
    private GameObject craftingUI;

    private bool _allowInput = true; //Prevent or allow for the player to interact or click on certain objects during runtime
    private bool _playerStayingInCollision = false; //Prevent or allow pausing the game and freezing the player in place
    private bool _playerInCollision = false; //Flag whether the player can interact with the crafting table or not: if they're in range or not

    void Start()
    {
        EventBus.Instance.Subscribe<Interact>(OpenCraftingUI);
        EventBus.Instance.Subscribe<ActivatePlayerInputs>(AllowPlayerInput);
    }

    //Receives a "ActivatePlayerInputs" event with parameters:
    //(bool) AllowInputs - (true = allow the player to interact with world objects and UI, 
    //false = do NOT allow the player to interact with world objects and UI).
    private void AllowPlayerInput(ActivatePlayerInputs activatePlayerInputs) //Mulitple publishers
    {
        _allowInput = activatePlayerInputs.AllowInputs;
    }

    //"Interact" is the name of an event. Empty event
    private void OpenCraftingUI(Interact interact) //When player interacts with this game object. Published by "PlayerInputController"
    {
        if(_allowInput == false)
            return;

        if(_playerStayingInCollision == true)
        {
            craftingUI.SetActive(true);
            EventBus.Instance.Publish(new FreezePlayer(true)); //Publish to "PlayerStateMachine"
            EventBus.Instance.Publish(new PauseExplorationTimer(true)); //Publish to "ExplorationTimer"
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
