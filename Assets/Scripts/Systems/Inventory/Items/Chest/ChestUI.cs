using UnityEngine;

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

    private void CheckToAddItemIntoChest(CheckToAddItemToChest checkToAddItemToChest)
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

    //Adding onto quantity to an existing inventory item with new "itemToCheck" quantity
    private int AddToExistingStacks(ItemData itemToCheck) // Step Two
    {
        for(int i = 0; i < chestSlots.Length; i++)
        {
            if(_remainingQuantity <= 0)
                break;

            if(!IsMatchingStackableSlot(i, itemToCheck))
                continue;

            int currentQuantity = chestSlots[i].InventoryItem.Quantity;
            int availableSpace = MAX_STACK_AMOUNT - currentQuantity;

            if(availableSpace > 0)
            {
                int amountToAdd = Mathf.Min(availableSpace, _remainingQuantity);
                chestSlots[i].InventoryItem.Quantity += amountToAdd;
                chestSlots[i].UpdateItemTextQuantity(chestSlots[i].InventoryItem.Quantity);

                _remainingQuantity -= amountToAdd;
            }
        }

        return _remainingQuantity;
    }

    //Check if chest slot item is stackable, not null, and shares same item type as itemToCheck
    private bool IsMatchingStackableSlot(int slotIndex, ItemData itemToCheck) //Step Three
    {
        return chestSlots[slotIndex].InventoryItem != null &&
            chestSlots[slotIndex].InventoryItem.IsThisAStackableItem &&
            chestSlots[slotIndex].InventoryItem.ItemType == itemToCheck.ItemType;
    }

    //Create new chest item stack for itemToCheck in chest slot
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

    //Create new chest item overflow stacks for itemToCheck in chest slots
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