using UnityEngine;

public class CorriosonZone : MonoBehaviour
{
    [SerializeField]
    private CorriosonValues corriosonValues;

    [SerializeField]
    private GameObject nextCorriosonZone;

    [SerializeField]
    [Range(1, 3)]
    private int moonPuzzleZoneIndex; //The moon puzzle area index to be completed in order to swap corrioson zones

    [SerializeField]
    private float CurrentSpeedToLowerHealth;

    private int counter;

    private bool _playerCollisionDetected = false; 

    private const bool RECOVER_HEALTH = false;

    void Start()
    {
        CurrentSpeedToLowerHealth = corriosonValues.DefaultStartingSpeed; 

        EventBus.Instance.Subscribe<ChangeCorriosonValue>(ChangeSpeedToLowerHealth);
        EventBus.Instance.Subscribe<RestoreCorriosonValue>(ReturnToDefaultSpeed);
        EventBus.Instance.Subscribe<NewMoonFragmentObtained>(SetNewCorriosonZone);
    }

    void OnDestroy()
    {
        if (EventBus.Instance != null)
        {
            EventBus.Instance.Unsubscribe<ChangeCorriosonValue>(ChangeSpeedToLowerHealth);
            EventBus.Instance.Unsubscribe<RestoreCorriosonValue>(ReturnToDefaultSpeed);
            EventBus.Instance.Unsubscribe<NewMoonFragmentObtained>(SetNewCorriosonZone);
        }
    }

    private void SetNewCorriosonZone(NewMoonFragmentObtained newMoonFragmentObtained)
    {
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
        if(moonPuzzleZoneIndex != changeCorriosonValue.CorriosonAreaNumber)
            return;

        CurrentSpeedToLowerHealth = changeCorriosonValue.CorriosonValue;
    }

    private void ReturnToDefaultSpeed(RestoreCorriosonValue restoreCorriosonValue)
    {
        CurrentSpeedToLowerHealth = corriosonValues.DefaultStartingSpeed; 
    }
}
