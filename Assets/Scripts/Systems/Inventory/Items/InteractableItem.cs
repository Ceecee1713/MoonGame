using UnityEngine;

public class InteractableItem : MonoBehaviour
{
    [SerializeField]
    private AudioClip interactionSFX;

    [SerializeField]
    public ItemData inventoryItem; 

    [Header ("Game Object's Visibility - Environment")]
    public GameObject gameObjectToSetInactive;
    [SerializeField]
    private bool makeGameObjectInactive = false;

    [HideInInspector]
    public bool DeleteAfterInteraction = false; //Accessed by PlayerSpawner

    private bool _allowInput = true;
    private bool _interactedByPlayerOnce = false;
    private bool _playerStayingInCollision = false; 
    private bool _playerInCollision = false;

    void Start()
    {
        EventBus.Instance.Subscribe<Interact>(CheckIfItemIsPickedUp);
        EventBus.Instance.Subscribe<ActivatePlayerInputs>(AllowPlayerInput);
        EventBus.Instance.Subscribe<ResetWorldItems>(ResetVisibilityOfGameObject);
    }

    void OnDestroy()
    {
        if (EventBus.Instance != null)
        {
            EventBus.Instance.Unsubscribe<Interact>(CheckIfItemIsPickedUp);
            EventBus.Instance.Unsubscribe<ActivatePlayerInputs>(AllowPlayerInput);
            EventBus.Instance.Unsubscribe<ResetWorldItems>(ResetVisibilityOfGameObject);
        }
    }

    private void AllowPlayerInput(ActivatePlayerInputs activatePlayerInputs)
    {
        _allowInput = activatePlayerInputs.AllowInputs;
    }

    private void ResetVisibilityOfGameObject(ResetWorldItems resetWorldItems)
    {
        _interactedByPlayerOnce = false;

        if(makeGameObjectInactive == true && gameObjectToSetInactive != null)
            gameObjectToSetInactive.SetActive(true);
    }

    private void CheckIfItemIsPickedUp(Interact pickingUpItem) //When player "interacts" with this game object (keybind E)
    {
        if(_allowInput == false || _interactedByPlayerOnce == true)
            return;

        if(_playerStayingInCollision == true && _interactedByPlayerOnce == false)
        {
            AudioManager.Instance.PlaySoundEffect(interactionSFX);

            //Pass item into inventory system
            ItemData clonedInventoryItem = inventoryItem.Clone();

            if(clonedInventoryItem.IsDroppedItem == true)
                clonedInventoryItem.IsDroppedItem = false;
                
            EventBus.Instance.Publish(new AddItemToInventory(clonedInventoryItem)); 

            _interactedByPlayerOnce = true;

            //Mark player no longer in collision with this interactable object
            _playerInCollision = false;
            EventBus.Instance.Publish(new InCollision(_playerInCollision));

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
