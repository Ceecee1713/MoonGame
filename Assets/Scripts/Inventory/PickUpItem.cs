using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    [SerializeField]
    private InventoryItem inventoryItem;

    private bool _playerCollisionDetected = false; 

    void Start()
    {
        EventBus.Instance.Subscribe<Interact>(CheckIfItemIsPickedUp);
    }

    private void CheckIfItemIsPickedUp(Interact pickingUpItem) //When player "interacts" with this game object (keybind for interact)
    {
        if(_playerCollisionDetected == true)
        {
            EventBus.Instance.Publish(new CheckToAddInventoryItem(inventoryItem));
            Debug.Log("I've been picked up by the player");
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            _playerCollisionDetected = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            _playerCollisionDetected = false; 
        }
    }
}
