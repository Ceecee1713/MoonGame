using UnityEngine;

public class ChangeLight : MonoBehaviour
{
    public GameObject Moon;
    public Material litUpMoonMaterial;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MeshRenderer moonFragmentRenderer = Moon.GetComponent<MeshRenderer>();
            moonFragmentRenderer.material = litUpMoonMaterial;
        }
    }
}
