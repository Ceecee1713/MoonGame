using UnityEngine;

public class StreetLamps : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;

    [SerializeField]
    private GameObject lights;

    [SerializeField]
    [Range(1, 2)]
    private int areaNumber;

    void Update()
    {
        if(gameManager.areaChangesCount == areaNumber)
        {
            lights.SetActive(true);
            enabled = false; 
        }  
    }
}
