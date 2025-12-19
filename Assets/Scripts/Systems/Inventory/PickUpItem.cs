using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    [SerializeField]
    public ItemData inventoryItem; 

    [Header ("Game Object's Visibility")]
    [SerializeField]
    private GameObject gameObjectToSetInactive;
    [SerializeField]
    private bool makeGameObjectInactive = false;
    [SerializeField]
    private bool deleteAfterInteraction = false;

    private bool _interactedWithOne = false;
    private bool _allowInput = true;
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
        _interactedWithOne = false;

        if(makeGameObjectInactive == true && gameObjectToSetInactive != null)
            gameObjectToSetInactive.SetActive(true);
    }

    private void CheckIfItemIsPickedUp(Interact pickingUpItem) //When player "interacts" with this game object (keybind E)
    {
        if(_allowInput == false)
            return;

        if(_playerStayingInCollision == true && _interactedWithOne == false)
        {
            //Pass item into inventory system
            ItemData clonedInventoryItem = inventoryItem.Clone();
            EventBus.Instance.Publish(new AddItemToInventory(clonedInventoryItem));

            _interactedWithOne = true;

            if(makeGameObjectInactive == true && gameObjectToSetInactive != null)
                gameObjectToSetInactive.SetActive(false);

            if(deleteAfterInteraction == true && makeGameObjectInactive == false)
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
        {
            _playerStayingInCollision = true;
        }
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
