using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private Slider health;

    private bool _pauseCorrioson = false;
    private bool _recoverHealth = false;
    private float _speedToChangeHealth;

    void Start()
    {
        EventBus.Instance.Subscribe<AlterPlayerHealth>(ChangeHealthValue);
        EventBus.Instance.Subscribe<MaintainPlayerHealth>(ApplyCorrioson);
    }

    void Update()
    {
        if( _pauseCorrioson == true)
            return;

        if(_recoverHealth == false && health.value != 0.0f)
        {
            health.value -= Time.deltaTime * _speedToChangeHealth;
        }

        if(_recoverHealth == true && health.value != 1.0f)
        {
            health.value += Time.deltaTime * _speedToChangeHealth;
        }

        //if(health.value == 0.0f) //Edit to show death screen
            //Debug.Log("You died");
    }

    private void ChangeHealthValue(AlterPlayerHealth alterPlayerHealth)
    {
        _recoverHealth = alterPlayerHealth.RecoverHealth;
        _speedToChangeHealth = alterPlayerHealth.SpeedToChangeHealth;
    }

    private void ApplyCorrioson(MaintainPlayerHealth maintainPlayerHealth)
    {
        _pauseCorrioson = maintainPlayerHealth.PauseCorrioson;
    }
}
