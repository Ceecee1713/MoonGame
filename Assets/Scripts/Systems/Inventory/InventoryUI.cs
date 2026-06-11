using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the player's inventory: adding items, removing items, equiping/unequiping items, dropping items
/// </summary>
/// 
/// <remarks>
/// This script is made to be on an UI object for the inventory UI
/// and on the same game object as "InventoryUISlot" as public variables and methods are to be referenced by that script
/// 
/// This script works together with the "CraftManager", "InventoryUISlot", "InteractableItem", "ChestSlot", "InteractableItem", "PlayerInputController", "PlayerStateMachine" scripts
/// See <see cref="CraftManager"/> and how they interact with removing items and flagging those items for removal as well as adding the crafted inventory item 
/// as well as prompting the "AdjustInventorySlotItemQuantity" and "RemoveUsedMaterials" events this script listens to
/// 
/// See <see cref="InventoryUISlot"/> and how they interact with visuals and setting an inventory item to a slot 
/// as well as prompting the "SelectInventoryItem" and "RemoveItemFromSlot" events this script listens to
/// 
/// See <see cref="ChestSlot"/> and how they interact with visuals and setting an inventory item to a slot as well as 
/// prompting the "AddItemToInventory' event this script listens to
/// 
/// See <see cref="InteractableItem"/> - Listening to "AddItemToInventory' event that "InteractableItem" publishes
/// See <see cref="PlayerInputController"/> Listening to "DropEquipedInventoryItem' and "UseInventoryItem" events that "PlayerInputController" publishes 
/// 
/// "ChestUI" acts similarily to this script. The method of adding an inventory item to their respective inventories is the same
/// Make sure they both function the same
/// See <see cref="ChestUI"/> for how they function similarily.
/// 
/// This script unequips items when they are dropped, but those dropped items are instantiated and initialized in a different script: "PlayerSpawner"
/// See <see cref="PlayerSpawner"/> on how it's done
/// 
/// See <see cref="PlayerStateMachine"/> - Publishing "SpeedUpPlayer" event to speed up the player upon using an item
/// 
/// See <see cref="InventoryItemTypes"/> for what makes up an inventory item.
/// See <see cref="InventoryData"/> for what the collection is made up of.
/// 
/// This script works with multiple other scripts that publish and subscribe to "ActivatePlayerInputs" event
/// Please see <see cref="AddItemToInventory"/> to get the full details as it would be too much to write in this script alone
/// 
/// </remarks>

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private InventoryUISlot [] inventorySlots = new InventoryUISlot [8]; //Inventory slots visibly shown on screen

    [SerializeField] 
    private InventoryData inventoryData; //Collection of all the player's inventory items to be easily accessible

    [SerializeField]
    private ItemData _equipedInventoryItem;

    [SerializeField]
    private InventoryUISlot _selectedInventoryUISlot;
    [SerializeField]
    private InventoryUISlot _previousInventoryUISlot;

    private bool _allowInput = true; //Prevent or allow for the player to use or drop items
    private bool _playerIsInCollision = false; //Flag whether the player can interact with the crafting table or not: if they're in range or not

    //Removes inventory items consumed during crafting
    private int _amountOfFullStacksPerMaterialConsumed;
    private int _numberToMatchAmountOfFullyConsumedMaterial = 0;

    //Reduce quantity of inventory items during crafting
    private int _inventoryDataIndex;
    private int _newQuantity;

    //For adding items into inventory
    private int _remainingQuantity;

    private const int MAX_STACK_AMOUNT = 9; //Make sure this value is the same as the MAX_STACK_AMOUNT for "ChestUI" script

    void Start()
    {
        inventoryData.Inventory.Clear(); 

        //Adds a new item to inventory and removes the materials used
        EventBus.Instance.Subscribe<AddItemToInventory>(AddInventoryItem);
        EventBus.Instance.Subscribe<RemoveUsedMaterials>(RemoveConsumedMaterials);
        EventBus.Instance.Subscribe<AdjustInventorySlotItemQuantity>(AdjustInventoryQuantity);

        //Player interaction events 
        EventBus.Instance.Subscribe<PreventPlayerInteractingWithInventory>(CheckIfPlayerIsInACollision);
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

    //Receives a "PreventPlayerInteractingWithInventory" event with parameters:
    //(bool) PlayerInCollision - (true = player is in a CERTAIN TYPE of collision, 
    //false = player is NOT in a CERTAIN TYPE of collision).
    private void CheckIfPlayerIsInACollision(PreventPlayerInteractingWithInventory preventPlayerInteractingWithInventory) //Multiple publishers
    {
        _playerIsInCollision = preventPlayerInteractingWithInventory.PlayerInCollision;
    }

    //Receives a "ActivatePlayerInputs" event with parameters:
    //(bool) AllowInputs - (true = allow the player to interact with world objects and UI, 
    //false = do NOT allow the player to interact with world objects and UI).
    private void AllowPlayerInput(ActivatePlayerInputs activatePlayerInputs) //Multiple publishers and subscribers
    {
        _allowInput = activatePlayerInputs.AllowInputs;
    }

    #region Adding Inventory Item

    //Receives a "AddItemToInventory" event with parameters:
    //(ItemData) InventoryItem - Inventory item to add into player's inventory
    //Published by "ChestSlot" or "InteractableItem"
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

    //Adding onto quantity of an existing, same inventory item with new "itemToCheck" quantity
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

    //Adding onto quantity of existing same inventory item in inventory data
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

    //Add new iventory item stack for "itemToCheck" in inventory slot and inventory data
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

    //Add new iventory item overflow stacks for "itemToCheck" in inventory slot and inventory data
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


    //Receives a "AdjustInventorySlotItemQuantity" event with parameters:
    //(int) NewQuantity - Quantity to now set for the inventory item that's been marked by "CraftManager" to be used as a material
    //(InventoryItemTypes) ItemType - Inventory type of the material 

    //Published by "CraftManager" if there's remainder for a crafting material
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

        //Update quantity of an existing inventory item in an inventory slot and inventory data
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

    //Receives a "RemoveUsedMaterials" event with parameters:
    //List <ItemData> CraftingMaterialItems - All the inventory items that have been marked as materials by "CraftManager"
    //List <int> AmountsPerStackableItemToRemove - Number of full stacks of each material to be removed
    //Example: Each index represents the number of full stacks of ONE inventory item to be removed from player's ivnentory

    //Published by "CraftManager" to completely remove used up inventory items as materials
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

        //Complete removal of inventory items that were used as crafting materials 
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
                break; //Removed all full stacks needed for this material, move onto next material (back to Step One)

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

    //The ordering of items between the player's displayed inventory and what they actually in their inventory through a scriptable object have can become out of sync.
    //The scriptable object representing the inventory is meant to be an easier way of accessibility to know what the player has 
    //Thus, the "InventoryUI" iterates backwards to avoid issues of desyncing 

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

    //Receives a "SelectInventoryItem" event with parameters:
    //(ItemData) InventoryItem - Equiped inventory item from currently selected inventory UI slot
    //(InventoryUISlot) InventoryUISlot - Currently selected inventory UI slot
    private void EquipInventoryItem(SelectInventoryItem selectInventoryItem) //Published by "InventoryUISlot"
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

    //Method for "inventoryData" as the ordering of items between the player's displayed inventory (in the inventory slots)
    //and what they actually in their inventory through the scriptable object, "inventoryData", they can become out of sync
    private int FindInventoryDataIndex(InventoryItemTypes itemType)
    {
        for(int i = 0; i < inventoryData.Inventory.Count; i++)
            if(inventoryData.Inventory[i].ItemType == itemType)
                return i;

        return -1;
    }

    //"UseInventoryItem" is the name of an event. Empty event
    private void CheckToUseInventoryItem(UseInventoryItem useInventoryItem) //Reserved for only speed potions, published by "PlayerInputController"
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

                    EventBus.Instance.Publish(new SpeedUpPlayer()); //Publish to "PlayerStateMachine"

                    //Deselect inventory slot
                    _selectedInventoryUISlot.OutlineImage.SetActive(false); 
                    _selectedInventoryUISlot = null;
                    _equipedInventoryItem = null;
                    break;
                }
            }
        }
    }

    //"DropEquipedInventoryItem" is the name of an event. Empty event
    private void DropEquipedInventoryItem(DropEquipedInventoryItem dropEquipedInventoryItem) //Published by "PlayerInputController"
    {
        if(_playerIsInCollision || _allowInput == false)
            return;

        for(int i = 0; i < inventorySlots.Length; i++)
        {
            if(inventorySlots[i] == _selectedInventoryUISlot && _selectedInventoryUISlot.InventoryItem != null) 
            {
                int dataIndex = FindInventoryDataIndex(_selectedInventoryUISlot.InventoryItem.ItemType);

                //Removing item from inventory slot and inventory data
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

    //Receives a "RemoveItemFromSlot" event with parameters:
    //(InventoryUISlot) InventorySlot - Currently selected inventory UI slot
    private void RemoveItemFromInventory(RemoveItemFromSlot removeItemFromSlot) //Published by "InventoryUISlot" to remove its item when item moves into a chest
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