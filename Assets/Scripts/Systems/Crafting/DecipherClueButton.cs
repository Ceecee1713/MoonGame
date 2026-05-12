using System.Collections;
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

    private bool _allowClicking = true;

    private int newMoonPuzzleIndex;

    private const bool CRAFTING_A_CLUE = true;

    private const float DELAY = 0.5f;

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
        _allowClicking = true;
    }

    //Explain the numbers for comparison in the "if" statements and why they matter - GameManager 
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
        if(_allowClicking != true)
        return;

        _allowClicking = false;

        AudioManager.Instance.PlaySoundEffect(buttonClickSFX);
        
        EventBus.Instance.Publish(new CheckForCompleteClues());
        
        craftManager.ResetStatus();

        for(int i = 0; i < craftingMaterials.Length; i++)
            craftManager.CheckInventoryForCraftingMaterials(craftingMaterials[i]);

        craftManager.TryCompleteCraft(craftingMaterials.Length, CRAFTING_A_CLUE); 
        craftManager.DelayClickingOfButtons();
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

    public void AllowClicking()
    {
        _allowClicking = true;
    }
}
