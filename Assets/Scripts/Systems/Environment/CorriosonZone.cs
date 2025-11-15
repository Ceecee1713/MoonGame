using UnityEngine;

public class CorriosonZone : MonoBehaviour
{
    [SerializeField]
    private CorriosonValues corriosonValues;

    [SerializeField]
    [Range(1, 3)]
    private int areaNumber;

    [SerializeField]
    private float startingSpeedToLowerHealth = 1.0f;

    private float _speedToLowerHealth;
    private float _defaultSpeedToLowerHealth;

    private bool _recoverHealth = false;
    private bool _playerCollisionDetected = false; 

    void Start()
    {
        _defaultSpeedToLowerHealth = startingSpeedToLowerHealth; 
        _speedToLowerHealth = _defaultSpeedToLowerHealth; 

        EventBus.Instance.Subscribe<ChangeCorriosonValue>(ChangeSpeedToLowerHealth);
        EventBus.Instance.Subscribe<RestoreCorriosonValue>(ReturnToDefaultSpeed);
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player") && _playerCollisionDetected == false)
        {
            EventBus.Instance.Publish(new AlterPlayerHealth(_recoverHealth, _speedToLowerHealth));
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

    private void ChangeSpeedToLowerHealth(ChangeCorriosonValue changeCorriosonValue)
    {
        if(areaNumber != changeCorriosonValue.CorriosonAreaNumber)
            return;

        if(changeCorriosonValue.CorriosonValue == corriosonValues.SpeedToLowerHealthWhenAreaIsCleared)
            _defaultSpeedToLowerHealth = changeCorriosonValue.CorriosonValue;

        _speedToLowerHealth = changeCorriosonValue.CorriosonValue;
    }

    private void ReturnToDefaultSpeed(RestoreCorriosonValue restoreCorriosonValue)
    {
        _speedToLowerHealth = _defaultSpeedToLowerHealth;
    }
}
