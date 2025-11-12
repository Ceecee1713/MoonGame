using UnityEngine;

public class NPCHoverMessage : MonoBehaviour
{
    [SerializeField]
    private GameObject hoverMessageUI;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
            hoverMessageUI.SetActive(true);
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
            hoverMessageUI.SetActive(false);
    }
}
