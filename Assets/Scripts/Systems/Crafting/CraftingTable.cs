using UnityEngine;

public class CraftingTable : MonoBehaviour
{
    [SerializeField]
    private GameObject craftingUI;

    private bool _allowInput = true;

    private bool _playerStayingInCollision = false; 
    private bool _playerInCollision = false; 

    void Start()
    {
        EventBus.Instance.Subscribe<Interact>(OpenCraftingUI);
        EventBus.Instance.Subscribe<ActivatePlayerInputs>(AllowPlayerInput);
    }

    private void AllowPlayerInput(ActivatePlayerInputs activatePlayerInputs)
    {
        _allowInput = activatePlayerInputs.AllowInputs;
    }

    private void OpenCraftingUI(Interact interact) //When player "interacts" with this game object (keybind E)
    {
        if(_allowInput == false)
            return;

        if(_playerStayingInCollision == true)
        {
            craftingUI.SetActive(true);
            EventBus.Instance.Publish(new FreezePlayer(true));
            EventBus.Instance.Publish(new PauseExplorationTimer(true));
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
