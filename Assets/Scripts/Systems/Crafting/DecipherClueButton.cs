using UnityEngine;

//Edit to add the changing of the material text on UI

public class DecipherClueButton : MonoBehaviour
{
    [Header ("Script References")]
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private CraftManager craftManager;

    [Header ("Materials")]
    [SerializeField]
    private ItemData [] craftingMaterials = new ItemData[2];
    [SerializeField]
    private ItemData [] craftingMaterialsForAreaTwo = new ItemData[2];
    [SerializeField]
    private ItemData [] craftingMaterialsForAreaThree = new ItemData[2];

    private bool _craftingAClue = false;

    private int newMoonPuzzleIndex;

    void OnEnable()
    {
        newMoonPuzzleIndex = gameManager.areaChangesCount;
        CheckToSwitchMaterials();
    }

    void OnDisable()
    {
    }

    private void CheckToSwitchMaterials()
    {
        if(newMoonPuzzleIndex == 0)
            return;

        if(newMoonPuzzleIndex == 1) //Edit(?)
        {
            for(int i = 0; i < craftingMaterials.Length; i++)
                craftingMaterials[i] = craftingMaterialsForAreaTwo[i];
        }

        if(newMoonPuzzleIndex == 2) //Edit(?)
        {
            for(int i = 0; i < craftingMaterials.Length; i++)
                craftingMaterials[i] = craftingMaterialsForAreaThree[i];
        }
    }

    public void OnDecipherClueClick()
    {
        EventBus.Instance.Publish(new CheckForCompleteClues()); //Calls CluebookManager
        craftManager.ResetStatus();

        _craftingAClue = true;

        for(int i = 0; i < craftingMaterials.Length; i++)
            craftManager.CheckInventoryForCraftingMaterials(craftingMaterials[i], craftingMaterials.Length, _craftingAClue);
    }
}
