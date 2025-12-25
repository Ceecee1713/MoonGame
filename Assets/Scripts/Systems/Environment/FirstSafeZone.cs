using UnityEngine;

public class FirstSafeZone : MonoBehaviour
{
    [SerializeField]
    private GameObject newSafeZoneArea;

    [Header ("For Tutorial Dialogue Message")]
    [SerializeField]
    private GameObject dialogueUI;
    [SerializeField]
    private StorytellingDialogueData corriosonZoneTutorial;

    [SerializeField]
    private float speedToIncraseHealth = 1.2f;

    private bool _showMoonTutorial = false;
    private bool _recoverHealth = true;
    private bool _playerCollisionDetected = false; 

    private const bool NEW_EXPLORATION_PHASE = false;
    private const bool STARTING_THE_GAME = false; 

    private const float DELAY = 0.5f;

    void Start()
    {
        EventBus.Instance.Subscribe<NewMoonFragmentObtained>(SetNewSafeZoneCollision);
    }

    void OnDestroy()
    {
        if (EventBus.Instance != null)
            EventBus.Instance.Unsubscribe<NewMoonFragmentObtained>(SetNewSafeZoneCollision);
    }

    private void SetNewSafeZoneCollision(NewMoonFragmentObtained newMoonFragmentObtained)
    {
        newSafeZoneArea.SetActive(true);
        Destroy(this.gameObject);
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player") && _playerCollisionDetected == false)
        {
            EventBus.Instance.Publish(new AlterPlayerHealth(_recoverHealth, speedToIncraseHealth));
            _playerCollisionDetected = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            _playerCollisionDetected = false; 

            if(_showMoonTutorial == false)
            {
                _showMoonTutorial = true;
                Invoke("ShowTutorial", DELAY);
                return;
            }
        }
    }

    private void ShowTutorial()
    {
        dialogueUI.SetActive(true);
        EventBus.Instance.Publish(new FreezePlayer(true));
        EventBus.Instance.Publish(new TypeDialogueOnMainUI(corriosonZoneTutorial, NEW_EXPLORATION_PHASE, STARTING_THE_GAME));
    }
}
