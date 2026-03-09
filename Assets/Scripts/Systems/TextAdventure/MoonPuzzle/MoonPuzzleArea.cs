using UnityEngine;

public class MoonPuzzleArea : MonoBehaviour
{
    [SerializeField]
    private GameObject warningMoonPuzzleUI;
    [SerializeField]
    private WarningMoonPuzzleUI warningMoonPuzzleUIScript;

    [HideInInspector]
    public bool InteractedOnce = false; //Influenced by "WarningMoonPuzzleUI" script

    private bool _allowInput = true;
    private bool _playerCollisionDetected = false;

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
        if(InteractedOnce == true || _allowInput == false)
            return;

        if(_playerCollisionDetected == true)
        {
            EventBus.Instance.Publish(new FreezePlayer(true));
            EventBus.Instance.Publish(new MaintainPlayerHealth(true));
            EventBus.Instance.Publish(new PauseExplorationTimer(true));
            
            warningMoonPuzzleUI.SetActive(true);
            warningMoonPuzzleUIScript.ShowWarningMessage(this);
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
            _playerCollisionDetected = false; 
    }
}
