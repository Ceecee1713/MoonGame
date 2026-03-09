using UnityEngine;

public class PrayToMoonStatue : MonoBehaviour
{
    [Header ("UI Information")]
    [SerializeField]
    private GameObject storytellingUI;
    [SerializeField]
    private GameObject dialogueUI;

    [SerializeField]
    private StorytellingDialogueData moonStatueTutorialDialogue;

    private bool _showMoonTutorial = false;
    private bool _allowInput = true;
    private bool _playerCollisionDetected = false;
    private bool _interactedOnce = false;

    private const bool START_PRAYER_PHASE = true;
    private const bool START_MOON_PUZZLE = false;
    private const bool NEW_EXPLORATION_PHASE = false;
    private const bool STARTING_THE_GAME = false; 
    
    void Start()
    {
        EventBus.Instance.Subscribe<Interact>(OpenStorytellingUI);
        EventBus.Instance.Subscribe<ActivatePlayerInputs>(AllowPlayerInput);
    } 

    private void AllowPlayerInput(ActivatePlayerInputs activatePlayerInputs)
    {
        _allowInput = activatePlayerInputs.AllowInputs;
    }

    private void OpenStorytellingUI(Interact interact) //When player "interacts" with this game object (keybind E)
    {
        if(_interactedOnce == true || _allowInput == false)
            return;

        if(_playerCollisionDetected == true)
        {
            if(_showMoonTutorial == false) //Show moon tutorial dialogue
            {
                _showMoonTutorial = true;
                dialogueUI.SetActive(true);
                
                EventBus.Instance.Publish(new FreezePlayer(true));
                EventBus.Instance.Publish(new TypeDialogueOnMainUI(moonStatueTutorialDialogue, NEW_EXPLORATION_PHASE, STARTING_THE_GAME));
                return;
            }

            _interactedOnce = true;
            EventBus.Instance.Publish(new FreezePlayer(true));
            EventBus.Instance.Publish(new ChangeCanvases(storytellingUI, START_MOON_PUZZLE, START_PRAYER_PHASE));
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
            _playerCollisionDetected = true;
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            _playerCollisionDetected = false; 
            _interactedOnce = false;
        }
    }
}
