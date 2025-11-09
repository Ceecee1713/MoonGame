using UnityEngine;

public class MoonPuzzleArea : MonoBehaviour
{
    [SerializeField]
    private GameObject textAdventureUI;

    private bool _textAdventure = true;
    private bool _solvedMoonPuzzle = false;
    private bool _playerCollisionDetected = false;

    void Start()
    {
        EventBus.Instance.Subscribe<Interact>(OpenTextAdventureUI);
    } 

    private void OpenTextAdventureUI(Interact interact) //When player "interacts" with this game object (keybind for interact)
    {
        if(_playerCollisionDetected == true)
        {
            EventBus.Instance.Publish(new FreezePlayer(true));
            EventBus.Instance.Publish(new ChangeCanvases(textAdventureUI, _solvedMoonPuzzle, _textAdventure));
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
