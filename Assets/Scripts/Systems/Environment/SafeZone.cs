using UnityEngine;

/// <summary>
/// Manages the prompting of raising of the player's health 
/// </summary>
/// 
/// <remarks>
/// Safe Zones are the OnTrigger collisions that'll raise the player's health continuously.
/// This script works similarily to "FirstSafeZone" with both being collisions that'll raise the player's health continuously.
/// See <see cref="FirstSafeZone"/> for similarities and make sure they both work the same
/// 
/// This script works together with "PlayerHealth",  "MoonPuzzleDialogueText" scripts
/// See <see cref="PlayerHealth"/> - Publishing "AlterPlayerHealth" to raise the player's health
/// See <see cref="MoonPuzzleDialogueText"/> - Listening to "NewMoonFragmentObtained" event that "MoonPuzzleDialogueText" publishes to change safe zone 
/// areas in the same area and delete the current safe zone in the area
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

    //"NewMoonFragmentObtained" is the name of an event. Empty event
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
