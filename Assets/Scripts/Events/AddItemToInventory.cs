using System.Collections.Generic;
using UnityEngine;

//This contains all the events (data types) that the game uses

public class CompletedAllMoonPuzzles : IEvent //Game Manager
{
}

#region Inventory and Crafting System Events

public class AddItemToInventory : IEvent
{
    public ItemData InventoryItem;

    public AddItemToInventory(ItemData inventoryItem)
    {
        InventoryItem = inventoryItem;
    }
}


//Remove inventory items from inventory that were used as materials for crafting
public class RemoveUsedMaterials : IEvent 
{
    public List <ItemData> CraftingMaterialItems;
    public List <int> AmountsPerStackableItemToRemove;

    public RemoveUsedMaterials(List <ItemData> craftingMaterialItems, List <int> amountsPerStackableItemToRemove)
    {
        CraftingMaterialItems = craftingMaterialItems;
        AmountsPerStackableItemToRemove = amountsPerStackableItemToRemove;
    }
}


//Selecting on an inventory slot on inventory UI and equiping that selected inventory slot's item 
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


//To use an inventory item (the equipped inventory item) on the selected inventory slot,
//clear the selected inventory slot and remove that item from inventory (DELETE COMMENT AFTER WRITING FULL DOCUMENTATION)
public class UseInventoryItem : IEvent
{
}


//To drop the currently equipped inventory item from the selected inventory slot,
//clear the selected inventory slot and remove that item from inventory (DELETE COMMENT AFTER WRITING FULL DOCUMENTATION)
public class DropEquipedInventoryItem : IEvent 
{
}


//Clear the selected inventory slot and remove that item from the inventory when adding an item to a chest 
public class RemoveItemFromSlot : IEvent 
{
    public InventoryUISlot InventorySlot;

    public RemoveItemFromSlot(InventoryUISlot inventorySlot)
    {
        InventorySlot = inventorySlot;
    }
}


//Adjusting an inventory item's quantity in an inventory UI slot and in inventory.
//Manage quantities between what's in inventory and what will be used as crafting materials 
public class AdjustInventorySlotItemQuantity : IEvent 
{
    public int NewQuantity;
    public int InventoryIndex;

    public AdjustInventorySlotItemQuantity(int newQuantity, int inventoryIndex)
    {
        NewQuantity = newQuantity;
        InventoryIndex = inventoryIndex;
    }
}


//Instantiate an inventory item into the world after dropping it from the inventory slot
public class SpawnDroppedInventoryItem : IEvent
{
    public ItemData InventoryItem;

    public SpawnDroppedInventoryItem(ItemData inventoryItem)
    {
        InventoryItem = inventoryItem;
    }
}


//Checking if a clue has been resolved from the clue book BEFORE permitting to decipher clue at crafting table
public class AllowToCraftClue : IEvent
{
    public bool AvaliableClueToDecipher;

    public AllowToCraftClue(bool avaliableClueToDecipher)
    {
        AvaliableClueToDecipher = avaliableClueToDecipher;
    }
}
#endregion


#region Text System Events 

//Starting a Moon Puzzle Text Adventure (prepare first dialogue for any moon puzzle)
public class StartNewTextAdventure : IEvent
{
}


//Progressing through dialogue for both Moon Puzzle and Storytelling
public class AdvanceThroughTextAdventure : IEvent
{
}

//Interacting with moon statue 
public class StartPrayerPhase : IEvent
{
}


public class StartEndGameDialogue : IEvent
{
}


//Tracking the current dialogue in the Moon Puzzle Text Adventure
//To determine the next dialogue to say if the player guesses correctly
public class SetMoonPuzzleQuestions : IEvent
{
    public MoonPuzzleDialogueData QuestionDialogue;
    public int TextBranchIndex;

    public SetMoonPuzzleQuestions(MoonPuzzleDialogueData questionDialogue, int textBranchIndex)
    {
        QuestionDialogue = questionDialogue;
        TextBranchIndex = textBranchIndex;
    }
}
#endregion


#region Cluebook Events

public class FoundClueFragment : IEvent
{
    public string ClueDialogue;

    public FoundClueFragment(string clueDialogue)
    {
        ClueDialogue = clueDialogue;
    }
}


public class DecipherClue : IEvent
{
}


//To check if any of the clues are complete 
//Permitting whether decipering a clue can be done or not 
public class CheckForCompleteClues : IEvent
{
}
#endregion


#region UI Events

//Alternate between canvases through a transistion
public class ChangeCanvases : IEvent
{
    public GameObject NewCanvas;
    public bool StartMoonPuzzle;
    public bool StartPrayerPhase;

    public ChangeCanvases(GameObject newCanvas, bool startMoonPuzzle, bool startPrayerPhase)
    {
        NewCanvas = newCanvas;
        StartMoonPuzzle = startMoonPuzzle;
        StartPrayerPhase = startPrayerPhase;
    }
}


//Fade the current active canvas with a black screen for a transistion
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


//Assign a single dialogue to the UI responsible for handling single dialogues 
public class TypeDialogueOnMainUI : IEvent
{
    public StorytellingDialogueData Dialogue;
    public bool NewExplorationPhase; 
    public bool StartingTheGame; 

    public TypeDialogueOnMainUI(StorytellingDialogueData dialogue, bool newExplorationPhase, bool startingTheGame)
    {
        Dialogue = dialogue;
        NewExplorationPhase = newExplorationPhase;
        StartingTheGame = startingTheGame;
    }
} 


//Advance through the single dialogue to the UI responsible for handling single dialogues
public class AdvanceDialogueOnMainUI : IEvent
{
}


//(DELETE COMMENT AFTER WRITING FULL DOCUMENTATION)
//For a new exploration phase ONLY AFTER completing a moon puzzle text adventure:
//Show dialogue from the moon statue, change materials for deciphering clue on craft manager,
//destroy NPCs in that completed puzzle area AND change collisions (safe zone and corrioson areas)
public class NewMoonFragmentObtained : IEvent 
{
}


public class ResetExplorationTimer : IEvent
{
}


public class StartBeginnerTutorial : IEvent
{
}

public class ShowBeginnerGoal : IEvent
{
}


public class PauseExplorationTimer : IEvent
{
    public bool AllowCountdown;

    public PauseExplorationTimer(bool allowCountdown)
    {
        AllowCountdown = allowCountdown;
    }
}
#endregion


#region Player Events

//Prevent player interactions with specific UI/object interactions
public class ActivatePlayerInputs : IEvent
{
    public bool AllowInputs;

    public ActivatePlayerInputs(bool allowInputs)
    {
        AllowInputs = allowInputs;
    }
}


//Either incrase or lower the player's health based on environment collisions
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


//Increase the speed of the player after using a speed-up item from inventory slot
public class SpeedUpPlayer : IEvent
{
}
#endregion


#region Player Input Events

public class Interact : IEvent 
{
}


//Prevent the player from using an equpped inventory item when in collision with an object
public class InCollision : IEvent
{
    public bool PlayerInCollision;

    public InCollision(bool playerInCollision)
    {
        PlayerInCollision = playerInCollision;
    }
}


//Either momentarily pause the dropping of the player's health or not
public class MaintainPlayerHealth : IEvent
{
    public bool PauseCorrioson;

    public MaintainPlayerHealth(bool pauseCorrioson)
    {
        PauseCorrioson = pauseCorrioson;
    }
}


public class PauseGame : IEvent
{
}
#endregion


#region Chest Interaction Events
public class ChestIsOpen : IEvent
{
    public bool IsAChestOpen;

    public ChestIsOpen(bool isAChestOpen)
    {
        IsAChestOpen = isAChestOpen;
    }
}

public class CheckToAddItemToChest : IEvent 
{
    public ItemData InventoryItem;

    public CheckToAddItemToChest(ItemData inventoryItem)
    {
        InventoryItem = inventoryItem;
    }
}
#endregion


#region Environment Events

public class ChangeCorriosonValue : IEvent
{
    public float CorriosonValue;
    public int CorriosonAreaNumber;

    public ChangeCorriosonValue(float corriosonValue, int corriosonAreaNumber)
    {
        CorriosonValue = corriosonValue;
        CorriosonAreaNumber = corriosonAreaNumber;
    }
}

public class RestoreCorriosonValue : IEvent
{
}

public class ChangeThirdCorriosonAreaValue : IEvent
{
}

//(DELETE COMMENT AFTER WRITING FULL DOCUMENTATION)
//Teleport player back to moon puzzle, reset player health to full AND delete item drops
//Published for EACH new exploration phase, not just for finishing a moon puzzle
public class NewExplorationPhase : IEvent
{
}

public class ResetWorldItems : IEvent
{
}

public class StopMoonStatueSpin : IEvent
{
}

public class MakeMoonStatueSpin : IEvent
{
}

//For LightPropMoon and StreetLamps scripts 
public class NewAreaChange : IEvent 
{
    public int AreaChangesCount;

    public NewAreaChange(int areaChangesCount)
    {
        AreaChangesCount = areaChangesCount;
    }
}

#endregion
