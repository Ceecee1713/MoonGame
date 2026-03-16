using UnityEngine;
using TMPro;

public class DecipherClueButton : MonoBehaviour
{
    [SerializeField]
    private AudioClip buttonClickSFX;

    [SerializeField]
    private TextMeshProUGUI materialDescriptionText;

    [Header ("Script References")]
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private CraftManager craftManager;

    [Header ("Materials")]
    [SerializeField]
    private ItemData [] craftingMaterials = new ItemData[2];

    //All material arrays MUST be the same length as "craftingMaterials"
    [SerializeField]
    private ItemData [] craftingMaterialsForAreaTwo = new ItemData[2];
    [SerializeField]
    private ItemData [] craftingMaterialsForAreaThree = new ItemData[2];

    private string [] _craftingMaterialsNames = new string[2];

    private bool _craftingAClue = false;

    private int newMoonPuzzleIndex;

    void Start()
    {
        UpdateMaterialDescriptionText(); 
    }

    void OnEnable()
    {
        newMoonPuzzleIndex = gameManager.AreaChangesCount;
        CheckToSwitchMaterials();
    }

    void OnDisable()
    {
    }

    private void CheckToSwitchMaterials()
    {
        if(newMoonPuzzleIndex == 0)
            return;

        if(newMoonPuzzleIndex == 1) 
        {
            //Changing the crafting materials
            for(int i = 0; i < craftingMaterials.Length; i++) 
                craftingMaterials[i] = craftingMaterialsForAreaTwo[i];

            UpdateMaterialDescriptionText();
            return;
        }

        if(newMoonPuzzleIndex == 2) 
        {
            //Changing the crafting materials
            for(int i = 0; i < craftingMaterials.Length; i++) 
                craftingMaterials[i] = craftingMaterialsForAreaThree[i];

            UpdateMaterialDescriptionText();
        }
    }

    public void OnDecipherClueClick()
    {
        AudioManager.Instance.PlaySoundEffect(buttonClickSFX);
        
        EventBus.Instance.Publish(new CheckForCompleteClues()); //Publish to CluebookManager
        craftManager.ResetStatus();

        _craftingAClue = true;

        for(int i = 0; i < craftingMaterials.Length; i++)
            craftManager.CheckInventoryForCraftingMaterials(craftingMaterials[i], craftingMaterials.Length, _craftingAClue);
    }

    private void UpdateMaterialDescriptionText()
    {
        for(int i = 0; i < craftingMaterials.Length; i++)
        {
            string craftingMaterialName = craftingMaterials[i].ItemType.ToDisplayName(); //Method extension to InventoryItemTypes Enums
            string craftinMaterialQuantity = craftingMaterials[i].Quantity.ToString();
            _craftingMaterialsNames[i] = craftingMaterialName + " x" + craftinMaterialQuantity;
        }
            
        materialDescriptionText.text = string.Join("\n", _craftingMaterialsNames);
    }
}
