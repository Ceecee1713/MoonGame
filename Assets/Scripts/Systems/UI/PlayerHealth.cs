using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private Slider health;

    private bool _recoverHealth = false;
    private float _speedToChangeHealth;

    void Start()
    {
        EventBus.Instance.Subscribe<AlterPlayerHealth>(ChangeHealthValue);
    }

    void Update()
    {
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

        //if(health.value == 1.0f)
            //Debug.Log("You're full health now");
    }

    private void ChangeHealthValue(AlterPlayerHealth alterPlayerHealth)
    {
        _recoverHealth = alterPlayerHealth.RecoverHealth;
        _speedToChangeHealth = alterPlayerHealth.SpeedToChangeHealth;
    }
}
