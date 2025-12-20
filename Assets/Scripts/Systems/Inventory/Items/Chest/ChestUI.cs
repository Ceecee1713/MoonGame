using UnityEngine;

public class ChestUI : MonoBehaviour
{
    [SerializeField]
    private ChestSlot [] chestSlots;

    private const int MAX_STACK_AMOUNT = 9;

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

    private void AddInventoryItem(ItemData itemToCheck) //Add an inventory item (Crafted item or not)
    {
        /* //Dec 18
        for(int i = 0; i < chestSlots.Length; i++) //Add same type, stackable items together in same inventory slot
        {
            if(chestSlots[i].InventoryItem.IsThisAStackableItem == true && itemToCheck.IsThisAStackableItem == true)
            {
                if(chestSlots[i].InventoryItem.ItemType == itemToCheck.ItemType)
                {
                    //If the inventory slot's item's quantity isn't above "_maxStackAmount" (increase quantity)
                    if(chestSlots[i].InventoryItem.Quantity < _maxStackAmount) 
                    {
                        chestSlots[i].InventoryItem.Quantity++; 
                        return;
                    }
                } 
            }
        }

        //Add new inventory item in any empty inventory slot, whether item is stackable or not
        for(int i = 0; i < chestSlots.Length; i++) 
        {
            if(chestSlots[i].IsEmpty == true)
            {
                chestSlots[i].AddItemToSlot(itemToCheck);
                break;
            }
        }
        */

        int remainingQuantity = itemToCheck.Quantity;

        //Add quantity of newly added item
        //Into existing items of the same type in the inventory
        if(itemToCheck.IsThisAStackableItem == true)
        {
            for(int i = 0; i < chestSlots.Length; i++) 
            {
                if(remainingQuantity <= 0)
                    break;

                if(chestSlots[i].InventoryItem.IsThisAStackableItem == true && chestSlots[i].InventoryItem.ItemType == itemToCheck.ItemType)
                {
                    int currentQuantity = chestSlots[i].InventoryItem.Quantity;
                    
                    //Calculate how much space is available in this item 
                    int availableSpace = MAX_STACK_AMOUNT - currentQuantity;
                    
                    if(availableSpace > 0)
                    {
                        //Calculate how much quantity can add to this item 
                        int amountToAdd = Mathf.Min(availableSpace, remainingQuantity);
                        
                        //Add quantity to existing item 
                        chestSlots[i].InventoryItem.Quantity += amountToAdd;
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
            
            for(int i = 0; i < chestSlots.Length; i++) 
            {
                if(chestSlots[i].IsEmpty == true)
                {
                    //For the first new item, use the original item
                    if(remainingQuantity == itemToCheck.Quantity)
                    {
                        int quantityForThisStack = Mathf.Min(remainingQuantity, MAX_STACK_AMOUNT);
                        itemToCheck.Quantity = quantityForThisStack;
                        
                        chestSlots[i].AddItemToSlot(itemToCheck);
                        remainingQuantity -= quantityForThisStack;
                    }

                    //Clone for additional overflow quantity
                    else
                    {
                        ItemData itemToCheckClone = itemToCheck.Clone();
                        itemToCheckClone.Quantity = Mathf.Min(remainingQuantity, MAX_STACK_AMOUNT);
                        
                        chestSlots[i].AddItemToSlot(itemToCheckClone);
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
}
