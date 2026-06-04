using UnityEngine;

/// <summary>
/// Manages the speed to lower the player's health based on OnTrigger collisions with the player
/// </summary>
/// 
/// <remarks>
/// Corrioson Zones are the OnTrigger collisions that'll drop the player's health continuously 
/// 
/// This script works together with "PlayerHealth", "StorytellingDialogueText", "GameManager", "ExplorationTimer", "MoonPuzzleDialogueText" scripts
/// See <see cref="PlayerHealth"/> - prompting to change the speed of lowering player's health
/// See <see cref="StorytellingDialogueText"/> - prompting the "RestoreCorriosonValue" and "ChangeCorriosonValue" events this script listens to
/// See <see cref="GameManager"/> - prompting the "ChangeThirdCorriosonAreaValue" event this script listens to
/// See <see cref="ExplorationTimer"/> - prompting the "ChangeCorriosonValue" event this script listens to
/// See <see cref="MoonPuzzleDialogueText"/> - prompting the "ChangeCorriosonValue" event this script listens to
/// 
/// </remarks>

public class CorriosonZone : MonoBehaviour
{
    [SerializeField]
    private GameObject nextCorriosonZone; 
    //Game object that'll have a larger OnTrigger collision for the same area when a new moon puzzle is completed.
    //Game object should remain inactive as this script sets it active during runtime

    [SerializeField]
    [Range(1, 3)]
    private int moonPuzzleZoneIndex; //The moon puzzle area to be completed in order to swap corrioson zones

    [SerializeField]
    private float CurrentSpeedToLowerHealth;
    [SerializeField]
    private float DefaultSpeedToLowerHealth;
    [SerializeField]
    private float speedToLowerHealthForThirdCorriosonArea; //Only used for the third collision area, flagged by "lastMoonPuzzleCorriosonZone"

    [SerializeField]
    private bool isFirstCorriosonZoneInSecondArea = false;
    [SerializeField]
    private bool lastMoonPuzzleCorriosonZone = false; 

    private int _numberOfMoonPuzzlesSolved; 

    private bool _playerCollisionDetected = false; //Flag whether the player is inside the collision to lower their health

    private const bool RECOVER_HEALTH = false;

    void Start()
    {
        CurrentSpeedToLowerHealth = DefaultSpeedToLowerHealth; 

        EventBus.Instance.Subscribe<ChangeCorriosonValue>(ChangeSpeedToLowerHealth);
        EventBus.Instance.Subscribe<RestoreCorriosonValue>(ReturnToDefaultSpeed);
        EventBus.Instance.Subscribe<NewMoonFragmentObtained>(SetNewCorriosonZone);
        EventBus.Instance.Subscribe<ChangeThirdCorriosonAreaValue>(SetThirdCorriosonZoneSpeed);
    }

    void OnDestroy()
    {
        if (EventBus.Instance != null)
        {
            EventBus.Instance.Unsubscribe<ChangeCorriosonValue>(ChangeSpeedToLowerHealth);
            EventBus.Instance.Unsubscribe<RestoreCorriosonValue>(ReturnToDefaultSpeed);
            EventBus.Instance.Unsubscribe<NewMoonFragmentObtained>(SetNewCorriosonZone);
            EventBus.Instance.Unsubscribe<ChangeThirdCorriosonAreaValue>(SetThirdCorriosonZoneSpeed);
        }
    }

    //Destroy the Game Object attached to this script and set the replacement corrison zone for the same area active
    private void SetNewCorriosonZone(NewMoonFragmentObtained newMoonFragmentObtained) //Published by "MoonPuzzleDialogueText"
    {
        if(lastMoonPuzzleCorriosonZone == true)
            return;

        if(nextCorriosonZone != null)
        {
            _numberOfMoonPuzzlesSolved++;

            if(_numberOfMoonPuzzlesSolved == moonPuzzleZoneIndex)
            {
                nextCorriosonZone.SetActive(true);
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider collider) //Lower Player Health
    {
        if (collider.gameObject.CompareTag("Player") && _playerCollisionDetected == false)
        {
            EventBus.Instance.Publish(new AlterPlayerHealth(RECOVER_HEALTH, CurrentSpeedToLowerHealth)); //Publish to "PlayerHealth"
            _playerCollisionDetected = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
            _playerCollisionDetected = false; 
    }

    private void ChangeSpeedToLowerHealth(ChangeCorriosonValue changeCorriosonValue) //Published by "StorytellingDialogueText" and/or "ExplorationTimer"
    {
        if(moonPuzzleZoneIndex != changeCorriosonValue.CorriosonAreaNumber || lastMoonPuzzleCorriosonZone == true)
            return;
        
        if(isFirstCorriosonZoneInSecondArea == true)
            return;

        CurrentSpeedToLowerHealth = changeCorriosonValue.CorriosonValue;
    }

    private void ReturnToDefaultSpeed(RestoreCorriosonValue restoreCorriosonValue) //Published by "StorytellingDialogueText"
    {
        if(lastMoonPuzzleCorriosonZone == true || isFirstCorriosonZoneInSecondArea == true)
            return;

        CurrentSpeedToLowerHealth = DefaultSpeedToLowerHealth;
    }

    private void SetThirdCorriosonZoneSpeed(ChangeThirdCorriosonAreaValue changeThirdCorriosonAreaValue) //Published by "GameManager"
    {
        if(lastMoonPuzzleCorriosonZone == false)
            return;

        DefaultSpeedToLowerHealth = speedToLowerHealthForThirdCorriosonArea;
        CurrentSpeedToLowerHealth = speedToLowerHealthForThirdCorriosonArea;
        lastMoonPuzzleCorriosonZone = false; //Allow third corrioson zone to now have value changes like other corrioson zones
    }
}
