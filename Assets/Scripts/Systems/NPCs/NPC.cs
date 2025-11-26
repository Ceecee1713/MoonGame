using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField]
    private GameObject dialogueCanvas;

    [SerializeField]
    private Dialogue npcMessage;

    private string _extraMessage = "... (You can't understand what they're saying, but you write down their words anyways).";
    private string _fullNPCMesasge;

    private bool _playerStayingInCollision = false; 
    private bool _playerInCollision = false;

    private const bool NEW_EXPLORATION_PHASE = false;

    void Start()
    {
        _fullNPCMesasge = npcMessage.Message + _extraMessage;
        EventBus.Instance.Subscribe<Interact>(CheckToShowDialogue);
    }

    private void CheckToShowDialogue(Interact pickingUpItem) //When player "interacts" with this game object (keybind E)
    {
        if(_playerStayingInCollision == true)
        {
            dialogueCanvas.SetActive(true);

            EventBus.Instance.Publish(new FreezePlayer(true));
            EventBus.Instance.Publish(new MaintainPlayerHealth(true));
            EventBus.Instance.Publish(new TypeOutSingleDialogue(_fullNPCMesasge, NEW_EXPLORATION_PHASE));
            EventBus.Instance.Publish(new FoundClueFragment(npcMessage));
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
