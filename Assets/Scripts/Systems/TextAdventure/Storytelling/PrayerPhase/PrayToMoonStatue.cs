using UnityEngine;

public class PrayToMoonStatue : MonoBehaviour
{
    [SerializeField]
    private GameObject storytellingUI;

    private bool _startMoonPuzzle = false;
    private bool _startPrayerPhase = true;
    private bool _playerCollisionDetected = false;

    void Start()
    {
        EventBus.Instance.Subscribe<Interact>(OpenStorytellingUI);
    } 

    private void OpenStorytellingUI(Interact interact) //When player "interacts" with this game object (keybind E)
    {
        if(_playerCollisionDetected == true)
        {
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
        }
    }
}
