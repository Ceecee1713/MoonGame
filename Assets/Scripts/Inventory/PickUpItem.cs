using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    private bool _playerCollisionDetected = false; 

    void Update()
    {
        if(_playerCollisionDetected == true)
        {
            Debug.Log("I've been collided by the player");
        }
    }

    private void OnTriggerEnter(Collider collider)
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
