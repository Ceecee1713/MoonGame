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

    private void CheckToMakeClue(AllowToCraftClue allowToCraftClue)
    {
        _allowCraftingForClue = allowToCraftClue.AvaliableClueToDecipher;
    }

    public void SetInventoryItemToCraft(ItemData craftableInventoryItem)
    {
        _itemToCraft = craftableInventoryItem;
    }

    public void CheckInventoryForCraftingMaterials(ItemData craftingMaterial, int maxAmountOfCraftingMaterialTypes, bool craftingAClue)
    {
        if(_notEnoughItemQuantity == true)
            return;
        
        _breakLoop = false;
        
        //Iterate through inventory for "maxAmountOfCraftingMaterialTypes" amount of times
        for(int i = 0; i < maxAmountOfCraftingMaterialTypes; i++) 
        {
            if(_breakLoop == true)
                break;

            //If there's still remaining quantity needed for previous material when wanting to move on to the next material, exit method
            if(_remainingQuantity > 0) 
            {
                _notEnoughItemQuantity = true;
                return;
            }

            _amountOfAnInventoryItemNeeded = 0;

            SearchInventoryForItemMaterial(craftingMaterial);
        }

        if(craftingAClue == true)
        {
            if(_amountOfMatchingCraftingMaterials >= maxAmountOfCraftingMaterialTypes && _allowCraftingForClue != false)
            {
                DecipherClue();
                EventBus.Instance.Publish(new DecipherClue());
            }
        }

        else //Adding an inventory item to inventory
        {
            if(_amountOfMatchingCraftingMaterials >= maxAmountOfCraftingMaterialTypes)
                CraftInventoryItem(_itemToCraft);
        }
    }

    //Iterate through inventory slots for the specific inventory item material ("craftingMaterial")
    private void SearchInventoryForItemMaterial(ItemData craftingMaterial)
    {
        for(int j = 0; j < inventoryData.Inventory.Count; j++) 
        {
            if(_breakLoop == true)
                break;

            //Skip inventory slots that don't match "craftingMaterial"
            if(!IsMatchingInventoryItemMaterial(j, craftingMaterial))
                continue;

            DetermineInventoryItemQuantity(j, craftingMaterial);
        }
    }

    //Check if the inventory slot's item matches the inventory item data of "craftingMaterial" (same type and are stackable)
    private bool IsMatchingInventoryItemMaterial(int slotIndex, ItemData craftingMaterial)
    {
        return inventoryData.Inventory[slotIndex].ItemType == craftingMaterial.ItemType &&
               inventoryData.Inventory[slotIndex].IsThisAStackableItem == true && 
               craftingMaterial.IsThisAStackableItem == true;
    }

    //Determine the inventory item's quantity 
    private void DetermineInventoryItemQuantity(int slotIndex, ItemData craftingMaterial)
    {
        CalculateRemainingQuantity(slotIndex, craftingMaterial);
        
        if(_remainingQuantity > 0)
            HandleMoreInventoryItemQuantityNeeded(); 

        else if(_remainingQuantity == 0)
            HandleInventoryItemQuantityConsumed(slotIndex, craftingMaterial);

        else 
            HandleRemainingInventoryItemQuantity(slotIndex, craftingMaterial);
    }

    private void CalculateRemainingQuantity(int slotIndex, ItemData craftingMaterial)
    {
        if(_quantityRemaining == false) //First-time calculation 
            _remainingQuantity = craftingMaterial.Quantity - inventoryData.Inventory[slotIndex].Quantity;

        else //Subtract from previous remainder
            _remainingQuantity = _remainingQuantity - inventoryData.Inventory[slotIndex].Quantity;
    }

    private void HandleMoreInventoryItemQuantityNeeded()
    {
        _quantityRemaining = true;
        _amountOfAnInventoryItemNeeded++;
    }

    //Exact quantity needed for the inventory item material, mark inventory item for removal
    private void HandleInventoryItemQuantityConsumed(int slotIndex, ItemData craftingMaterial)
    {
        _quantityRemaining = false;
        _amountOfAnInventoryItemNeeded++; //Adding because this inventory item's quantity is fully consumed
        
        MarkItemForRemoval(slotIndex);
        CountMatchingInventoryItemMaterials(craftingMaterial);
        
        _breakLoop = true;
    }

    private void CountMatchingInventoryItemMaterials(ItemData craftingMaterial)
    {
        for(int k = 0; k < inventoryData.Inventory.Count; k++)
        {
            if(inventoryData.Inventory[k].ItemType == craftingMaterial.ItemType)
                _amountOfAnInventoryItemNeeded--;

            if(_amountOfAnInventoryItemNeeded == 0)
            {
                _amountOfMatchingCraftingMaterials++; //Count for one of the materials for the craftable item's recipe found
                break;
            }
        }
    }

    //Remaining quantity left for the inventory item in inventory, adjust its quantity
    private void HandleRemainingInventoryItemQuantity(int slotIndex, ItemData craftingMaterial)
    {
        _quantityRemaining = false;
        
        MarkItemForRemoval(slotIndex);
        AdjustInventoryQuantity(slotIndex);
        
        _amountOfMatchingCraftingMaterials++; //Count for one of the materials for the craftable item's recipe found
        _breakLoop = true;
    }

    //Mark an inventory item to be removed (item is removed in "InventoryUI")
    private void MarkItemForRemoval(int slotIndex)
    {
        _amountsPerUniqueInventoryItemsToRemove.Add(_amountOfAnInventoryItemNeeded);
        _materialsForCraftableItem.Add(inventoryData.Inventory[slotIndex]);
    }

    //Adjust quantity of an inventory item when there's a remainder
    private void AdjustInventoryQuantity(int slotIndex)
    {
        inventoryData.Inventory[slotIndex].Quantity = -_remainingQuantity;
    }

    private void CraftInventoryItem(ItemData craftableInventoryItem)
    {
        EventBus.Instance.Publish(new AddItemToInventory(craftableInventoryItem));
        EventBus.Instance.Publish(new RemoveUsedMaterials(_materialsForCraftableItem, _amountsPerUniqueInventoryItemsToRemove));
        ResetStatus();
    }

    private void DecipherClue()
    {
        EventBus.Instance.Publish(new RemoveUsedMaterials(_materialsForCraftableItem, _amountsPerUniqueInventoryItemsToRemove));
        ResetStatus();
    }
}
