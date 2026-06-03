using UnityEngine;

/// <summary>
/// Manages the inventory of the chest though for only adding items
/// </summary>
/// 
/// <remarks>
/// This script works together with the "ChestSlot" script
/// See <see cref="ChestSlot"/> and how they interact with visuals and setting an inventory item to a slot
/// 
/// "InventoryUI" acts similarily to this script. The method of adding an inventory item to their respective inventories is the same
/// Make sure they both function the same
/// See <see cref="InventoryUI"/> for how they function similarily and how "InventoryUI" publishes the "CheckToAddItemToChest" event this script listens to
/// 
/// See <see cref="InventoryItemTypes"/> for what makes up an inventory item.
/// 
/// </remarks>

public class ChestUI : MonoBehaviour
{
    [SerializeField]
    private ChestSlot[] chestSlots;

    private int _remainingQuantity;

    private const int MAX_STACK_AMOUNT = 9; //Make sure this value is the same as the MAX_STACK_AMOUNT for InventoryUI

    void Start()
    {
        EventBus.Instance.Subscribe<CheckToAddItemToChest>(CheckToAddItemIntoChest);
    }

    void OnEnable()
    {
        EventBus.Instance.Publish(new FreezePlayer(true));
        EventBus.Instance.Publish(new MaintainPlayerHealth(true));
        EventBus.Instance.Publish(new PauseExplorationTimer(true));
    }

    void OnDisable()
    {
        EventBus.Instance.Publish(new FreezePlayer(false));
        EventBus.Instance.Publish(new MaintainPlayerHealth(false));
        EventBus.Instance.Publish(new PauseExplorationTimer(false));
    }

    private void CheckToAddItemIntoChest(CheckToAddItemToChest checkToAddItemToChest) //Published by "InventoryUISlot" 
    {
        AddInventoryItem(checkToAddItemToChest.InventoryItem);
    }

    #region Adding Inventory Item

    private void AddInventoryItem(ItemData itemToCheck) // Step One
    {
        itemToCheck.IsDroppedItem = false;
        _remainingQuantity = itemToCheck.Quantity;

        if(itemToCheck.IsThisAStackableItem)
            _remainingQuantity = AddToExistingStacks(itemToCheck); // Step Two

        if(_remainingQuantity > 0)
            _remainingQuantity = CreateNewStack(itemToCheck); // Step Two

        if(_remainingQuantity > 0)
            CreateOverflowStacks(itemToCheck); // Step Two
    }

    //Adding onto quantity of an existing, same inventory item with new "itemToCheck" quantity
    private int AddToExistingStacks(ItemData itemToCheck) // Step Two
    {
        for(int i = 0; i < chestSlots.Length; i++)
        {
            if(_remainingQuantity <= 0)
                break;

            //Checking is inventory slot item is stackable, not null and shares same item type as "itemToCheck"
            if(!IsMatchingStackableSlot(i, itemToCheck))
                continue;

            int currentQuantity = chestSlots[i].InventoryItem.Quantity;
            int availableSpace = MAX_STACK_AMOUNT - currentQuantity;

            if(availableSpace > 0)
            {
                //Add quantity on existing inventory item in inventory slot
                int amountToAdd = Mathf.Min(availableSpace, _remainingQuantity);
                chestSlots[i].InventoryItem.Quantity += amountToAdd;
                chestSlots[i].UpdateItemTextQuantity(chestSlots[i].InventoryItem.Quantity);

                _remainingQuantity -= amountToAdd;
            }
        }

        return _remainingQuantity;
    }

    //Check if inventory slot item is stackable, not null and shares same item type as "itemToCheck"
    private bool IsMatchingStackableSlot(int slotIndex, ItemData itemToCheck) //Step Three
    {
        return chestSlots[slotIndex].InventoryItem != null &&
            chestSlots[slotIndex].InventoryItem.IsThisAStackableItem &&
            chestSlots[slotIndex].InventoryItem.ItemType == itemToCheck.ItemType;
    }

    //Add new chest item stack for itemToCheck in chest slot
    private int CreateNewStack(ItemData itemToCheck) //Step Two
    {
        itemToCheck.Quantity = Mathf.Min(_remainingQuantity, MAX_STACK_AMOUNT);

        for(int i = 0; i < chestSlots.Length; i++)
        {
            if(chestSlots[i].IsEmpty)
            {
                chestSlots[i].AddItemToSlot(itemToCheck);
                _remainingQuantity -= itemToCheck.Quantity;
                break;
            }
        }

        return _remainingQuantity;
    }

    //Add new chest item overflow stacks for itemToCheck in chest slots
    private void CreateOverflowStacks(ItemData itemToCheck) //Step Two
    {
        while(_remainingQuantity > 0)
        {
            bool foundEmptySlot = false;

            for(int i = 0; i < chestSlots.Length; i++)
            {
                if(chestSlots[i].IsEmpty)
                {
                    ItemData clonedItem = itemToCheck.Clone();
                    clonedItem.Quantity = Mathf.Min(_remainingQuantity, MAX_STACK_AMOUNT);

                    chestSlots[i].AddItemToSlot(clonedItem);
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
}