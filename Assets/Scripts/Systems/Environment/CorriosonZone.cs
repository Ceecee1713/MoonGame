using UnityEngine;

public class CorriosonZone : MonoBehaviour
{
    [SerializeField]
    private CorriosonValues corriosonValues;

    [SerializeField]
    private GameObject otherAreaCollision;

    [SerializeField]
    [Range(1, 3)]
    private int areaNumber;

    public float DefaultSpeedToLowerHealth;
    public float CurrentSpeedToLowerHealth;

    [HideInInspector]
    public int counter;

    private bool _recoverHealth = false;
    private bool _playerCollisionDetected = false; 

    void Start()
    {
        CurrentSpeedToLowerHealth = DefaultSpeedToLowerHealth; 

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
        counter++;

        if(counter == areaNumber && otherAreaCollision != null)
        {
            otherAreaCollision.SetActive(true);
            CorriosonZone corriosonScript = otherAreaCollision.GetComponent<CorriosonZone>();
            corriosonScript.DefaultSpeedToLowerHealth = DefaultSpeedToLowerHealth;
            corriosonScript.CurrentSpeedToLowerHealth = corriosonScript.DefaultSpeedToLowerHealth;
            corriosonScript.counter = counter;

            Destroy(this.gameObject);
        }
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player") && _playerCollisionDetected == false)
        {
            EventBus.Instance.Publish(new AlterPlayerHealth(_recoverHealth, CurrentSpeedToLowerHealth));
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

        CurrentSpeedToLowerHealth = changeCorriosonValue.CorriosonValue;
    }

    private void ReturnToDefaultSpeed(RestoreCorriosonValue restoreCorriosonValue)
    {
        CurrentSpeedToLowerHealth = DefaultSpeedToLowerHealth;
    }
}
