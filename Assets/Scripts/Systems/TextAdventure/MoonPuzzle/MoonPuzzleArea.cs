using UnityEngine;

public class MoonPuzzleArea : MonoBehaviour
{
    [SerializeField]
    private GameObject textAdventureUI;

    private bool _startMoonPuzzle = true;
    private bool _startPrayerPhase = false;
    private bool _playerCollisionDetected = false;
    private bool _allowInput = false;
    private bool _interactedOnce = false;

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
            EventBus.Instance.Publish(new ChangeCanvases(textAdventureUI, _startMoonPuzzle, _startPrayerPhase));
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
