using UnityEngine;

/// <summary>
/// Manages the player's interaction with making the chest UI appear and pausing the game
/// </summary>
/// 
/// <remarks>
/// This script works together with the "PlayerInputController", "InventoryUISlot", "OpenCluebookMainUI" scripts
/// 
/// See <see cref="PlayerInputController"/> how they work together - prompting the "Interact" event this script listens to
/// See <see cref="InventoryUISlot"/> how they work together - prompting the "ChestIsOpen" event this script to the "InventoryUISlot" script
/// See <see cref="OpenCluebookMainUI"/> how they work together - stopping the cluebook button on the main player UI from being interactable temporarily
/// 
/// </remarks>

public class ChestInteraction : MonoBehaviour
{
    [SerializeField]
    private AudioClip openChestSFX;

    [SerializeField]
    private GameObject chestUI;

    private bool _playerStayingInCollision = false; //Prevent or allow pausing the game and freezing the player in place
    private bool _playerInCollision = false; //Flag whether the player can interact with the chest or not: if they're in range or not

    void Start()
    {
        EventBus.Instance.Subscribe<Interact>(CheckForInteraction);
    }

    private void CheckForInteraction(Interact interact) //When player interacts with this game object. Published by "PlayerInputController"
    {
        if(_playerStayingInCollision == true)
        {
            AudioManager.Instance.PlaySoundEffect(openChestSFX);
            chestUI.SetActive(true);
            EventBus.Instance.Publish(new ChestIsOpen(true)); //Publish to "InventoryUISlot" and "OpenCluebookMainUI"
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
