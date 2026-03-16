using UnityEngine;

public class ChestInteraction : MonoBehaviour
{
    [SerializeField]
    private AudioClip openChestSFX;

    [SerializeField]
    private GameObject chestUI;

    private bool _playerStayingInCollision = false; 
    private bool _playerInCollision = false;

    void Start()
    {
        EventBus.Instance.Subscribe<Interact>(CheckForInteraction);
    }

    private void CheckForInteraction(Interact interact) //When player "interacts" with this game object (keybind E)
    {
        if(_playerStayingInCollision == true)
        {
            AudioManager.Instance.PlaySoundEffect(openChestSFX);
            chestUI.SetActive(true);
            EventBus.Instance.Publish(new ChestIsOpen(true));
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            _playerInCollision = true;
            EventBus.Instance.Publish(new InCollision(_playerInCollision));
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
            _playerStayingInCollision = true;
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            _playerInCollision = false;
            _playerStayingInCollision = false; 
            EventBus.Instance.Publish(new InCollision(_playerInCollision));
        }
    }
}
