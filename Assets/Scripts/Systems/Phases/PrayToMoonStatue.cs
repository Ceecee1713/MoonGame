using UnityEngine;

public class PrayToMoonStatue : MonoBehaviour
{
    [SerializeField]
    private GameObject storytellingUI;

    private bool _startMoonPuzzle = false;
    private bool _startPrayerPhase = true;
    private bool _playerCollisionDetected = false;
    private bool _interactedOnce = false;
    private bool _allowInput = false;

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
            _interactedOnce = true;

            EventBus.Instance.Publish(new FreezePlayer(true));
            EventBus.Instance.Publish(new ChangeCanvases(storytellingUI, _startMoonPuzzle, _startPrayerPhase));
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
            _interactedOnce = false;
        }
    }
}
