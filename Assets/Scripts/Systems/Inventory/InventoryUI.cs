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

    private int _maxStackAmount = 3;

    //Removes inventory items consumed during crafting
    private int _amountOfSingleFullyConsumedMaterialToRemove;
    private int _numberToMatchAmountOfFullyConsumedMaterial;

    void Start()
    {
        inventoryData.Inventory.Clear();

        //Adds a new item to inventory and removes the materials used
        EventBus.Instance.Subscribe<CheckToAddInventoryItem>(CheckInventorySlot);
        EventBus.Instance.Subscribe<CheckToAddCraftedItem>(CheckToAddCraftedItem);

        //Player interaction events 
        EventBus.Instance.Subscribe<InCollision>(CheckIfPlayerIsInACollision);
        EventBus.Instance.Subscribe<SelectInventoryItem>(EquipInventoryItem);
        EventBus.Instance.Subscribe<DropEquipedInventoryItem>(DropEquipedInventoryItem);
        EventBus.Instance.Subscribe<UseInventoryItem>(CheckToUseInventoryItem);

        //Removes inventory items consumed when deciphering a cluebook clue
        EventBus.Instance.Subscribe<RemoveInventoryItemsForMaterials>(RemoveInventoryItemsForDecipheringClue);
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

    private void CheckInventorySlot(CheckToAddInventoryItem checkToAddInventoryItem) //Add normal inventory item to player inventory
    {
        AddInventoryItem(checkToAddInventoryItem.InventoryItem);
    }

    private void AddInventoryItem(ItemData itemToCheck) //Add an inventory item (Crafted item or not)
    {
        for(int i = 0; i < inventorySlots.Length; i++) //Add same type, stackable items together in same inventory slot
        {
            if(inventorySlots[i].InventoryItem.IsThisAStackableItem == true && itemToCheck.IsThisAStackableItem == true)
            {
                if(inventorySlots[i].InventoryItem.ItemType == itemToCheck.ItemType)
                {
                    //If the inventory slot's item's quantity isn't above "_maxStackAmount" (increase quantity)
                    if(inventorySlots[i].InventoryItem.Quantity < _maxStackAmount) 
                    {
                        inventorySlots[i].InventoryItem.Quantity++; 
                        return;
                    }
                } 
            }
        }

        //Add new inventory item in any empty inventory slot, whether item is stackable or not
        for(int i = 0; i < inventorySlots.Length; i++) 
        {
            if(inventorySlots[i].IsEmpty == true)
            {
                inventorySlots[i].AddItemToSlot(itemToCheck);
                inventoryData.Inventory.Add(itemToCheck);
                break;
            }
        }
    }

    private void CheckToAddCraftedItem(CheckToAddCraftedItem checkToAddCraftedItem) //Add crafted inventory item to player inventory
    {
        AddInventoryItem(checkToAddCraftedItem.InventoryItem);

        if(checkToAddCraftedItem.AmountsPerStackableItemToRemove.Count == 0) 
            return;

        //Removal of inventory items that were used as crafting materials for the CraftManager:
        for(int i = 0; i < checkToAddCraftedItem.AmountsPerStackableItemToRemove.Count; i++)
        {
            _amountOfSingleFullyConsumedMaterialToRemove = checkToAddCraftedItem.AmountsPerStackableItemToRemove[i];
            _numberToMatchAmountOfFullyConsumedMaterial = 0;

            var targetInventoryItemType = checkToAddCraftedItem.CraftingMaterialItems[i].ItemType;
            
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

    //Remove inventory items that were used as materials for deciphering a clue
    private void RemoveInventoryItemsForDecipheringClue(RemoveInventoryItemsForMaterials removeInventoryItemsForMaterials) 
    {
        if(removeInventoryItemsForMaterials.AmountsPerStackableItemToRemove.Count == 0) 
            return;

        //Removal of inventory items that were used as crafting materials
        for(int i = 0; i < removeInventoryItemsForMaterials.AmountsPerStackableItemToRemove.Count; i++)
        {
            _amountOfSingleFullyConsumedMaterialToRemove = removeInventoryItemsForMaterials.AmountsPerStackableItemToRemove[i];
            _numberToMatchAmountOfFullyConsumedMaterial = 0;

            var targetInventoryItemType = removeInventoryItemsForMaterials.CraftingMaterialItems[i].ItemType;
            
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
        if(_playerIsInCollision == true)
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
}
