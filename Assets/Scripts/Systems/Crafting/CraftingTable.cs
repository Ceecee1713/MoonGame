using UnityEngine;

/// <summary>
/// Manages the player's interaction with making the crafting UI appear and pausing the game
/// This script is to be attached to the crafting table game object the player is meant to interact with
/// </summary>
/// 
/// <remarks>
/// This script works closely with "PlayerInputController" and "ExplorationTimer" scripts 
/// See <see cref="PlayerInputController"/> for how they work together - prompting Interact event that this script listens to
/// See <see cref="ExplorationTimer"/> for how they work together - pausing the exploration timer countdown
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

    private void AllowPlayerInput(ActivatePlayerInputs activatePlayerInputs)
    {
        _allowInput = activatePlayerInputs.AllowInputs;
    }

    private void OpenCraftingUI(Interact interact) //When player interacts with this game object. Published by "PlayerInputController"
    {
        if(_allowInput == false)
            return;

        if(_playerStayingInCollision == true)
        {
            craftingUI.SetActive(true);
            EventBus.Instance.Publish(new FreezePlayer(true));
            EventBus.Instance.Publish(new PauseExplorationTimer(true)); //Publish to "ExplorationTimer"
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            _playerInCollision = true;
            EventBus.Instance.Publish(new InCollision(_playerInCollision));
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
            EventBus.Instance.Publish(new InCollision(_playerInCollision));
        }
    }
}
