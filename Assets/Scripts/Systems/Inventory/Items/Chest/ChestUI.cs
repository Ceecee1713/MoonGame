using UnityEngine;

public class ChestUI : MonoBehaviour
{
    [SerializeField]
    private ChestSlot[] chestSlots;

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

    private void AddInventoryItem(ItemData itemToCheck)
    {
        int remainingQuantity = itemToCheck.Quantity;

        itemToCheck.IsDroppedItem = false;

        //Add quantity to existing stacks
        if(itemToCheck.IsThisAStackableItem == true)
        {
            for(int i = 0; i < chestSlots.Length; i++) 
            {
                if(remainingQuantity <= 0)
                    break;

                //If chest slot isn't null, if chest slot is stackable and if the chest slot's item type and new item type match
                if(chestSlots[i].InventoryItem != null && chestSlots[i].InventoryItem.IsThisAStackableItem == true && chestSlots[i].InventoryItem.ItemType == itemToCheck.ItemType)
                {
                    int currentQuantity = chestSlots[i].InventoryItem.Quantity;
                    int availableSpace = MAX_STACK_AMOUNT - currentQuantity;
                    
                    if(availableSpace > 0)
                    {
                        int amountToAdd = Mathf.Min(availableSpace, remainingQuantity);
                        chestSlots[i].InventoryItem.Quantity += amountToAdd;
                        remainingQuantity -= amountToAdd;
                    }
                }
            }
        }

        //Create a new stack for the same inventory type
        if(remainingQuantity > 0)
        {
            itemToCheck.Quantity = Mathf.Min(remainingQuantity, MAX_STACK_AMOUNT);
            
            for(int i = 0; i < chestSlots.Length; i++) 
            {
                if(chestSlots[i].IsEmpty == true)
                {
                    chestSlots[i].AddItemToSlot(itemToCheck);
                    remainingQuantity -= itemToCheck.Quantity;
                    break;
                }
            }
        }
        
        //Handling subsequent overflow of quantity stacking with clones 
        while(remainingQuantity > 0)
        {
            bool foundEmptySlot = false;
            
            for(int i = 0; i < chestSlots.Length; i++) 
            {
                if(chestSlots[i].IsEmpty == true)
                {
                    ItemData clonedItem = itemToCheck.Clone();
                    clonedItem.Quantity = Mathf.Min(remainingQuantity, MAX_STACK_AMOUNT);
                    clonedItem.IsDroppedItem = false; 
                    
                    chestSlots[i].AddItemToSlot(clonedItem);
                    remainingQuantity -= clonedItem.Quantity;
                    
                    foundEmptySlot = true;
                    break;
                }
            }
            
            if(!foundEmptySlot)
                break;
        }
    }
}