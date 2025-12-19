using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private InventoryData inventoryData;

    [SerializeField]
    private InventoryUISlot [] inventorySlots = new InventoryUISlot [8];

    private ItemData _equipedInventoryItem;

    private InventoryUISlot _selectedInventoryUISlot;
    private InventoryUISlot _previousInventoryUISlot;

    private bool _playerIsInCollision = false;
    private bool _allowInput = false;

    //Removes inventory items consumed during crafting
    private int _amountOfSingleFullyConsumedMaterialToRemove;
    private int _numberToMatchAmountOfFullyConsumedMaterial;

    private const int MAX_STACK_AMOUNT = 9;

    void Start()
    {
        inventoryData.Inventory.Clear();

        //Adds a new item to inventory and removes the materials used
        EventBus.Instance.Subscribe<AddItemToInventory>(AddInventoryItem);
        EventBus.Instance.Subscribe<RemoveUsedMaterials>(RemoveConsumedMaterials);

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

    private void AddInventoryItem(AddItemToInventory addItemToInventory) 
    {
        ItemData itemToCheck = addItemToInventory.InventoryItem;
        
        int remainingQuantity = itemToCheck.Quantity;

        //Add quantity of newly added item
        //Into existing items of the same type in the inventory
        if(itemToCheck.IsThisAStackableItem == true)
        {
            for(int i = 0; i < inventorySlots.Length; i++) 
            {
                if(remainingQuantity <= 0)
                    break;

                if(inventorySlots[i].InventoryItem.IsThisAStackableItem == true && inventorySlots[i].InventoryItem.ItemType == itemToCheck.ItemType)
                {
                    int currentQuantity = inventorySlots[i].InventoryItem.Quantity;
                    
                    //Calculate how much space is available in this item 
                    int availableSpace = MAX_STACK_AMOUNT - currentQuantity;
                    
                    if(availableSpace > 0)
                    {
                        //Calculate how much quantity can add to this item 
                        int amountToAdd = Mathf.Min(availableSpace, remainingQuantity);
                        
                        //Add quantity to existing item 
                        inventorySlots[i].InventoryItem.Quantity += amountToAdd;
                        remainingQuantity -= amountToAdd;
                    }
                }
            }
        }

        //Create a new item for any remaining quantity
        //This accounts for non-stackable items as well (not adding quantity)
        while(remainingQuantity > 0)
        {
            bool foundEmptySlot = false;
            
            for(int i = 0; i < inventorySlots.Length; i++) 
            {
                if(inventorySlots[i].IsEmpty == true)
                {
                    //For the first new item, use the original item
                    if(remainingQuantity == itemToCheck.Quantity)
                    {
                        int quantityForThisStack = Mathf.Min(remainingQuantity, MAX_STACK_AMOUNT);
                        itemToCheck.Quantity = quantityForThisStack;
                        
                        inventorySlots[i].AddItemToSlot(itemToCheck);
                        inventoryData.Inventory.Add(itemToCheck);
                        
                        remainingQuantity -= quantityForThisStack;
                    }

                    //Clone for additional overflow quantity
                    else
                    {
                        ItemData itemToCheckClone = itemToCheck.Clone();
                        itemToCheckClone.Quantity = Mathf.Min(remainingQuantity, MAX_STACK_AMOUNT);
                        
                        inventorySlots[i].AddItemToSlot(itemToCheckClone);
                        inventoryData.Inventory.Add(itemToCheckClone);
                        
                        remainingQuantity -= itemToCheckClone.Quantity;
                    }
                    
                    foundEmptySlot = true;
                    break;
                }
            }
            
            //If no empty slot was found, we can't add more items
            if(!foundEmptySlot)
                break;
        }
    }

    private void RemoveConsumedMaterials(RemoveUsedMaterials removeUsedMaterials)
    {
        if(removeUsedMaterials.AmountsPerStackableItemToRemove.Count == 0) 
            return;

        //Removal of inventory items that were used as crafting materials
        for(int i = 0; i < removeUsedMaterials.AmountsPerStackableItemToRemove.Count; i++)
        {
            _amountOfSingleFullyConsumedMaterialToRemove = removeUsedMaterials.AmountsPerStackableItemToRemove[i];
            _numberToMatchAmountOfFullyConsumedMaterial = 0;

            var targetInventoryItemType = removeUsedMaterials.CraftingMaterialItems[i].ItemType;
            
            for(int j = 0; j < inventorySlots.Length; j++)
            {
                if(_numberToMatchAmountOfFullyConsumedMaterial >= _amountOfSingleFullyConsumedMaterialToRemove)
                    break;
                
                if (inventorySlots[j].InventoryItem.ItemType == targetInventoryItemType)
                {
                    inventoryData.Inventory.Remove(inventorySlots[j].InventoryItem);
                    inventorySlots[j].RemoveItemFromSlot();
                    _numberToMatchAmountOfFullyConsumedMaterial++;
                }
            }
        }
    }

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

                _selectedInventoryUISlot = selectInventoryItem.InventoryUISlot; 
                
                //Enable visuals of selected inventory slot
                _selectedInventoryUISlot.OutlineImage.SetActive(true); 
                _equipedInventoryItem = selectInventoryItem.InventoryUISlot.InventoryItem;
                break;
            }
        }
    }

    private void CheckToUseInventoryItem(UseInventoryItem useInventoryItem)
    {
        if(_playerIsInCollision == true || _allowInput == false)
            return;

        if(_equipedInventoryItem != null && _equipedInventoryItem.ItemType == InventoryItemTypes.SpeedPotion)
        {
            for(int i = 0; i < inventorySlots.Length; i++)
            {
                //If selected UI slot is within the "inventorySlots" array
                if(inventorySlots[i] == _selectedInventoryUISlot)
                {
                    inventoryData.Inventory.Remove(inventorySlots[i].InventoryItem);
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
        if(_allowInput == false)
            return;

        for(int i = 0; i < inventorySlots.Length; i++)
        {
            //If selected UI slot is within the "inventorySlots" array 
            if(inventorySlots[i] == _selectedInventoryUISlot)
            {
                //Remove inventory item from inventory, its inventory slot and instiantiate item in world space
                inventoryData.Inventory.Remove(inventorySlots[i].InventoryItem);
                inventorySlots[i].DropItem();

                //Deselect inventory slot
                _selectedInventoryUISlot.OutlineImage.SetActive(false); 
                _selectedInventoryUISlot = null;
                _equipedInventoryItem = null;
                break;
            }
        }
    }

    //Called by InventoryUISlot (remove its inventory item to move to a chest's inventory)
    private void RemoveItemFromInventory(RemoveItemFromSlot removeItemFromSlot)
    {
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            //If selected UI slot is within the "inventorySlots" array 
            if(inventorySlots[i] == removeItemFromSlot.InventorySlot)
            {
                //Remove inventory item from inventory, its inventory slot and instiantiate item in world space
                inventoryData.Inventory.Remove(inventorySlots[i].InventoryItem);
                inventorySlots[i].RemoveItemFromSlot();

                //Deselect inventory slot
                //_selectedInventoryUISlot.OutlineImage.SetActive(false); 
                //_selectedInventoryUISlot = null;
                //_equipedInventoryItem = null;
                break;
            }
        }
    }
}
