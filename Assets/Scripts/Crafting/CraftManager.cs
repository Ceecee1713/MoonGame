using System.Collections.Generic;
using UnityEngine;

public class CraftManager : MonoBehaviour
{
    [SerializeField]
    private InventoryData inventoryData;

    private List <int> checkedInventoryDataIndexes = new List <int>();
    private List <InventoryItem> allCraftingMaterials = new List <InventoryItem>();

    private bool _canCraft = false;
    private int _amountOfMatchingCraftingMaterials = 0;

    public void ResetStatus() //Called every click on a crafting button (once per click)
    {
        checkedInventoryDataIndexes.Clear();
        allCraftingMaterials.Clear();
        _amountOfMatchingCraftingMaterials = 0;
        _canCraft = false;
    }

    public void CheckInventoryForCraftingMaterials(InventoryItem craftableInventoryItem, InventoryItem craftingMaterial, int maxAmountOfCraftingMaterialTypes)
    {
        if(_canCraft == true)
            return;

        //Checking inventory data if it has the same inventory item data as "craftingMaterial's" inventory item data
        //And interate completely through inventory data's list for "maxAmountOfCraftingMaterialTypes" amount of times
        for(int i = 0; i < inventoryData.Inventory.Count; i++)
        {
            for(int j = 0; j < maxAmountOfCraftingMaterialTypes; j++)
            {
               if(inventoryData.Inventory[i].ItemData.ItemType == craftingMaterial.ItemData.ItemType && !checkedInventoryDataIndexes.Contains(i))
                {
                    _amountOfMatchingCraftingMaterials++;
                    checkedInventoryDataIndexes.Add(i); //Prevent checked inventory data indexes from being looked at again 
                    allCraftingMaterials.Add(craftingMaterial); 
                }  
            }
              
        }

        if(_amountOfMatchingCraftingMaterials >= maxAmountOfCraftingMaterialTypes)
        {
            _canCraft = true;
            CraftInventoryItem(craftableInventoryItem);
        }
    }

    private void CraftInventoryItem(InventoryItem craftableInventoryItem)
    {
        Debug.Log("We got enough materials to craft!");
        EventBus.Instance.Publish(new CheckToAddCraftedItem(craftableInventoryItem, allCraftingMaterials));
    }
}
