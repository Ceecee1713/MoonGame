using UnityEngine;

public class ChestUI : MonoBehaviour
{
    [SerializeField]
    private ChestSlot [] chestSlots;

    private int _maxStackAmount = 3;

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
    }
}
