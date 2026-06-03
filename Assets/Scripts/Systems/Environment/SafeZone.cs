using UnityEngine;

/// <summary>
/// Manages the prompting of raising of the player's health 
/// </summary>
/// 
/// <remarks>
/// Safe Zones are the OnTrigger collisions that'll raise the player's health continuously.
/// 
/// This script works together with "PlayerHealth" and "MoonPuzzleDialogueText" scripts
/// See <see cref="PlayerHealth"/> for how they work together - prompting to raise the player's health
/// See <see cref="MoonPuzzleDialogueText"/> for how they work together - publishing the "NewMoonFragmentObtained" event this script listens to
/// 
/// </remarks>

public class SafeZone : MonoBehaviour
{
    [SerializeField]
    private GameObject nextSafeZoneArea;
    //Game object that'll have a larger OnTrigger collision for the same area when a new moon puzzle is completed.
    //Game object should remain inactive as this script sets it active during runtime
    
    [SerializeField]
    private float speedToIncraseHealth = 1.2f;

    private bool _playerCollisionDetected = false; //Flag whether the player is inside the collision to raise their health

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

    //Destroy the Game Object attached to this script and set the replacement safe zone for the same area active
    private void SetNewSafeZoneCollision(NewMoonFragmentObtained newMoonFragmentObtained) //Published by "MoonPuzzleDialogueText"
    {
        if(nextSafeZoneArea != null)
        {
            nextSafeZoneArea.SetActive(true);
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerStay(Collider collider) //Raise Player Health
    {
        if (collider.gameObject.CompareTag("Player") && _playerCollisionDetected == false)
        {
            EventBus.Instance.Publish(new AlterPlayerHealth(RECOVER_HEALTH, speedToIncraseHealth)); //Publish to "PlayerHealth"
            _playerCollisionDetected = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
            _playerCollisionDetected = false; 
    }
}
