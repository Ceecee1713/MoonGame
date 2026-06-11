using UnityEngine;

/// <summary>
/// Manages an interactable inventory item that can be added to the player's inventory
/// </summary>
/// 
/// <remarks>
/// See <see cref="InventoryItemTypes"/> for what makes up an inventory item and how inventory UI slots are made up.
/// 
/// This script works together with scripts: "MoonPuzzleDialogueText", "StorytellingDialogueText", "PlayerInputController", "InventoryUI"
/// See <see cref="MoonPuzzleDialogueText"/> - Listening to "ResetWorldItemsActiveness" event "MoonPuzzleDialogueText" publishes 
/// See <see cref="StorytellingDialogueText"/> - Listening to "ResetWorldItemsActiveness" event "StorytellingDialogueText" publishes 
/// See <see cref="PlayerInputController"/> - Listening to "Interact" event "PlayerInputController" publishes 
/// See <see cref="InventoryUI"/> - Publishing "AddItemToInventory" event to add the inventory item to player inventory,
/// and publishing "PreventPlayerInteractingWithInventory" to allow/prevent player input with the inventory system
/// 
/// See <see cref="PlayerSpawner"/> - Instantiating an inventory item
/// 
/// This script works with multiple other scripts that publish and subscribe to "ActivatePlayerInputs" event
/// Please see <see cref="AddItemToInventory"/> to get the full details as it would be too much to write in this script alone
/// 
/// </remarks>

public class InteractableItem : MonoBehaviour
{
    [SerializeField]
    private AudioClip interactionSFX;

    [SerializeField]
    public ItemData inventoryItem; 

    [Header ("Game Object's Visibility - Environment")]
    /// <summary> A child game object to set inactive on a prefab game object.</summary>
    public GameObject gameObjectToSetInactive;

    [SerializeField]
    private bool makeGameObjectInactive = false; //Flag whether a child game object needs to be set inactive. Works together with "gameObjectToSetInactive"

    /// <summary> Accessed by PlayerSpawner, used when I=instantiating a dropped inventory item. </summary>
    [HideInInspector] 
    public bool DeleteAfterInteraction = false; 

    /// <summary> Accessed by PlayerSpawner, used when I=instantiating a dropped inventory item. </summary>
    [HideInInspector]
    public bool InteractedByPlayerOnce = false; 

    private bool _allowInput = true; //Prevent or allow for the player to interact with this item
    private bool _playerStayingInCollision = false; //Flags if the player's remaining inside the item's collision
    private bool _playerInCollision = false; //Flags if the player is inside this item's collision to be interacted with

    void Start()
    {
        EventBus.Instance.Subscribe<Interact>(CheckIfItemIsPickedUp);
        EventBus.Instance.Subscribe<ActivatePlayerInputs>(AllowPlayerInput);
        EventBus.Instance.Subscribe<ResetWorldItemsActiveness>(ResetVisibilityOfGameObject);
    }

    void OnDestroy()
    {
        if (EventBus.Instance != null)
        {
            EventBus.Instance.Unsubscribe<Interact>(CheckIfItemIsPickedUp);
            EventBus.Instance.Unsubscribe<ActivatePlayerInputs>(AllowPlayerInput);
            EventBus.Instance.Unsubscribe<ResetWorldItemsActiveness>(ResetVisibilityOfGameObject);
        }
    }

    //Receives a "ActivatePlayerInputs" event with parameters:
    //(bool) AllowInputs - (true = allow the player to interact with world objects and UI, 
    //false = do NOT allow the player to interact with world objects and UI).
    private void AllowPlayerInput(ActivatePlayerInputs activatePlayerInputs) //Multiple publishers and subscribers
    {
        _allowInput = activatePlayerInputs.AllowInputs;
    }

    //"ResetWorldItemsActiveness" is the name of an event. Empty event
    private void ResetVisibilityOfGameObject(ResetWorldItemsActiveness resetWorldItemsActiveness) //Published by "MoonPuzzleDialogueText" or "StorytellingDialogueText"
    {
        InteractedByPlayerOnce = false;

        if(makeGameObjectInactive == true && gameObjectToSetInactive != null)
            gameObjectToSetInactive.SetActive(true);
    }

    //"Interact" is the name of an event. Empty event
    private void CheckIfItemIsPickedUp(Interact pickingUpItem) //When player interacts with this game object. Published by "PlayerInputController"
    {
        if(_allowInput == false || InteractedByPlayerOnce == true)
            return;

        if(_playerStayingInCollision == true && InteractedByPlayerOnce == false)
        {
            AudioManager.Instance.PlaySoundEffect(interactionSFX);

            //Pass item into inventory system
            ItemData clonedInventoryItem = inventoryItem.Clone();

            if(clonedInventoryItem.IsDroppedItem == true)
                clonedInventoryItem.IsDroppedItem = false;
                
            EventBus.Instance.Publish(new AddItemToInventory(clonedInventoryItem)); //Publish to "InventoryUI"

            InteractedByPlayerOnce = true;

            //Mark player no longer in collision with this interactable object
            _playerInCollision = false;
            EventBus.Instance.Publish(new PreventPlayerInteractingWithInventory(_playerInCollision)); //Publish to "InventoryUI"

            if(makeGameObjectInactive == true && gameObjectToSetInactive != null)
                gameObjectToSetInactive.SetActive(false);

            if(DeleteAfterInteraction == true && makeGameObjectInactive == false)
                Destroy(this.gameObject);
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
