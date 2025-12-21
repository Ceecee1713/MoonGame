using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField]
    private GameObject dialogueCanvas;

    [SerializeField]
    private StorytellingDialogueData npcDialogue;

    private string _npcMessage;

    private bool _allowInput = true;
    private bool _playerStayingInCollision = false; 
    private bool _playerInCollision = false;

    private const bool NEW_EXPLORATION_PHASE = false;
    private const bool STARTING_THE_GAME = false; 

    void Start()
    {
        _npcMessage = npcDialogue.Messages[0];

        EventBus.Instance.Subscribe<Interact>(CheckToShowDialogue);
        EventBus.Instance.Subscribe<ActivatePlayerInputs>(AllowPlayerInput);
    }

    private void AllowPlayerInput(ActivatePlayerInputs activatePlayerInputs)
    {
        _allowInput = activatePlayerInputs.AllowInputs;
    }

    private void CheckToShowDialogue(Interact pickingUpItem) //When player "interacts" with this game object (keybind E)
    {
        if(_allowInput == false)
            return;

        if(_playerStayingInCollision == true)
        {
            dialogueCanvas.SetActive(true);

            EventBus.Instance.Publish(new FreezePlayer(true));
            EventBus.Instance.Publish(new MaintainPlayerHealth(true));
            EventBus.Instance.Publish(new TypeDialogueOnMainUI(npcDialogue, NEW_EXPLORATION_PHASE, STARTING_THE_GAME));
            EventBus.Instance.Publish(new FoundClueFragment(_npcMessage));
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
