using UnityEngine;

public class CorriosonZone : MonoBehaviour
{
    [SerializeField]
    private GameObject nextCorriosonZone;

    [SerializeField]
    [Range(1, 3)]
    private int moonPuzzleZoneIndex; //The moon puzzle area index to be completed in order to swap corrioson zones

    [SerializeField]
    private float CurrentSpeedToLowerHealth;
    [SerializeField]
    private float DefaultStartingSpeed;
    [SerializeField]
    private float speedForThirdCorriosonArea; //When two moon puzzles are complete ONLY

    [SerializeField]
    private bool isFirstCorriosonZoneInSecondArea = false; 
    [SerializeField]
    private bool lastMoonPuzzleCorriosonZone = false; 

    private int counter;

    private bool _playerCollisionDetected = false; 

    private const bool RECOVER_HEALTH = false;

    void Start()
    {
        CurrentSpeedToLowerHealth = DefaultStartingSpeed; 

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
            EventBus.Instance.Subscribe<ChangeThirdCorriosonAreaValue>(SetThirdCorriosonZoneSpeed);
        }
    }

    private void SetNewCorriosonZone(NewMoonFragmentObtained newMoonFragmentObtained)
    {
        if(lastMoonPuzzleCorriosonZone == true)
            return;

        if(nextCorriosonZone != null)
        {
            counter++;

            if(counter == moonPuzzleZoneIndex)
            {
                nextCorriosonZone.SetActive(true);
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player") && _playerCollisionDetected == false)
        {
            EventBus.Instance.Publish(new AlterPlayerHealth(RECOVER_HEALTH, CurrentSpeedToLowerHealth));
            _playerCollisionDetected = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
            _playerCollisionDetected = false; 
    }

    private void ChangeSpeedToLowerHealth(ChangeCorriosonValue changeCorriosonValue) //Published by StorytellingDialogueText and ExplorationTimer
    {
        if(moonPuzzleZoneIndex != changeCorriosonValue.CorriosonAreaNumber || lastMoonPuzzleCorriosonZone == true)
            return;
        
        if(isFirstCorriosonZoneInSecondArea == true)
            return;

        CurrentSpeedToLowerHealth = changeCorriosonValue.CorriosonValue;
    }

    private void ReturnToDefaultSpeed(RestoreCorriosonValue restoreCorriosonValue)
    {
        if(lastMoonPuzzleCorriosonZone == true || isFirstCorriosonZoneInSecondArea == true)
            return;

        CurrentSpeedToLowerHealth = DefaultStartingSpeed;
    }

    private void SetThirdCorriosonZoneSpeed(ChangeThirdCorriosonAreaValue changeThirdCorriosonAreaValue)
    {
        if(lastMoonPuzzleCorriosonZone == false)
            return;

        DefaultStartingSpeed = speedForThirdCorriosonArea;
        CurrentSpeedToLowerHealth = speedForThirdCorriosonArea;
        lastMoonPuzzleCorriosonZone = false; //Allow third corrioson zone to now have value changes like other corrioson zones
    }
}
