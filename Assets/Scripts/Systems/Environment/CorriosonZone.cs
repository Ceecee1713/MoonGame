using UnityEngine;

public class CorriosonZone : MonoBehaviour
{
    [SerializeField]
    private float speedToLowerHealth = 1.7f;

    private bool _recoverHealth = false;
    private bool _playerCollisionDetected = false; 

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player") && _playerCollisionDetected == false)
        {
            EventBus.Instance.Publish(new AlterPlayerHealth(_recoverHealth, speedToLowerHealth));
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
