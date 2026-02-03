using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftManager : MonoBehaviour
{
    [SerializeField]
    private InventoryData inventoryData;

    private ItemData _itemToCraft;

    private List <int> _amountsPerUniqueInventoryItemsToRemove = new List <int>(); //Each index represents the total number of a unique inventory item to be removed
    private List <ItemData> _materialsForCraftableItem = new List <ItemData>();

    private bool _allowCraftingForClue = false;
    private bool _quantityRemaining = false;
    private bool _breakLoop = false;
    private bool _notEnoughItemQuantity = false;
    private bool _allowPlayerInputs = false;

    private int _amountOfMatchingCraftingMaterials = 0; //To be compared to the needed amount of unique materials for craftable item's recipe
    private int _remainingQuantity; 
    private int _amountOfAnInventoryItemNeeded; //Int to be added into "_amountsPerUniqueInventoryItemsToRemove" list 
    //Counts the number (quantity) of a single unique inventory item to be removed

    /*
    "_amountsPerUniqueInventoryItemsToRemove[i]" means the amount of a unique material to be removed. 
    For example, if I have wood as the material I use for my crafting recipe and I have 3 stacks of 10 of wood in my inventory (30 in total) 
    and my recipe needed 30 stacks, that value "_amountsPerUniqueInventoryItemsToRemove[i]" would be 3. 
    "_materialsForCraftableItem[i]" represents a unique material's item data. 
    */

    void Start()
    {
        EventBus.Instance.Subscribe<AllowToCraftClue>(CheckToMakeClue);
    }

    void OnEnable()
    {
        //Prevent Player Inputs
        _allowPlayerInputs = false;
        EventBus.Instance.Publish(new ActivatePlayerInputs(_allowPlayerInputs));
    }

    void OnDisable()
    {
        //Allow Player Inputs
        _allowPlayerInputs = true;
        EventBus.Instance.Publish(new ActivatePlayerInputs(_allowPlayerInputs));
    }

    public void ResetStatus() 
    {
        _amountsPerUniqueInventoryItemsToRemove.Clear();
        _materialsForCraftableItem.Clear();

        _notEnoughItemQuantity = false;
        _quantityRemaining = false;
        _breakLoop = false;

        _amountOfMatchingCraftingMaterials = 0;
        _remainingQuantity = 0;
        _amountOfAnInventoryItemNeeded = 0;
    }

    private void CheckToMakeClue(AllowToCraftClue allowToCraftClue) //Event call passed from CluebookManager
    {
        _allowCraftingForClue = allowToCraftClue.AvaliableClueToDecipher;
    }

    public void SetInventoryItemToCraft(ItemData craftableInventoryItem) //Called by CraftButton. Step Zero
    {
        _itemToCraft = craftableInventoryItem;
    }

    //All parameters passed from CraftButton and DecipherClueButton. This method is repeatedly called from both scripts for each new crafting material
    public void CheckInventoryForCraftingMaterials(ItemData craftingMaterial, int maxAmountOfCraftingMaterialTypes, bool craftingAClue) //Step One
    {
        if(_notEnoughItemQuantity == true)
            return;
        
        //Resetting for each new material 
        _quantityRemaining = false;
        _remainingQuantity = 0;
        _amountOfAnInventoryItemNeeded = 0;
        _breakLoop = false;

        if(_remainingQuantity > 0) //Exit method if there's not enough quantity for a material in inventory
        {
            _notEnoughItemQuantity = true;
            return;
        }

        _amountOfAnInventoryItemNeeded = 0;
        SearchInventoryForItemMaterial(craftingMaterial); //Step Two

        if(craftingAClue == true) //Deciphering clue
        {
            if(_amountOfMatchingCraftingMaterials >= maxAmountOfCraftingMaterialTypes && _allowCraftingForClue != false)
            {
                DecipherClue();
                EventBus.Instance.Publish(new DecipherClue()); //Calling CluebookManager
            }
        }

        else //Adding an inventory item to inventory
        {
            if(_amountOfMatchingCraftingMaterials >= maxAmountOfCraftingMaterialTypes)
                CraftInventoryItem(_itemToCraft);
        }
    }

    private void SearchInventoryForItemMaterial(ItemData craftingMaterial) //Step Three
    {
        for(int j = 0; j < inventoryData.Inventory.Count; j++) 
        {
            if(_breakLoop == true) //Move onto next crafting material 
                break;

            //Skip inventory slots that don't match "craftingMaterial"
            if(!IsMatchingInventoryItemMaterial(j, craftingMaterial)) //Step Three.Five
                continue;

            DetermineInventoryItemQuantity(j, craftingMaterial); //Step Four
        }
    }

    //Check if inventory slot's item matches "craftingMaterial" (same type)
    private bool IsMatchingInventoryItemMaterial(int slotIndex, ItemData craftingMaterial) //Step Three.Five
    {
        //Checks both stackable and non-stackable items
        return inventoryData.Inventory[slotIndex].ItemType == craftingMaterial.ItemType;
    }

    //Determine the inventory item's quantity 
    private void DetermineInventoryItemQuantity(int slotIndex, ItemData craftingMaterial) //Step Four
    {
        CalculateRemainingQuantity(slotIndex, craftingMaterial); //Step Five
        
        if(_remainingQuantity > 0)
            HandleMoreInventoryItemQuantityNeeded(slotIndex, craftingMaterial); //Step Six

        else if(_remainingQuantity == 0)
            HandleInventoryItemQuantityConsumed(slotIndex, craftingMaterial); //Step Six

        else 
            HandleRemainingInventoryItemQuantity(slotIndex, craftingMaterial); //Step Six
    }

    private void CalculateRemainingQuantity(int slotIndex, ItemData craftingMaterial) //Step Five
    {
        if(_quantityRemaining == false) //First-time calculation
            _remainingQuantity = craftingMaterial.Quantity - inventoryData.Inventory[slotIndex].Quantity;
            
        else //Subtract from previous remainder
            _remainingQuantity = _remainingQuantity - inventoryData.Inventory[slotIndex].Quantity;
    }

    private void HandleMoreInventoryItemQuantityNeeded(int slotIndex, ItemData craftingMaterial) //Step Six
    {
        _quantityRemaining = true;
        _amountOfAnInventoryItemNeeded++;
        MarkItemForRemoval(slotIndex);
        CountMatchingInventoryItemMaterials(craftingMaterial);
    }

    //Exact quantity needed for crafting material, mark inventory item for removal
    private void HandleInventoryItemQuantityConsumed(int slotIndex, ItemData craftingMaterial) //Step Six
    {
        _quantityRemaining = false;
        _amountOfAnInventoryItemNeeded++; 
        MarkItemForRemoval(slotIndex);
        CountMatchingInventoryItemMaterials(craftingMaterial);
        _breakLoop = true; //Move onto next crafting material 
    }

    //Leftover quantity, adjust inventory item's quantity
    private void HandleRemainingInventoryItemQuantity(int slotIndex, ItemData craftingMaterial) //Step Six
    {
        _quantityRemaining = false;
        AdjustInventoryQuantity(slotIndex);
        _amountOfMatchingCraftingMaterials++; //Count for one material for craftable item's recipe found
        _breakLoop = true; //Move onto next crafting material 
    }

    //Mark an inventory item to be removed (item is removed in "InventoryUI")
    private void MarkItemForRemoval(int slotIndex)
    {
        _amountsPerUniqueInventoryItemsToRemove.Add(_amountOfAnInventoryItemNeeded);
        _materialsForCraftableItem.Add(inventoryData.Inventory[slotIndex]);
    }

    private void CountMatchingInventoryItemMaterials(ItemData craftingMaterial)
    {
        for(int i = 0; i < inventoryData.Inventory.Count; i++)
        {
            if(inventoryData.Inventory[i].ItemType == craftingMaterial.ItemType)
                _amountOfAnInventoryItemNeeded--;

            if(_amountOfAnInventoryItemNeeded == 0)
            {
                _amountOfMatchingCraftingMaterials++;
                break;
            }
        }
    }

    //Adjust quantity of an inventory item when there's a remainder of quantity for materials (done in InventoryUI)
    private void AdjustInventoryQuantity(int slotIndex)
    {
        var newQuantity = Mathf.Abs(_remainingQuantity);
        EventBus.Instance.Publish(new AdjustInventorySlotItemQuantity(newQuantity, slotIndex));
    }

    /*
    "_amountsPerUniqueInventoryItemsToRemove[i]" means the amount of a unique material to be removed. 
    For example, if I have wood as the material I use for my crafting recipe and I have 3 stacks of 10 of wood in my inventory (30 in total) 
    and my recipe needed 30 stacks, that value "_amountsPerUniqueInventoryItemsToRemove[i]" would be 3. 
    "_materialsForCraftableItem[i]" represents a unique material's item data. 
    */

    private void CraftInventoryItem(ItemData craftableInventoryItem)
    {
        ItemData clonedItem = craftableInventoryItem.Clone();
        EventBus.Instance.Publish(new AddItemToInventory(clonedItem));
        EventBus.Instance.Publish(new RemoveUsedMaterials(_materialsForCraftableItem, _amountsPerUniqueInventoryItemsToRemove));
        ResetStatus();
    }

    private void DecipherClue()
    {
        EventBus.Instance.Publish(new RemoveUsedMaterials(_materialsForCraftableItem, _amountsPerUniqueInventoryItemsToRemove));
        ResetStatus();
    }
}
