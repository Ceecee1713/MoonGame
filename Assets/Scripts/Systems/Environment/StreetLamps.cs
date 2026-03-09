using UnityEngine;

public class StreetLamps : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;

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
    }

    void Update()
    {
        if(gameManager.AreaChangesCount == areaNumber)
        {
            light.intensity = startingIntensity;
            enabled = false; 
        }  
    }
}
