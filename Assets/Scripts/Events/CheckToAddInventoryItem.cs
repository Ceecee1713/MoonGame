using System.Collections.Generic;
using UnityEngine;

//This contains all the events (data types) that the game uses

//Inventory and Crafting System Events below:
//For picking up an item in the world to add to inventory
public class CheckToAddInventoryItem : IEvent 
{
    public ItemData InventoryItem;

    public CheckToAddInventoryItem(ItemData inventoryItem)
    {
        InventoryItem = inventoryItem;
    }
}

//For adding a crafted item into the inventory and removing items already in the inventory 
//that's been used as crafting materials
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

//To use an inventory item (the equipped inventory item) on the selected inventory slot
public class UseInventoryItem : IEvent
{
}

//To drop the currently equipped inventory item from the selected inventory slot
public class DropEquipedInventoryItem : IEvent 
{
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

//To display a warning message for the crafting UI 
//if crafting materials aren't insufficient to craft something
public class StopCraftingTemporarily : IEvent
{
    public bool ShowingWarningMessage;

    public StopCraftingTemporarily(bool showingWarningMessage)
    {
        ShowingWarningMessage = showingWarningMessage;
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

//For removing inventory items from the inventory that were 
//used as crafting materials to decipher a clue on the crafting table
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

//Tracking the current dialogue in the Moon Puzzle Text Adventure
//To determine the next dialogue to say if the player guesses correctly
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

//Cluebook Events below:
//To add a clue fragment into the cluebook after interacting with an NPC that gives a clue fragment
public class FoundClueFragment : IEvent
{
    public Dialogue ClueDialogue;

    public FoundClueFragment(Dialogue clueDialogue)
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
    public bool SolvedMoonPuzzle;
    public bool PromptTextAdventure;

    public ChangeCanvases(GameObject newCanvas, bool solvedMoonPuzzle, bool promptTextAdventure)
    {
        NewCanvas = newCanvas;
        SolvedMoonPuzzle = solvedMoonPuzzle;
        PromptTextAdventure = promptTextAdventure;
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
public class TypeOutSingleDialogue : IEvent
{
    public string Message;

    public TypeOutSingleDialogue(string message)
    {
        Message = message;
    }
} 

//Advance through the single dialogue to the UI responsible for handling single dialogues
public class AdvanceSingleMessage : IEvent
{
}

//Player events below:
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
public class Interact : IEvent //Interacting with objects in world space
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
