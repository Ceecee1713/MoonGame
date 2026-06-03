using UnityEngine;

/// <summary>
/// Manages light intensity
/// </summary>
/// 
/// <remarks>
/// This script will be disabled as it only needs to serve one small minor purpose
/// 
/// This script works together with the "GameManager" script
/// See <see cref="GameManager"/> for how they work together - publishing the "NewAreaChange" event this script listens to
/// </remarks>

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

    private void LightUpLamp(NewAreaChange newAreaChange) //Published by "GameManager" when a moon puzzle has been completed
    {
        if(newAreaChange.AreaChangesCount == areaNumber)
        {
            light.intensity = startingIntensity;
            enabled = false; 
        } 
    }
}
