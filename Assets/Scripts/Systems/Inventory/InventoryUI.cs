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

    private ItemData _equipedInventoryItem;

    private InventoryUISlot _selectedInventoryUISlot;
    private InventoryUISlot _previousInventoryUISlot;

    private bool _playerIsInCollision = false;
    private bool _allowInput = true;

    //Removes inventory items consumed during crafting
    private int _amountOfSingleFullyConsumedMaterialToRemove;
    private int _numberToMatchAmountOfFullyConsumedMaterial;

    private int _remainingQuantity;

    private const int MAX_STACK_AMOUNT = 9;

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

    private void AdjustInventoryQuantity(AdjustInventorySlotItemQuantity adjustInventorySlotItemQuantity)
    {
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            if(inventorySlots[i].InventoryItem.Quantity == inventoryData.Inventory[adjustInventorySlotItemQuantity.InventoryIndex].Quantity &&
            inventorySlots[i].InventoryItem.ItemType == inventoryData.Inventory[adjustInventorySlotItemQuantity.InventoryIndex].ItemType)
            {
                inventorySlots[i].InventoryItem.Quantity = adjustInventorySlotItemQuantity.NewQuantity;
                inventoryData.Inventory[adjustInventorySlotItemQuantity.InventoryIndex].Quantity = adjustInventorySlotItemQuantity.NewQuantity; 
                break;
            }

            else    
                Debug.Log("Not the same!");
        }
    }

    void Update()
    {
        if(_selectedInventoryUISlot != null) 
            _equipedInventoryItem = _selectedInventoryUISlot.InventoryItem;

        if (Input.GetKeyDown(KeyCode.Space)) //Testing
            Debug.Log(inventoryData.Inventory[0].Quantity + " " + inventoryData.Inventory[0].ItemType);
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

        if(itemToCheck.IsThisAStackableItem == true && !itemToCheck.IsDroppedItem)
        {
            for(int i = 0; i < inventorySlots.Length; i++) 
            {
                if(remainingQuantity <= 0)
                    break;

                if(inventorySlots[i].InventoryItem != null && inventorySlots[i].InventoryItem.IsThisAStackableItem == true && 
                    inventorySlots[i].InventoryItem.ItemType == itemToCheck.ItemType) 
                {
                    int currentQuantity = inventorySlots[i].InventoryItem.Quantity;
                    int availableSpace = MAX_STACK_AMOUNT - currentQuantity;
                    
                    if(availableSpace > 0) 
                    {
                        int amountToAdd = Mathf.Min(availableSpace, remainingQuantity); 
                        inventorySlots[i].InventoryItem.Quantity += amountToAdd; //Add quantity onto inventory slot's item's quantity
                        
                        //Adding quantity onto inventory item in inventoryData 
                        for(int j = 0; j < inventoryData.Inventory.Count; j++) 
                        {
                            if(inventoryData.Inventory[j].ItemType == itemToCheck.ItemType && inventoryData.Inventory[j].Quantity < MAX_STACK_AMOUNT)
                                inventoryData.Inventory[j].Quantity += amountToAdd; //Removed break statement here
                        }

                        remainingQuantity -= amountToAdd;
                    }
                }
            }
        }

        //Create a new stack for the same inventory type 
        if(remainingQuantity > 0)
        {
            itemToCheck.Quantity = Mathf.Min(remainingQuantity, MAX_STACK_AMOUNT);
            itemToCheck.IsDroppedItem = false;

            for(int i = 0; i < inventorySlots.Length; i++) 
            {
                if(inventorySlots[i].IsEmpty == true)
                {
                    inventorySlots[i].AddItemToSlot(itemToCheck);
                    inventoryData.Inventory.Add(itemToCheck);
                    remainingQuantity -= itemToCheck.Quantity;
                    break;
                }
            }
        }
        
        //Handling subsequent overflow of quantity stacking with clones 
        while(remainingQuantity > 0)
        {
            bool foundEmptySlot = false;
            
            for(int i = 0; i < inventorySlots.Length; i++) 
            {
                if(inventorySlots[i].IsEmpty == true)
                {
                    //Clone for each additional overflow slot
                    ItemData clonedItem = itemToCheck.Clone();
                    clonedItem.Quantity = Mathf.Min(remainingQuantity, MAX_STACK_AMOUNT);
                    itemToCheck.IsDroppedItem = false;

                    inventorySlots[i].AddItemToSlot(clonedItem);
                    inventoryData.Inventory.Add(clonedItem);
                    remainingQuantity -= clonedItem.Quantity;
                    foundEmptySlot = true;
                    break;
                }
            }
            
            if(!foundEmptySlot)
                break;
        }
    }

    //Refactored version of AddItemToInventory
    /*
    private void AddInventoryItem(AddItemToInventory addItemToInventory) 
    {
        ItemData itemToCheck = addItemToInventory.InventoryItem;
        int remainingQuantity = itemToCheck.Quantity;

        if(itemToCheck.IsThisAStackableItem && !itemToCheck.IsDroppedItem)
        {
            remainingQuantity = AddToExistingStacks(itemToCheck, remainingQuantity);
        }

        if(remainingQuantity > 0)
        {
            remainingQuantity = CreateNewStack(itemToCheck, remainingQuantity);
        }
        
        if(remainingQuantity > 0)
        {
            CreateOverflowStacks(itemToCheck, remainingQuantity);
        }
    }

    private int AddToExistingStacks(ItemData itemToCheck, int remainingQuantity)
    {
        for(int i = 0; i < inventorySlots.Length; i++) 
        {
            if(remainingQuantity <= 0)
                break;

            if(!IsMatchingStackableSlot(i, itemToCheck))
                continue;

            int currentQuantity = inventorySlots[i].InventoryItem.Quantity;
            int availableSpace = MAX_STACK_AMOUNT - currentQuantity;
            
            if(availableSpace > 0) 
            {
                int amountToAdd = Mathf.Min(availableSpace, remainingQuantity); 
                inventorySlots[i].InventoryItem.Quantity += amountToAdd;
                
                UpdateInventoryDataQuantity(itemToCheck.ItemType, amountToAdd);
                
                remainingQuantity -= amountToAdd;
            }
        }
        
        return remainingQuantity;
    }

    private bool IsMatchingStackableSlot(int slotIndex, ItemData itemToCheck)
    {
        return inventorySlots[slotIndex].InventoryItem != null && 
            inventorySlots[slotIndex].InventoryItem.IsThisAStackableItem && 
            inventorySlots[slotIndex].InventoryItem.ItemType == itemToCheck.ItemType;
    }

    private void UpdateInventoryDataQuantity(InventoryItemTypes itemType, int amountToAdd)
    {
        for(int j = 0; j < inventoryData.Inventory.Count; j++) 
        {
            if(inventoryData.Inventory[j].ItemType == itemType && 
            inventoryData.Inventory[j].Quantity < MAX_STACK_AMOUNT)
            {
                inventoryData.Inventory[j].Quantity += amountToAdd;
            }
        }
    }

    private int CreateNewStack(ItemData itemToCheck, int remainingQuantity)
    {
        itemToCheck.Quantity = Mathf.Min(remainingQuantity, MAX_STACK_AMOUNT);
        itemToCheck.IsDroppedItem = false;

        for(int i = 0; i < inventorySlots.Length; i++) 
        {
            if(inventorySlots[i].IsEmpty)
            {
                inventorySlots[i].AddItemToSlot(itemToCheck);
                inventoryData.Inventory.Add(itemToCheck);
                remainingQuantity -= itemToCheck.Quantity;
                break;
            }
        }
        
        return remainingQuantity;
    }

    private void CreateOverflowStacks(ItemData itemToCheck, int remainingQuantity)
    {
        while(remainingQuantity > 0)
        {
            bool foundEmptySlot = false;
            
            for(int i = 0; i < inventorySlots.Length; i++) 
            {
                if(inventorySlots[i].IsEmpty)
                {
                    ItemData clonedItem = itemToCheck.Clone();
                    clonedItem.Quantity = Mathf.Min(remainingQuantity, MAX_STACK_AMOUNT);
                    clonedItem.IsDroppedItem = false;

                    inventorySlots[i].AddItemToSlot(clonedItem);
                    inventoryData.Inventory.Add(clonedItem);
                    remainingQuantity -= clonedItem.Quantity;
                    foundEmptySlot = true;
                    break;
                }
            }
            
            if(!foundEmptySlot)
                break;
        }
    }
    */


    /*
    In my RemoveConsumedMaterials method, "removeUsedMaterials.AmountsPerStackableItemToRemove[i]" means the amount of a unique material to be removed. 
    For example, if I have wood as the material I use for my crafting recipe and I have 3 stacks of 10 of wood in my inventory (30 in total) 
    and my recipe needed 30 stacks, that value "AmountsPerStackableItemToRemove[i]" would be 3. 
    It represents the full stacks of materials that will be consumed for crafting
    This value is the same and is passed from the craft manager where it is called "_amountsPerUniqueInventoryItemsToRemove"
    "removeUsedMaterials.CraftingMaterialItems[i]" or "_materialsForCraftableItem[i]" represents a unique material's item data. 
    */

    private void RemoveConsumedMaterials(RemoveUsedMaterials removeUsedMaterials)
    {
        //Removal of inventory items that were used as crafting materials (by CraftManager)
        for(int i = 0; i < removeUsedMaterials.AmountsPerStackableItemToRemove.Count; i++)
        {
            _amountOfSingleFullyConsumedMaterialToRemove = removeUsedMaterials.AmountsPerStackableItemToRemove[i];
            _numberToMatchAmountOfFullyConsumedMaterial = 0;

            var targetInventoryItemType = removeUsedMaterials.CraftingMaterialItems[i].ItemType;
            
            for(int j = 0; j < inventorySlots.Length; j++)
            {
                if(_numberToMatchAmountOfFullyConsumedMaterial >= _amountOfSingleFullyConsumedMaterialToRemove)
                    break;

                if (inventorySlots[j].InventoryItem != null && inventorySlots[j].InventoryItem.ItemType == targetInventoryItemType)
                {
                    //Remove item from inventory slot
                    ItemData itemToRemove = inventorySlots[j].InventoryItem;
                    int quantityToMatch = itemToRemove.Quantity;
                    inventorySlots[j].RemoveItemFromSlot();
                    
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
                    
                    _numberToMatchAmountOfFullyConsumedMaterial++;
                }
            }
        }
    }

    /*
    //Refactored version of RemoveConsumedMaterials
    private void RemoveConsumedMaterials(RemoveUsedMaterials removeUsedMaterials)
    {
        //Removal of inventory items that were used as crafting materials (by CraftManager)
        for(int i = 0; i < removeUsedMaterials.AmountsPerStackableItemToRemove.Count; i++)
        {
            _amountOfSingleFullyConsumedMaterialToRemove = removeUsedMaterials.AmountsPerStackableItemToRemove[i];
            _numberToMatchAmountOfFullyConsumedMaterial = 0;

            var targetInventoryItemType = removeUsedMaterials.CraftingMaterialItems[i].ItemType;
            
            RemoveStacksOfMaterial(targetInventoryItemType);
        }
    }

    private void RemoveStacksOfMaterial(InventoryItemTypes targetInventoryItemType)
    {
        for(int j = 0; j < inventorySlots.Length; j++)
        {
            if(_numberToMatchAmountOfFullyConsumedMaterial >= _amountOfSingleFullyConsumedMaterialToRemove)
                break;

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

    private void RemoveFromInventoryData(InventoryItemTypes targetInventoryItemType, int quantityToMatch)
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
    */

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

    //Called by InventoryUISlot 
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