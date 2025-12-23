using System.Collections.Generic;
using UnityEngine;

//This contains all the events (data types) that the game uses

//Inventory and Crafting System Events below:
public class AddItemToInventory : IEvent
{
    public ItemData InventoryItem;

    public AddItemToInventory(ItemData inventoryItem)
    {
        InventoryItem = inventoryItem;
    }
}

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

//For selecting on an inventory slot on the inventory UI 
//and equip the item in that selected inventory slot
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
//clear the selected inventory slot and remove that item from the inventory data
public class UseInventoryItem : IEvent
{
}

//To drop the currently equipped inventory item from the selected inventory slot,
//clear the selected inventory slot and remove that item from the inventory data
public class DropEquipedInventoryItem : IEvent 
{
}

//Clear the selected inventory slot and remove that item from the inventory data
//This is used for adding an item to a chest but removing it from the inventory data and player's inventory
public class RemoveItemFromSlot : IEvent 
{
    public InventoryUISlot InventorySlot;

    public RemoveItemFromSlot(InventoryUISlot inventorySlot)
    {
        InventorySlot = inventorySlot;
    }
}

//To instantiate an inventory item into the world after dropping it from the inventory 
public class SpawnDroppedInventoryItem : IEvent
{
    public ItemData InventoryItem;

    public SpawnDroppedInventoryItem(ItemData inventoryItem)
    {
        InventoryItem = inventoryItem;
    }
}

//Checking if a clue has been resolved from the clue book 
//before being able to decipher it on the crafting table
public class AllowToCraftClue : IEvent
{
    public bool AvaliableClueToDecipher;

    public AllowToCraftClue(bool avaliableClueToDecipher)
    {
        AvaliableClueToDecipher = avaliableClueToDecipher;
    }
}





//Text System events below:
//Starting a Moon Puzzle Text Adventure (prepare the first dialogue)
public class StartNewTextAdventure : IEvent
{
}

//Progressing through dialogue in the Moon Puzzle Text Adventure
public class AdvanceTextAdventure : IEvent
{
}

//Showing a moon fragment UI with a changing sprite based on the Moon Puzzle
public class DisplayMoonFragmentImage : IEvent
{
    public Sprite MoonFragmentSprite;

    public DisplayMoonFragmentImage(Sprite moonFragmentSprite)
    {
        MoonFragmentSprite = moonFragmentSprite;
    }
}

public class StartPrayerPhase : IEvent
{
}

public class StartEndGameDialogue : IEvent //Not published by any script. Only inside StorytellingDialogueText  - Nov 25
{
}

//Tracking the current dialogue in the Moon Puzzle Text Adventure
//To determine the next dialogue to say if the player guesses correctly
public class SetTextAdventureQuestion : IEvent
{
    public MoonPuzzleDialogueData QuestionDialogue;
    public int TextBranchIndex;

    public SetTextAdventureQuestion(MoonPuzzleDialogueData questionDialogue, int textBranchIndex)
    {
        QuestionDialogue = questionDialogue;
        TextBranchIndex = textBranchIndex;
    }
}








//Cluebook Events below:
//To add a clue fragment into the cluebook after interacting with an NPC that gives a clue fragment
public class FoundClueFragment : IEvent
{
    public string ClueDialogue;

    public FoundClueFragment(string clueDialogue)
    {
        ClueDialogue = clueDialogue;
    }
}

//To decipher a clue in the cluebook 
//when the crafting table has checked it has enough crafting materials in the inventory 
public class DecipherClue : IEvent
{
}

//To check if any of the clues are complete 
//to allow for decipher (crafting) of the clue to happen
public class CheckForCompleteClues : IEvent
{
}







//UI events below:
//Change the current active canvas to "NewCanvas" with a fading black screen for a transistion
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

//Fade "CurrentCanvas" with a black screen for a transistion
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

//For a new exploration phase ONLY AFTER completing a moon puzzle text adventure:
//Show dialogue from the moon statue, change materials for deciphering clue on craft manager
//AND to destroy NPCs in that completed puzzle area
public class NewMoonFragmentObtained : IEvent 
{
}

//For a new exploration phase
public class ResetExplorationTimer : IEvent
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






//Player events below:
//Prevent player interactions with specific UI/object interactions
public class ActivatePlayerInputs : IEvent
{
    public bool AllowInputs;

    public ActivatePlayerInputs(bool allowInputs)
    {
        AllowInputs = allowInputs;
    }
}

//To either or lower the player's health based on environment collisions
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

//To stop the player from moving
public class FreezePlayer : IEvent
{
    public bool PausePlayerMovement;

    public FreezePlayer(bool pausePlayerMovement)
    {
        PausePlayerMovement = pausePlayerMovement;
    }
}

//To increase the speed of the player after using a speed-up item in the inventory
public class SpeedUpPlayer : IEvent
{
}





//Player input events below:
//Interacting with objects in world space
public class Interact : IEvent 
{
}

//To prevent the player from using an equpped inventory item when in collision with an object
public class InCollision : IEvent
{
    public bool PlayerInCollision;

    public InCollision(bool playerInCollision)
    {
        PlayerInCollision = playerInCollision;
    }
}

//To momentarily pause the dropping of the player's health
public class MaintainPlayerHealth : IEvent
{
    public bool PauseCorrioson;

    public MaintainPlayerHealth(bool pauseCorrioson)
    {
        PauseCorrioson = pauseCorrioson;
    }
}

//Open the Pause Menu UI
public class PauseGame : IEvent
{
}




//Chest Interaction Events Below:
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




//Environment Events Below:
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

//Teleport player back to moon puzzle, reset player health to full AND delete item drops
//Published for EACH new exploration phase, not just for finishing a moon puzzle
public class NewExplorationPhase : IEvent
{
}

public class CompletedAllMoonPuzzles : IEvent
{
}

public class ResetWorldItems : IEvent
{
}
