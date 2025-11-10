using UnityEngine;

public class SafeZone : MonoBehaviour
{
    [SerializeField]
    private float speedToIncraseHealth = 1.2f;

    private bool _recoverHealth = true;
    private bool _playerCollisionDetected = false; 

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
