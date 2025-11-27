using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    [SerializeField]
    private ItemData inventoryItem; 

    private bool _playerStayingInCollision = false; 
    private bool _playerInCollision = false;
    private bool _allowInput = false;

    void Start()
    {
        EventBus.Instance.Subscribe<Interact>(CheckIfItemIsPickedUp);
        EventBus.Instance.Subscribe<ActivatePlayerInputs>(AllowPlayerInput);
    }

    private void AllowPlayerInput(ActivatePlayerInputs activatePlayerInputs)
    {
        _allowInput = activatePlayerInputs.AllowInputs;
    }

    private void CheckIfItemIsPickedUp(Interact pickingUpItem) //When player "interacts" with this game object (keybind E)
    {
        if(_allowInput == false)
            return;

        if(_playerStayingInCollision == true)
        {
            ItemData clonedInventoryItem = inventoryItem.Clone();
            EventBus.Instance.Publish(new AddItemToInventory(clonedInventoryItem));
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
