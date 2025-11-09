using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    [SerializeField]
    private ItemData inventoryItem; //Refactor

    private bool _playerCollisionDetected = false; 

    void Start()
    {
        EventBus.Instance.Subscribe<Interact>(CheckIfItemIsPickedUp);
    }

    private void CheckIfItemIsPickedUp(Interact pickingUpItem) //When player "interacts" with this game object (keybind for interact)
    {
        if(_playerCollisionDetected == true)
        {
            ItemData clonedInventoryItem = inventoryItem.Clone();
            EventBus.Instance.Publish(new CheckToAddInventoryItem(clonedInventoryItem));
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

/*
inventoryitem.itemSlotImageSprite; 
inventoryitem.NameOfItem; 
inventoryitem.ItemType; 
inventoryitem.ItemObject; 

inventoryitem.Quantity; 
inventoryitem.IsThisAStackableItem;
*/
