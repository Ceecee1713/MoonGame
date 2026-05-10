using UnityEngine;

public class StreetLamps : MonoBehaviour
{
    [SerializeField]
    private Light light;

    [SerializeField]
    [Range(1, 2)]
    private int areaNumber;

    private float startingIntensity;

    void Start()
    {
        startingIntensity = light.intensity;
        light.intensity = 0f;

        EventBus.Instance.Subscribe<NewAreaChange>(LightUpLamp);
    }

    void OnEnable()
    {
    }

    void OnDisable()
    {
        if (EventBus.Instance != null)
            EventBus.Instance.Unsubscribe<NewAreaChange>(LightUpLamp);
    }

    private void LightUpLamp(NewAreaChange newAreaChange)
    {
        if(newAreaChange.AreaChangesCount == areaNumber)
        {
            light.intensity = startingIntensity;
            enabled = false; 
        } 
    }
}
