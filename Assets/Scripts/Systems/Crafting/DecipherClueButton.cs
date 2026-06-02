using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Assigns the materials to decipher a clue from the cluebook
/// </summary>
/// 
/// <remarks>
/// This script works closely with "CraftManager" script for it to run through each material and see 
/// if the player's inventory has the right amount of that particular material to decipher a clue. 
/// This script also works closely with the "GameManager" script to get a public variable's value only.
/// 
/// The materials change as more moon puzzle areas are solved, which they are updated in a method: "CheckToSwitchMaterials" and "UpdateMaterialDescriptionText"
/// 
/// This script is designed to be on a button game object 
/// This script is on the same game object as the "CraftManager", so it can directly access any of its public methods or variables
///</remarks>


public class DecipherClueButton : MonoBehaviour
{
    [SerializeField]
    private AudioClip buttonClickSFX;

    [SerializeField]
    private TextMeshProUGUI materialDescriptionText; //Description detailing the names of the materials and their quantities

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

    //All material arrays MUST be the same length as "craftingMaterials"
    //All crafting materials are the same as normal inventory items

    /// <remarks>See <see cref="InventoryItemTypes"/> for inventory items are structured.</remarks>
    private string [] _craftingMaterialsNames = new string[2]; //For the crafting material descriptions. 
    //Updates when materials are changed when more moon puzzle areas are solved

    private bool _allowClicking = true; //Prevent or allow for the player to click on the button game object this script is attached to

    /// <remarks>See <see cref="GameManager"/> for context on "AreaChangesCount".</remarks>
    private int newMoonPuzzleIndex; //This value "syncs" with the GameManager's "AreaChangesCount" variable though it's only a publicy get variable.


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

    //Checking whether to change materials based on how many moon puzzle areas were solved. 
    //The if-statement branches ranging from 0-2 is because of the maximum value of "gameManager.AreaChangesCount"
    //The ints go up to the maximum value of "gameManager.AreaChangesCount". Change materials as each moon puzzle area is resolved.
    //Look at the GameManager's script on how it works with this script
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

    /// <remarks>
    /// See <see cref="CluebookManager"/> for more context on which method "CheckForCompleteClues" event is sent to.</remarks>
    /// See <see cref="CraftManager"/> for this script interacts with its various methods.</remarks>
    ///</remarks>
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
