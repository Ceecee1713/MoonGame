using UnityEngine;

public class MoonPuzzleArea : MonoBehaviour
{
    [SerializeField]
    private GameObject textAdventureUI;

    private bool _allowInput = true;
    private bool _playerCollisionDetected = false;
    private bool _interactedOnce = false;

    private const bool START_MOON_PUZZLE = true;
    private const bool START_PRAYER_PHASE = false;

    void Start()
    {
        EventBus.Instance.Subscribe<Interact>(OpenTextAdventureUI);
        EventBus.Instance.Subscribe<ActivatePlayerInputs>(AllowPlayerInput);
    } 

    private void AllowPlayerInput(ActivatePlayerInputs activatePlayerInputs)
    {
        _allowInput = activatePlayerInputs.AllowInputs;
    }

    private void OpenTextAdventureUI(Interact interact) //When player "interacts" with this game object (keybind E)
    {
        if(_interactedOnce == true || _allowInput == false)
            return;

        if(_playerCollisionDetected == true)
        {
            _interactedOnce = true;

            EventBus.Instance.Publish(new FreezePlayer(true));
            EventBus.Instance.Publish(new MaintainPlayerHealth(true));
            EventBus.Instance.Publish(new ChangeCanvases(textAdventureUI, START_MOON_PUZZLE, START_PRAYER_PHASE));
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
