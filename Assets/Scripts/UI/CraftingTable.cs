using UnityEngine;

public class CraftingTable : MonoBehaviour
{
    [SerializeField]
    private GameObject craftingUI;

    private bool _playerCollisionDetected = false; 

    void Start()
    {
        EventBus.Instance.Subscribe<Interact>(OpenCraftingUI);
    }

    private void OpenCraftingUI(Interact interact) //When player "interacts" with this game object (keybind for interact)
    {
        if(_playerCollisionDetected == true)
        {
            craftingUI.SetActive(true);
            //Add freezing player event and other things
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
