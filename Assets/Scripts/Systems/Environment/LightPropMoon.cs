using UnityEngine;

public class LightPropMoon : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem moonSparkles;

    [Header ("Moon Statue Lights")]
    [SerializeField]
    private Light lightUnderMoon;
    [SerializeField]
    private Light moonlight;

    [Header ("Moon Statue Pieces Information")]
    [SerializeField]
    private GameObject [] moonFragments;
    [SerializeField]
    private Material litUpMoonMaterial;

    [SerializeField]
    [Range(1, 2)]
    private int areaNumber;

    private float startingMoonlightIntensity, startingLightUnderMoonIntensity;

    void Start()
    {
        startingLightUnderMoonIntensity = lightUnderMoon.intensity;
        startingMoonlightIntensity = moonlight.intensity;

        lightUnderMoon.intensity = 0f;
        moonlight.intensity = 0f;

        EventBus.Instance.Subscribe<NewAreaChange>(LightUpMoon);
    }

    void OnEnable()
    {
    }

    void OnDisable()
    {
        if (EventBus.Instance != null)
            EventBus.Instance.Unsubscribe<NewAreaChange>(LightUpMoon);
    }

    private void LightUpMoon(NewAreaChange newAreaChange)
    {
        if(newAreaChange.AreaChangesCount == areaNumber)
        {
            moonSparkles.Stop();

            lightUnderMoon.intensity = startingLightUnderMoonIntensity;
            moonlight.intensity = startingMoonlightIntensity;

            for(int i = 0; i < moonFragments.Length; i++)
            {
                MeshRenderer moonFragmentRenderer = moonFragments[i].GetComponent<MeshRenderer>();
                moonFragmentRenderer.material = litUpMoonMaterial;
            }
            
            enabled = false; 
        } 
    }
}
