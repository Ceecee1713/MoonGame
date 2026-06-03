using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the checking of player's inventory and matching current inventory items to given crafting materials
/// For the correct quantity amounts and type of item
/// </summary>
/// 
/// <remarks>
/// This script is to be attached to the Crafting UI and should have the "CraftButton" and "DecipherClueButton" be on the same parent game object
/// as those two scripts directly call public methods from this script
/// 
/// This script doesn't craft items or remove inventory items to be treated as crafting materials. This script acts more as a middle man to 
/// pass parameters through and validatebetween what's needed and what the player has. Removal of inventory items, adding inventory items, 
/// and solving clues are left to the "InventoryUI" and "CluebookManager" respectively
/// 
/// This script works closely with "CraftButton", "DecipherClueButton", "CluebookManager", "InventoryUI" scripts
/// See <see cref="CraftButton"/> for how they work together - "CraftButton" accessing multiple of this script's methods
/// See <see cref="DecipherClueButton"/> for how they work together - "DecipherClueButton" accessing multiple of this script's methods
/// See <see cref="CluebookManager"/> for how they work together - Publishing AllowToCraftClue event this script listens to and prompting to decipher a clue
/// See <see cref="InventoryUI"/> for how items are removed from inventory, added into inventory and currently held inventory items' quantities are adjusted.
/// 
/// See <see cref="InventoryData"/> for what the collection is made up of
/// 
///</remarks>

public class CraftManager : MonoBehaviour
{
    [Header ("Buttons")]
    [SerializeField]
    private CraftButton [] craftButtons; //Used to temporarily prevent the player from clicking on the buttons this script is attached to
    [SerializeField]
    private DecipherClueButton decipherClueButton; //Used to temporarily prevent the player from clicking on the buttons this script is attached to

    [SerializeField] private InventoryData inventoryData; //Collection of all the player's inventory items to be easily accessible

    private ItemData _itemToCraft; 

    private List <int> _amountOfFullStacksPerMaterialToRemove = new List <int>(); //Quantity of an item to be removed

    //Example: If 15 rocks and 4 wood needed to be used as materials, the indexes in this list would be 
    //At [0], it'll be 15. At [1], it'll be 4

    private List <ItemData> _materialsForCraftableItem = new List <ItemData>(); //Individual item data to be removed

    //"_amountOfFullStacksPerMaterialToRemove" and "_materialsForCraftableItem" are used when needing to remove an inventory item from player's inventory in "InventoryUI"

    private bool _allowCraftingForClue = false;
    private bool _quantityRemaining = false;
    private bool _moveToNextMaterial = false;
    private bool _notEnoughItemQuantity = false;
    private bool _allowPlayerInputs = false; //Prevent or allow for the player to interact or click on certain objects during runtime

    private int _amountOfMatchingCraftingMaterials = 0; //Amount of how many unique materials were matching in the inventory 

    //Example: If Two unique items were needed and the inventory had all Two matching inventory items, this value would be Two. 
    //If Two unique items were needed and the inventory had only two matching inventory items, this value would be two. 

    private int _remainingQuantity; //Leftover quantity for many of a singular item is needed, used when performing quantity calculations  
    private int _amountOfAnInventoryItemNeeded; //Quantity of a unique inventory item to be removed. This value is added to the "_amountOfFullStacksPerMaterialToRemove" list

    private const float DELAY = 0.25f;

    void Start()
    {
        EventBus.Instance.Subscribe<AllowToCraftClue>(CheckToMakeClue);
    }

    void OnEnable()
    {
        _allowPlayerInputs = false;
        EventBus.Instance.Publish(new ActivatePlayerInputs(_allowPlayerInputs));
    }

    void OnDisable()
    {
        _allowPlayerInputs = true;
        EventBus.Instance.Publish(new ActivatePlayerInputs(_allowPlayerInputs));

        StopAllCoroutines();
    }

    public void DelayClickingOfButtons() 
    {
        StopAllCoroutines();
        StartCoroutine(DelayClick());
    }

    public void ResetStatus() 
    {
        _amountOfFullStacksPerMaterialToRemove.Clear();
        _materialsForCraftableItem.Clear();

        _notEnoughItemQuantity = false;
        _quantityRemaining = false;
        _moveToNextMaterial = false;

        _amountOfMatchingCraftingMaterials = 0;
        _remainingQuantity = 0;
        _amountOfAnInventoryItemNeeded = 0;
    }

    private void CheckToMakeClue(AllowToCraftClue allowToCraftClue) //Published by "CluebookManager". Step Zero
    {
        _allowCraftingForClue = allowToCraftClue.AvaliableClueToDecipher;
    }

    public void SetInventoryItemToCraft(ItemData craftableInventoryItem) 
    {
        _itemToCraft = craftableInventoryItem;
    }

    //Called by "CraftButton" and "DecipherClueButton". Method repeatedly called from both scripts for each new crafting material
    public void CheckInventoryForCraftingMaterials(ItemData craftingMaterial) //Step One
    {
        if(_notEnoughItemQuantity == true)
            return;
        
        _quantityRemaining = false;
        _remainingQuantity = 0;
        _amountOfAnInventoryItemNeeded = 0;
        _moveToNextMaterial = false;

        SearchInventoryForItemMaterial(craftingMaterial);

        if(_remainingQuantity > 0)
        {
            _notEnoughItemQuantity = true;
            return;
        }
    }

    public void TryCompleteCraft(int maxAmountOfCraftingMaterialTypes, bool craftingAClue) //Step Seven
    {
        if(_notEnoughItemQuantity)
            return;

        if(craftingAClue)
        {
            if(_amountOfMatchingCraftingMaterials >= maxAmountOfCraftingMaterialTypes && _allowCraftingForClue != false)
            {
                DecipherClue(); //Step Seven
                EventBus.Instance.Publish(new DecipherClue()); //Publish to "CluebokManager"
            }
        }

        else
        {
            if(_amountOfMatchingCraftingMaterials >= maxAmountOfCraftingMaterialTypes)
                CraftInventoryItem(_itemToCraft); //Step Seven
        }
    }

    private void SearchInventoryForItemMaterial(ItemData craftingMaterial) //Step Two
    {
        for(int j = 0; j < inventoryData.Inventory.Count; j++) 
        {
            if(_moveToNextMaterial == true) 
                break;

            //Skip over inventory indexes at "j" that don't match "craftingMaterial"'s inventory item data
            if(!IsMatchingInventoryItemMaterial(j, craftingMaterial)) //Step Two.Four
                continue;

            DetermineInventoryItemQuantity(j, craftingMaterial); //Step Three
        }
    }

    //Check if the player's inventory data at "slotIndex" item matches "craftingMaterial"'s item type 
    private bool IsMatchingInventoryItemMaterial(int slotIndex, ItemData craftingMaterial) //Step Two.Four
    {
        //Checks both stackable and non-stackable items
        return inventoryData.Inventory[slotIndex].ItemType == craftingMaterial.ItemType;
    }

    private void DetermineInventoryItemQuantity(int slotIndex, ItemData craftingMaterial) //Step Three
    {
        CalculateRemainingQuantity(slotIndex, craftingMaterial); //Step Four
        
        if(_remainingQuantity > 0)
            HandleMoreInventoryItemQuantityNeeded(slotIndex, craftingMaterial); //Step Five

        else if(_remainingQuantity == 0)
            HandleInventoryItemQuantityConsumed(slotIndex, craftingMaterial); //Step Five

        else 
            HandleRemainingInventoryItemQuantity(slotIndex, craftingMaterial); //Step Five
    }

    private void CalculateRemainingQuantity(int slotIndex, ItemData craftingMaterial) //Step Four
    {
        if(_quantityRemaining == false) //First-time calculation
            _remainingQuantity = craftingMaterial.Quantity - inventoryData.Inventory[slotIndex].Quantity;
            
        else //Subtract from previous remainder
            _remainingQuantity = _remainingQuantity - inventoryData.Inventory[slotIndex].Quantity;
    }

    //Player needs more of an item, mark the inventory item's stack to be removed
    private void HandleMoreInventoryItemQuantityNeeded(int slotIndex, ItemData craftingMaterial) //Step Five
    {
        _quantityRemaining = true;
        _amountOfAnInventoryItemNeeded++;
        MarkItemForRemoval(slotIndex);
        CountMatchingInventoryItemMaterials(craftingMaterial);
    }

    //Exact quantity needed for crafting material found in player's inventory, mark inventory item for removal
    private void HandleInventoryItemQuantityConsumed(int slotIndex, ItemData craftingMaterial) //Step Five
    {
        _quantityRemaining = false;
        _amountOfAnInventoryItemNeeded++; 
        MarkItemForRemoval(slotIndex);
        CountMatchingInventoryItemMaterials(craftingMaterial);
        _moveToNextMaterial = true; //Move onto next crafting material (back to Step One)
    }

    //Leftover quantity of an item: Player owns more of that item in a SINGLE STACK than what's needed by the crafting recipe
    private void HandleRemainingInventoryItemQuantity(int slotIndex, ItemData craftingMaterial) //Step Five
    {
        _quantityRemaining = false;
        AdjustInventoryQuantity(slotIndex);
        _amountOfMatchingCraftingMaterials++; //Count for one material for craftable item's recipe found
        _moveToNextMaterial = true; //Move onto next crafting material (back to Step One)
    }

    //Mark an inventory item to be removed (item is removed in "InventoryUI")
    private void MarkItemForRemoval(int slotIndex)
    {
        _amountOfFullStacksPerMaterialToRemove.Add(_amountOfAnInventoryItemNeeded);
        _materialsForCraftableItem.Add(inventoryData.Inventory[slotIndex]);
    }

    private void CountMatchingInventoryItemMaterials(ItemData craftingMaterial)
    {
        for(int i = 0; i < inventoryData.Inventory.Count; i++)
        {
            if(inventoryData.Inventory[i].ItemType == craftingMaterial.ItemType)
                _amountOfAnInventoryItemNeeded--; //Decrease to indicate that a matching material has been found

            if(_amountOfAnInventoryItemNeeded == 0)
            {
                _amountOfMatchingCraftingMaterials++; //Increasing to get closer to the targetted amount of materials. EX: If a recipe needed 5 unique materials, the target would be 5
                break;
            }
        }
    }

    //Adjust quantity of an inventory item when there's a remainder of quantity for materials (more than what's needed by crafting recipe)
    private void AdjustInventoryQuantity(int slotIndex)
    {
        int newQuantity = Mathf.Abs(_remainingQuantity);
        InventoryItemTypes itemType = inventoryData.Inventory[slotIndex].ItemType;
        EventBus.Instance.Publish(new AdjustInventorySlotItemQuantity(newQuantity, itemType)); //Publish to "InventoryUI"
    }

    private void CraftInventoryItem(ItemData craftableInventoryItem) //Step Seven
    {
        ItemData clonedItem = craftableInventoryItem.Clone();
        EventBus.Instance.Publish(new RemoveUsedMaterials(_materialsForCraftableItem, _amountOfFullStacksPerMaterialToRemove)); //Publish to "InventoryUI"
        EventBus.Instance.Publish(new AddItemToInventory(clonedItem)); //Publish to "InventoryUI"
        ResetStatus();
    }

    private void DecipherClue() //Step Seven
    {
        EventBus.Instance.Publish(new RemoveUsedMaterials(_materialsForCraftableItem, _amountOfFullStacksPerMaterialToRemove)); //Publish to "InventoryUI"
        ResetStatus();
    }

    private IEnumerator DelayClick() 
    {
        yield return new WaitForSeconds(DELAY);

        for(int i = 0; i < craftButtons.Length; i++)
            craftButtons[i].AllowClicking();

        decipherClueButton.AllowClicking();
    }
}

