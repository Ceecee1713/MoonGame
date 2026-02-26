using UnityEngine;

public class SafeZone : MonoBehaviour
{
    [SerializeField]
    private GameObject newSafeZoneArea;
    
    [SerializeField]
    private float speedToIncraseHealth = 1.2f;

    private bool _recoverHealth = true;
    private bool _playerCollisionDetected = false; 

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
        if(newSafeZoneArea != null)
            newSafeZoneArea.SetActive(true);
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
        }
    }
}
