using UnityEngine;

public class CraftingTable : MonoBehaviour
{
    [SerializeField]
    private GameObject craftingUI;

    private bool _playerStayingInCollision = false; 
    private bool _playerInCollision = false; 

    void Start()
    {
        EventBus.Instance.Subscribe<Interact>(OpenCraftingUI);
    }

    private void OpenCraftingUI(Interact interact) //When player "interacts" with this game object (keybind for interact)
    {
        if(_playerStayingInCollision == true)
        {
            craftingUI.SetActive(true);
            EventBus.Instance.Publish(new FreezePlayer(true));
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
