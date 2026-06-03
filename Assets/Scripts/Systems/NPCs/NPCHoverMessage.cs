using UnityEngine;

/// <summary>
/// Manages displaying a world space UI component for NPC game objects based on player collision
/// </summary>

public class NPCHoverMessage : MonoBehaviour
{
    [SerializeField]
    private InteractableItem worldInteractableItem; //Set ONLY for interactble world items (already placed in scene, permenately)

    [SerializeField]
    private GameObject hoverMessageUI;

    private void OnTriggerEnter(Collider collider)
    {
        if(worldInteractableItem == null && collider.gameObject.CompareTag("Player"))
        {
            hoverMessageUI.SetActive(true);
            return;
        }
    
       if(worldInteractableItem != null && worldInteractableItem.InteractedByPlayerOnce == false && collider.gameObject.CompareTag("Player"))
            hoverMessageUI.SetActive(true);
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
            hoverMessageUI.SetActive(false);
    }
}
