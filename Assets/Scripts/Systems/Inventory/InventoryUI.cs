using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private InventoryUISlot [] inventorySlots = new InventoryUISlot [8];

    [SerializeField]
    private InventoryData inventoryData; 

    [SerializeField]
    private ItemData _equipedInventoryItem;

    [SerializeField]
    private InventoryUISlot _selectedInventoryUISlot;
    [SerializeField]
    private InventoryUISlot _previousInventoryUISlot;

    private bool _allowInput = true;
    private bool _playerIsInCollision = false;

    //Removes inventory items consumed during crafting
    private int _amountOfFullStacksPerMaterialConsumed;
    private int _numberToMatchAmountOfFullyConsumedMaterial = 0;

    //Reduce quantity of inventory items during crafting
    private int _inventoryDataIndex;
    private int _newQuantity;

    //For adding items into inventory
    private int _remainingQuantity;

    private const int MAX_STACK_AMOUNT = 9; //Make sure this value is the same as the MAX_STACK_AMOUNT for ChestUI

    void Start()
    {
        inventoryData.Inventory.Clear(); 

        //Adds a new item to inventory and removes the materials used
        EventBus.Instance.Subscribe<AddItemToInventory>(AddInventoryItem);
        EventBus.Instance.Subscribe<RemoveUsedMaterials>(RemoveConsumedMaterials);
        EventBus.Instance.Subscribe<AdjustInventorySlotItemQuantity>(AdjustInventoryQuantity);

        //Player interaction events 
        EventBus.Instance.Subscribe<InCollision>(CheckIfPlayerIsInACollision);
        EventBus.Instance.Subscribe<SelectInventoryItem>(EquipInventoryItem);
        EventBus.Instance.Subscribe<DropEquipedInventoryItem>(DropEquipedInventoryItem);
        EventBus.Instance.Subscribe<UseInventoryItem>(CheckToUseInventoryItem);
        EventBus.Instance.Subscribe<ActivatePlayerInputs>(AllowPlayerInput);

        //For removing an inventory item when moving item into a chest
        EventBus.Instance.Subscribe<RemoveItemFromSlot>(RemoveItemFromInventory);

        this.gameObject.SetActive(false);
    }

    void Update()
    {
        if(_selectedInventoryUISlot != null) 
            _equipedInventoryItem = _selectedInventoryUISlot.InventoryItem;
    }

    private void CheckIfPlayerIsInACollision(InCollision inCollision)
    {
        _playerIsInCollision = inCollision.PlayerInCollision;
    }

    private void AllowPlayerInput(ActivatePlayerInputs activatePlayerInputs)
    {
        _allowInput = activatePlayerInputs.AllowInputs;
    }

    #region Adding Inventory Item

    private void AddInventoryItem(AddItemToInventory addItemToInventory) //Step One
    {
        ItemData itemToCheck = addItemToInventory.InventoryItem;
        _remainingQuantity = itemToCheck.Quantity;

        if(itemToCheck.IsThisAStackableItem && !itemToCheck.IsDroppedItem)
            _remainingQuantity = AddToExistingStacks(itemToCheck); //Step Two

        if(_remainingQuantity > 0)
            _remainingQuantity = CreateNewStack(itemToCheck); //Step Two
        
        if(_remainingQuantity > 0)
            CreateOverflowStacks(itemToCheck); //Step Two
    }

    //Adding onto quantity to an existing inventory item with new "itemToCheck" quantity
    private int AddToExistingStacks(ItemData itemToCheck) //Step Two
    {
        for(int i = 0; i < inventorySlots.Length; i++) 
        {
            if(_remainingQuantity <= 0)
                break;

            //Checking is inventory slot item is stackable, not null and shares same item type as "itemToCheck"
            if(!IsMatchingStackableSlot(i, itemToCheck))
                continue;

            int currentQuantity = inventorySlots[i].InventoryItem.Quantity;
            int availableSpace = MAX_STACK_AMOUNT - currentQuantity;
            
            if(availableSpace > 0) 
            {
                //Add quantity on existing inventory item in inventory slot
                int amountToAdd = Mathf.Min(availableSpace, _remainingQuantity); 
                inventorySlots[i].InventoryItem.Quantity += amountToAdd;
                inventorySlots[i].UpdateItemTextQuantity(inventorySlots[i].InventoryItem.Quantity);
                
                //Adding onto quantity of existing inventory item in inventory data
                UpdateInventoryDataQuantity(itemToCheck.ItemType, amountToAdd);
                
                _remainingQuantity -= amountToAdd;
            }
        }
        
        return _remainingQuantity;
    }

    //Check if inventory slot item is stackable, not null and shares same item type as "itemToCheck"
    private bool IsMatchingStackableSlot(int slotIndex, ItemData itemToCheck)
    {
        return inventorySlots[slotIndex].InventoryItem != null && 
            inventorySlots[slotIndex].InventoryItem.IsThisAStackableItem && 
            inventorySlots[slotIndex].InventoryItem.ItemType == itemToCheck.ItemType;
    }

    //Adding onto quantity of existing inventory item in inventory data
    private void UpdateInventoryDataQuantity(InventoryItemTypes itemType, int amountToAdd) //Step Three
    {
        for(int j = 0; j < inventoryData.Inventory.Count; j++) 
        {
            if(inventoryData.Inventory[j].ItemType == itemType && inventoryData.Inventory[j].Quantity < MAX_STACK_AMOUNT)
            {
                inventoryData.Inventory[j].Quantity += amountToAdd;
                break;
            }
        }
    }

    //Create new iventory item stack for "itemToCheck" in inventory slot and inventory data
    private int CreateNewStack(ItemData itemToCheck) //Step Two
    {
        itemToCheck.Quantity = Mathf.Min(_remainingQuantity, MAX_STACK_AMOUNT);
        itemToCheck.IsDroppedItem = false;

        for(int i = 0; i < inventorySlots.Length; i++) 
        {
            if(inventorySlots[i].IsEmpty)
            {
                inventorySlots[i].AddItemToSlot(itemToCheck);
                inventoryData.Inventory.Add(itemToCheck);
                _remainingQuantity -= itemToCheck.Quantity;
                break;
            }
        }
        
        return _remainingQuantity;
    }

    //Create new iventory item overflow stacks for "itemToCheck" in inventory slot and inventory data
    private void CreateOverflowStacks(ItemData itemToCheck) //Step Two
    {
        while(_remainingQuantity > 0)
        {
            bool foundEmptySlot = false;
            
            for(int i = 0; i < inventorySlots.Length; i++) 
            {
                if(inventorySlots[i].IsEmpty)
                {
                    ItemData clonedItem = itemToCheck.Clone();
                    clonedItem.Quantity = Mathf.Min(_remainingQuantity, MAX_STACK_AMOUNT);
                    clonedItem.IsDroppedItem = false;

                    inventorySlots[i].AddItemToSlot(clonedItem);
                    inventoryData.Inventory.Add(clonedItem);
                    _remainingQuantity -= clonedItem.Quantity;
                    foundEmptySlot = true;
                    break;
                }
            }
            
            if(!foundEmptySlot)
                break;
        }
    }
    #endregion

    //Published by CraftManager if there's remainder for a crafting material
    private void AdjustInventoryQuantity(AdjustInventorySlotItemQuantity adjustInventorySlotItemQuantity)
    {
        bool hasEmptySlot = false;

        for(int i = 0; i < inventorySlots.Length; i++)
        {
            if(inventorySlots[i].IsEmpty)
            {
                hasEmptySlot = true;
                break;
            }
        }

        if(!hasEmptySlot)
            return;

        _newQuantity = adjustInventorySlotItemQuantity.NewQuantity;
        InventoryItemTypes itemType = adjustInventorySlotItemQuantity.ItemType;

        for(int j = 0; j < inventorySlots.Length; j++)
        {
            if(inventorySlots[j].InventoryItem != null &&
            inventorySlots[j].InventoryItem.ItemType == itemType)
            {
                inventorySlots[j].InventoryItem.Quantity = _newQuantity;
                inventorySlots[j].UpdateItemTextQuantity(_newQuantity);

                for(int k = 0; k < inventoryData.Inventory.Count; k++)
                {
                    if(inventoryData.Inventory[k].ItemType == itemType)
                    {
                        inventoryData.Inventory[k].Quantity = _newQuantity;
                        break;
                    }
                }
                break;
            }
        }
    }

    #region Removing Consumed Inventory Items That Were Used As Materials

    private void RemoveConsumedMaterials(RemoveUsedMaterials removeUsedMaterials) //Step One
    {
        //Check all inventory slots if they have items before starting to remove any
        //If no empty inventory slots are found (all slots have an item), don't remove any of them as materials
        bool hasEmptySlot = false;

        for(int i = 0; i < inventorySlots.Length; i++)
        {
            if(inventorySlots[i].IsEmpty)
            {
                hasEmptySlot = true;
                break;
            }
        }

        if(!hasEmptySlot)
            return;

        //Removal of inventory items that were used as crafting materials (by CraftManager) for FULL STACK materials 
        for(int j = 0; j < removeUsedMaterials.AmountsPerStackableItemToRemove.Count; j++)
        {
            _amountOfFullStacksPerMaterialConsumed = removeUsedMaterials.AmountsPerStackableItemToRemove[j];
            _numberToMatchAmountOfFullyConsumedMaterial = 0;

            var targetInventoryItemType = removeUsedMaterials.CraftingMaterialItems[j].ItemType;
            
            RemoveStacksOfMaterial(targetInventoryItemType); 
        }
    }

    private void RemoveStacksOfMaterial(InventoryItemTypes targetInventoryItemType) //Step Two
    {
        for(int j = 0; j < inventorySlots.Length; j++)
        {
            if(_numberToMatchAmountOfFullyConsumedMaterial >= _amountOfFullStacksPerMaterialConsumed) 
                break; //Removed all full stacks needed for the material, move onto next material (back to Step One)

            if (inventorySlots[j].InventoryItem == null || inventorySlots[j].InventoryItem.ItemType != targetInventoryItemType)
                continue;

            //Remove item from inventory slot
            ItemData itemToRemove = inventorySlots[j].InventoryItem;
            int quantityToMatch = itemToRemove.Quantity;
            inventorySlots[j].RemoveItemFromSlot();
            
            RemoveFromInventoryData(targetInventoryItemType, quantityToMatch);
            
            _numberToMatchAmountOfFullyConsumedMaterial++;
        }
    }

    private void RemoveFromInventoryData(InventoryItemTypes targetInventoryItemType, int quantityToMatch) //Step Three
    {
        //Iterating backwards as we're removing indexes of the "inventoryData.Inventory" List
        for(int k = inventoryData.Inventory.Count - 1; k >= 0; k--)
        {
            //Remove matching inventory item from inventoryData
            if(inventoryData.Inventory[k].ItemType == targetInventoryItemType && 
            inventoryData.Inventory[k].Quantity == quantityToMatch)
            {
                inventoryData.Inventory.RemoveAt(k);
                break;
            }
        }
    }
    #endregion

    private void EquipInventoryItem(SelectInventoryItem selectInventoryItem) //When selecting on an inventory slot
    {
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            if(inventorySlots[i] == selectInventoryItem.InventoryUISlot)
            {
                if(_selectedInventoryUISlot != null) 
                {
                    //Disable visuals of the previously selected inventory slot
                    _previousInventoryUISlot = _selectedInventoryUISlot;
                    _previousInventoryUISlot.OutlineImage.SetActive(false);
                }
                
                //Enable visuals of new selected inventory slot
                _selectedInventoryUISlot = selectInventoryItem.InventoryUISlot; 
                _selectedInventoryUISlot.OutlineImage.SetActive(true); 
                _equipedInventoryItem = selectInventoryItem.InventoryUISlot.InventoryItem;
                break;
            }
        }
    }

    //Method for Inventory Data as inventorySlots length and inventoryData length can become out of sync
    private int FindInventoryDataIndex(InventoryItemTypes itemType)
    {
        for(int i = 0; i < inventoryData.Inventory.Count; i++)
            if(inventoryData.Inventory[i].ItemType == itemType)
                return i;

        return -1;
    }

    private void CheckToUseInventoryItem(UseInventoryItem useInventoryItem)
    {
        if(_playerIsInCollision == true || _allowInput == false)
            return;

        if(_equipedInventoryItem != null && _equipedInventoryItem.ItemType == InventoryItemTypes.SpeedPotion)
        {
            for(int i = 0; i < inventorySlots.Length; i++)
            {
                //Remove inventory item from inventory and its inventory slot and speed up player
                if(inventorySlots[i] == _selectedInventoryUISlot && _selectedInventoryUISlot.InventoryItem != null)
                {
                    int dataIndex = FindInventoryDataIndex(_selectedInventoryUISlot.InventoryItem.ItemType);

                    if(dataIndex >= 0)
                        inventoryData.Inventory.RemoveAt(dataIndex);

                    _selectedInventoryUISlot.RemoveItemFromSlot();

                    EventBus.Instance.Publish(new SpeedUpPlayer());

                    //Deselect inventory slot
                    _selectedInventoryUISlot.OutlineImage.SetActive(false); 
                    _selectedInventoryUISlot = null;
                    _equipedInventoryItem = null;
                    break;
                }
            }
        }
    }

    private void DropEquipedInventoryItem(DropEquipedInventoryItem dropEquipedInventoryItem)
    {
        if(_playerIsInCollision || _allowInput == false)
            return;

        for(int i = 0; i < inventorySlots.Length; i++)
        {
            if(inventorySlots[i] == _selectedInventoryUISlot && _selectedInventoryUISlot.InventoryItem != null)
            {
                int dataIndex = FindInventoryDataIndex(_selectedInventoryUISlot.InventoryItem.ItemType);

                if(dataIndex >= 0)
                    inventoryData.Inventory.RemoveAt(dataIndex);

                inventorySlots[i].DropItem();

                //Deselect inventory slot
                _selectedInventoryUISlot.OutlineImage.SetActive(false);
                _selectedInventoryUISlot = null;
                _equipedInventoryItem = null;
                break;
            }
        }
    }

    private void RemoveItemFromInventory(RemoveItemFromSlot removeItemFromSlot) //Published by InventoryUISlot to remove its item when item moves into a chest
    {
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            //Remove inventory item from inventory and its inventory slot
            if(inventorySlots[i] == _selectedInventoryUISlot && _selectedInventoryUISlot == removeItemFromSlot.InventorySlot)
            {   
                int dataIndex = FindInventoryDataIndex(_selectedInventoryUISlot.InventoryItem.ItemType);

                if(dataIndex >= 0)
                    inventoryData.Inventory.RemoveAt(dataIndex);

                inventorySlots[i].RemoveItemFromSlot();
                break;
            }
        }
    }
}