using UnityEngine;

public class SafeZone : MonoBehaviour
{
    [SerializeField]
    private GameObject nextSafeZoneArea;
    
    [SerializeField]
    private float speedToIncraseHealth = 1.2f;

    private bool _playerCollisionDetected = false; 

    private const bool RECOVER_HEALTH = true;

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
        if(nextSafeZoneArea != null)
        {
            nextSafeZoneArea.SetActive(true);
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player") && _playerCollisionDetected == false)
        {
            EventBus.Instance.Publish(new AlterPlayerHealth(RECOVER_HEALTH, speedToIncraseHealth));
            _playerCollisionDetected = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
            _playerCollisionDetected = false; 
    }
}
