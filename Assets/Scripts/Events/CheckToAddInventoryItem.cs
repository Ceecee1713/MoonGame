using System.Collections.Generic;
using UnityEngine;

//This contains all the events (data types) that the game uses

//Inventory and Crafting System Events below

public class CheckToAddInventoryItem : IEvent 
{
    public ItemData InventoryItem;

    public CheckToAddInventoryItem(ItemData inventoryItem)
    {
        InventoryItem = inventoryItem;
    }
}

public class CheckToAddCraftedItem : IEvent 
{
    public ItemData InventoryItem;
    public List <ItemData> CraftingMaterialItems;
    public List <int> AmountsPerStackableItemToRemove;

    public CheckToAddCraftedItem(ItemData inventoryItem, List <ItemData> craftingMaterialItems, List <int> amountsPerStackableItemToRemove)
    {
        InventoryItem = inventoryItem;
        CraftingMaterialItems = craftingMaterialItems;
        AmountsPerStackableItemToRemove = amountsPerStackableItemToRemove;
    }
}

public class SelectInventoryItem : IEvent 
{
    public ItemData InventoryItem;
    public InventoryUISlot InventoryUISlot;

    public SelectInventoryItem(ItemData inventoryItem, InventoryUISlot inventoryUISlot)
    {
        InventoryItem = inventoryItem;
        InventoryUISlot = inventoryUISlot;
    }
}

public class SpawnDroppedInventoryItem : IEvent
{
    public ItemData InventoryItem;

    public SpawnDroppedInventoryItem(ItemData inventoryItem)
    {
        InventoryItem = inventoryItem;
    }
}

public class StopCraftingTemporarily : IEvent
{
    public bool ShowingWarningMessage;

    public StopCraftingTemporarily(bool showingWarningMessage)
    {
        ShowingWarningMessage = showingWarningMessage;
    }
}

public class AllowToCraftClue : IEvent
{
    public bool AvaliableClueToDecipher;

    public AllowToCraftClue(bool avaliableClueToDecipher)
    {
        AvaliableClueToDecipher = avaliableClueToDecipher;
    }
}

public class RemoveInventoryItemsForMaterials : IEvent
{
    public List <ItemData> CraftingMaterialItems;
    public List <int> AmountsPerStackableItemToRemove;

    public RemoveInventoryItemsForMaterials(List <ItemData> craftingMaterialItems, List <int> amountsPerStackableItemToRemove)
    {
        CraftingMaterialItems = craftingMaterialItems;
        AmountsPerStackableItemToRemove = amountsPerStackableItemToRemove;
    }
}

public class UseInventoryItem : IEvent
{

}

//Text System events below

public class StartNewTextAdventure : IEvent
{

}

public class AdvanceTextAdventure : IEvent
{

}

public class DisplayMoonFragmentImage : IEvent
{
    public Sprite MoonFragmentSprite;

    public DisplayMoonFragmentImage(Sprite moonFragmentSprite)
    {
        MoonFragmentSprite = moonFragmentSprite;
    }
}

public class SetTextAdventureQuestion : IEvent
{
    public DialogueData QuestionDialogue;
    public int TextBranchIndex;

    public SetTextAdventureQuestion(DialogueData questionDialogue, int textBranchIndex)
    {
        QuestionDialogue = questionDialogue;
        TextBranchIndex = textBranchIndex;
    }
}

//Cluebook Events below

public class FoundClueFragment : IEvent
{
    public Dialogue ClueDialogue;

    public FoundClueFragment(Dialogue clueDialogue)
    {
        ClueDialogue = clueDialogue;
    }
}

public class DecipherClue : IEvent
{
    
}

public class CheckForFinishedClues : IEvent
{
    
}


//UI events below

public class ChangeCanvases : IEvent
{
    public GameObject NewCanvas;
    public bool SolvedMoonPuzzle;
    public bool PromptTextAdventure;

    public ChangeCanvases(GameObject newCanvas, bool solvedMoonPuzzle, bool promptTextAdventure)
    {
        NewCanvas = newCanvas;
        SolvedMoonPuzzle = solvedMoonPuzzle;
        PromptTextAdventure = promptTextAdventure;
    }
}

public class FadeSingleCanvas : IEvent
{
    public GameObject CurrentCanvas;
    public bool FadeOutUI;

    public FadeSingleCanvas(GameObject currentCanvas, bool fadeOutUI)
    {
        CurrentCanvas = currentCanvas;
        FadeOutUI = fadeOutUI;
    }
}

public class SingleDialogueMessage : IEvent
{
    public string Message;

    public SingleDialogueMessage(string message)
    {
        Message = message;
    }
} 

public class AdvanceSingleMessage : IEvent
{

}

//Player events below

public class AlterPlayerHealth : IEvent
{
    public bool RecoverHealth;
    public float SpeedToChangeHealth;

    public AlterPlayerHealth(bool recoverHealth, float speedToChangeHealth)
    {
        RecoverHealth = recoverHealth;
        SpeedToChangeHealth = speedToChangeHealth;
    }
}

public class FreezePlayer : IEvent
{
    public bool PausePlayerMovement;

    public FreezePlayer(bool pausePlayerMovement)
    {
        PausePlayerMovement = pausePlayerMovement;
    }
}

public class SpeedUpPlayer : IEvent
{

}

//Player input events below

public class Interact : IEvent
{

}

public class DropEquipedInventoryItem : IEvent //For inventory system
{

}

//Mis.

public class InCollision : IEvent
{
    public bool PlayerInCollision;

    public InCollision(bool playerInCollision)
    {
        PlayerInCollision = playerInCollision;
    }
}

public class MaintainPlayerHealth : IEvent
{
    public bool PauseCorrioson;

    public MaintainPlayerHealth(bool pauseCorrioson)
    {
        PauseCorrioson = pauseCorrioson;
    }
}
