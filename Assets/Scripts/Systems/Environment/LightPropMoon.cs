using UnityEngine;

/// <summary>
/// Manages the material change and stopping of particle effects for decorative moon statues
/// </summary>
/// 
/// <remarks>
/// This script is to ONLY be attached to moon statues that their only purpose is to prompt a moon puzzle
/// This script will also be disabled as it only needs to serve one small minor purpose
/// 
/// This script works together with the "GameManager" script
/// See <see cref="GameManager"/> for how they work together - publishing the "NewAreaChange" event this script listens to
/// 
/// </remarks>

public class LightPropMoon : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem moonSparkles;

    [Header ("Moon Statue Lights")]
    [SerializeField]
    private Light lightUnderMoon; //Light that only illumintates the statue itself
    [SerializeField]
    private Light moonlight; //Light that illuminates the surrounding area

    [Header ("Moon Statue Pieces Information")]
    [SerializeField]
    private GameObject [] moonFragments; //Game Objects to swap their materials with
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

    //Swap materials, stop the particle effects and disable this script
    private void LightUpMoon(NewAreaChange newAreaChange) //Published by "GameManager" when a moon puzzle has been completed
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
