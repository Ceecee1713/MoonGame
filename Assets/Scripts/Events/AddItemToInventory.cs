using System.Collections.Generic;
using UnityEngine;

//This contains all the events (data types) that the game uses
//Scroll down to find the event you want. Events are separated into categorial regions.
//Some events have multiple publishers and subscribers:
//4+ Publishers:PreventPlayerInteractingWithInventory, ChangeCanvases, FreezePlayer, PauseExplorationTimer, MaintainPlayerHealth
//4+ Subscribers: NewMoonFragmentObtained, Interact
//4+ Publishers AND Subscribers: ActivatePlayerInputs

//Subscribers: GameManager
//Publishers: MoonPuzzleDialogueText
//Purpose: Signals that all three moon puzzles have been completed, prompting GameManager to show the end game dialogue
public class CompletedAllMoonPuzzles : IEvent
{
}

#region Inventory and Crafting System Events

//Subscribers: InventoryUI
//Publishers: InteractableItem, ChestSlot, CraftManager
//Purpose: Adds an inventory item into the player's inventory
public class AddItemToInventory : IEvent
{
    public ItemData InventoryItem;

    public AddItemToInventory(ItemData inventoryItem)
    {
        InventoryItem = inventoryItem;
    }
}


//Subscribers: InventoryUI
//Publishers: CraftManager
//Purpose: Removes inventory items from the player's inventory that were used as crafting materials
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


//Subscribers: InventoryUI
//Publishers: InventoryUISlot
//Purpose: Equips the inventory item from the currently selected inventory UI slot
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


//Subscribers: InventoryUI
//Publishers: PlayerInputController
//Purpose: Uses the currently equipped inventory item, removing it from the selected inventory slot (reserved for speed potions)
public class UseInventoryItem : IEvent
{
}


//Subscribers: InventoryUI
//Publishers: PlayerInputController
//Purpose: Drops the currently equipped inventory item from the selected inventory slot and removes it from inventory
public class DropEquipedInventoryItem : IEvent 
{
}


//Subscribers: InventoryUI
//Publishers: InventoryUISlot
//Purpose: Clears the selected inventory slot and removes that item from inventory when it's moved into a chest
public class RemoveItemFromSlot : IEvent 
{
    public InventoryUISlot InventorySlot;

    public RemoveItemFromSlot(InventoryUISlot inventorySlot)
    {
        InventorySlot = inventorySlot;
    }
}


//Subscribers: InventoryUI
//Publishers: CraftManager
//Purpose: Adjusts an inventory item's leftover quantity in an inventory UI slot and in inventory data after crafting
public class AdjustInventorySlotItemQuantity : IEvent 
{
    public int NewQuantity;
    public InventoryItemTypes ItemType;

    public AdjustInventorySlotItemQuantity(int newQuantity, InventoryItemTypes itemType)
    {
        NewQuantity = newQuantity;
        ItemType = itemType;
    }
}


//Subscribers: PlayerSpawner
//Publishers: InventoryUISlot
//Purpose: Instantiates an inventory item into the world after it's been dropped from the inventory slot
public class SpawnDroppedInventoryItem : IEvent
{
    public ItemData InventoryItem;

    public SpawnDroppedInventoryItem(ItemData inventoryItem)
    {
        InventoryItem = inventoryItem;
    }
}


//Subscribers: CraftManager
//Publishers: CluebookManager
//Purpose: Indicates whether a completed, gibberish clue is available to be deciphered before permitting crafting
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

//Subscribers: MoonPuzzleDialogueText
//Publishers: CanvasManager
//Purpose: Prepares and starts the first dialogue for a moon puzzle text adventure
public class StartNewTextAdventure : IEvent
{
}


//Subscribers: MoonPuzzleDialogueText, StorytellingDialogueText
//Publishers: PlayerInputController
//Purpose: Advances through dialogue for both the moon puzzle and storytelling text adventures
public class AdvanceThroughTextAdventure : IEvent
{
}


//Subscribers: StorytellingDialogueText
//Publishers: CanvasManager
//Purpose: Begins the prayer phase dialogue when interacting with the moon statue
public class StartPrayerPhase : IEvent
{
}


//Subscribers: StorytellingDialogueText
//Publishers: GameManager
//Purpose: Begins the end game dialogue on the storytelling UI
public class StartEndGameDialogue : IEvent
{
}


//Subscribers: MoonTextAdventureButton
//Publishers: MoonPuzzleDialogueText
//Purpose: Tracks the current question dialogue in the moon puzzle text adventure to determine the next dialogue if the player guesses correctly
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

//Subscribers: CluebookManager
//Publishers: NPC
//Purpose: Marks a clue fragment as found and updates the matching clue's text in the cluebook
public class FoundClueFragment : IEvent
{
    public string ClueDialogue;

    public FoundClueFragment(string clueDialogue)
    {
        ClueDialogue = clueDialogue;
    }
}


//Subscribers: CluebookManager
//Publishers: CraftManager
//Purpose: Reveals the deciphered, readable text of a completed clue
public class DecipherClue : IEvent
{
}


//Subscribers: CluebookManager
//Publishers: DecipherClueButton
//Purpose: Checks whether any clue is fully complete in its gibberish form, permitting whether deciphering can be done
public class CheckForCompleteClues : IEvent
{
}
#endregion


#region UI Events

//Subscribers: CanvasManager
//Publishers: GameManager, MoonPuzzleDialogueText, AcceptButton, PrayToMoonStatue, StorytellingDialogueText, PlayerHealth
//Purpose: Swaps the currently active UI canvas with another through a fade transition
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


//Subscribers: CanvasManager
//Publishers: MoonPuzzleDialogueText
//Purpose: Fades the current active UI canvas in or out
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


//Subscribers: DialogueCanvas
//Publishers: GameManager, MoonVisibility, NPC, PrayToMoonStatue, FirstSafeZone
//Purpose: Assigns and types out a single dialogue on the dialogue UI layered on top of the main player UI
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

//Subscribers: DialogueCanvas
//Publishers: PlayerInputController
//Purpose: Advances through the single dialogue displayed on the dialogue UI layered on top of the main player UI
public class AdvanceDialogueOnMainUI : IEvent
{
}

//Subscribers: CorriosonZone, FirstSafeZone, SafeZone, MoonVisibility, GameManager, NPC, GoalText
//Publishers: MoonPuzzleDialogueText
//Purpose: Signals that a moon puzzle has been completed, triggering zone swaps, deleting of NPCs, lighting changes, statue visuals, and goal text updates
public class NewMoonFragmentObtained : IEvent 
{
}

//Subscribers: ExplorationTimer
//Publishers: StorytellingDialogueText, DialogueCanvas
//Purpose: Resets the exploration timer countdown back to its maximum duration
public class ResetExplorationTimer : IEvent
{
}

//Subscribers: GameManager
//Publishers: StorytellingDialogueText
//Purpose: Prompts the beginning tutorial dialogue to be shown on the dialogue canvas
public class StartBeginnerTutorial : IEvent
{
}

//Subscribers: GoalText
//Publishers: DialogueCanvas
//Purpose: Displays the beginning goal text on the main player UI
public class ShowBeginnerGoal : IEvent
{
}

//Subscribers: WarningMoonPuzzleUI
//Publishers: MoonPuzzleArea
//Purpose: Displays the warning UI screen before starting a moon puzzle text adventure
public class OpenWarningMoonPuzzleUI : IEvent
{
    public int CurrentMoonPuzzleAreaNumber;

    public OpenWarningMoonPuzzleUI(int currentMoonPuzzleAreaNumber)
    {
        CurrentMoonPuzzleAreaNumber = currentMoonPuzzleAreaNumber;
    }
}

//Subscribers: ExplorationTimer
//Publishers: ChestUI, CluebookUI, PauseMenu, EndScreenUI, MoonPuzzleArea, MoonPuzzleDialogueText, 
//StorytellingDialogueText, DialogueCanvas, PlayerHealth, DeclineButton, CraftingTable
//Purpose: Pauses or unpauses the exploration timer countdown
public class PauseExplorationTimer : IEvent
{
    public bool AllowCountdown;

    public PauseExplorationTimer(bool allowCountdown)
    {
        AllowCountdown = allowCountdown;
    }
}

//Subscribers: CameraManager
//Publishers: FreezeWithActiveUI, EndScreenUI
//Purpose: Freeze the camera when a specific UI is active
public class FreezeCameraWithActiveUI : IEvent
{
    public GameObject CurrentUI;
    public bool FreezeCamera;

    public FreezeCameraWithActiveUI(GameObject currentUI, bool freezeCamera)
    {
        CurrentUI = currentUI;
        FreezeCamera = freezeCamera;
    }
}

//Subscribers: PlayerInputController
//Publishers: EndScreenUI
//Purpose: Disable all player inputs when the player has reached the end screen and can no longer play the game anymore
public class StopAllPlayerInputs : IEvent
{
}
#endregion


#region Player Events

//Subscribers: CraftingTable, InventoryUI, InventoryUISlot, InteractableItem, NPC, PrayToMoonStatue, MoonPuzzleArea
//Publishers: CraftManager, DialogueCanvas, PlayerHealth, PauseMenu, CluebookUI
//Purpose: Allows or prevents the player from interacting with world objects and UI
public class ActivatePlayerInputs : IEvent
{
    public bool AllowInputs;

    public ActivatePlayerInputs(bool allowInputs)
    {
        AllowInputs = allowInputs;
    }
}

//Subscribers: PlayerHealth
//Publishers: FirstSafeZone, SafeZone, CorriosonZone
//Purpose: Increases or lowers the player's health based on environment collisions
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

//Subscribers: PlayerStateMachine
//Publishers: NPC, PrayToMoonStatue, ChestUI, CluebookUI, PauseMenu, EndScreenUI, MoonPuzzleArea, 
//FirstSafeZone, StorytellingDialogueText, DialogueCanvas, PlayerHealth, DeclineButton
//Purpose: Freezes or unfreezes the player's movement
public class FreezePlayer : IEvent
{
    public bool PausePlayerMovement;

    public FreezePlayer(bool pausePlayerMovement)
    {
        PausePlayerMovement = pausePlayerMovement;
    }
}

//Subscribers: PlayerStateMachine
//Publishers: InventoryUI
//Purpose: Increases the speed of the player after using a speed-up item from an inventory slot
public class SpeedUpPlayer : IEvent
{
}
#endregion


#region Player Input Events

//Subscribers: NPC, PrayToMoonStatue, ChestInteraction, CraftingTable, InteractableItem, MoonPuzzleArea
//Publishers: PlayerInputController
//Purpose: Triggers an interaction with whichever interactable object the player is currently colliding with
public class Interact : IEvent 
{
}

//Subscribers: InventoryUI
//Publishers: NPC, PrayToMoonStatue, ChestInteraction, CraftingTable, InteractableItem, MoonPuzzleArea
//Purpose: Prevents or allows the player from using an equipped inventory item while in collision with certain objects
public class PreventPlayerInteractingWithInventory : IEvent
{
    public bool PlayerInCollision;

    public PreventPlayerInteractingWithInventory(bool playerInCollision)
    {
        PlayerInCollision = playerInCollision;
    }
}

//Subscribers: PlayerHealth
//Publishers: NPC, ChestUI, CluebookUI, PauseMenu, EndScreenUI, MoonPuzzleArea, StorytellingDialogueText, DialogueCanvas, DeclineButton
//Purpose: Pauses or resumes the continuous dropping of the player's health
public class MaintainPlayerHealth : IEvent
{
    public bool PauseCorrioson;

    public MaintainPlayerHealth(bool pauseCorrioson)
    {
        PauseCorrioson = pauseCorrioson;
    }
}

//Subscribers: PauseMenu
//Publishers: PlayerInputController
//Purpose: Displays the pause menu if no other blocking UI is currently active
public class PauseGame : IEvent
{
}


#endregion


#region Chest Interaction Events

//Subscribers: InventoryUISlot, OpenCluebookMainUI
//Publishers: ChestInteraction, ChestButton
//Purpose: Marks whether a chest is currently open, changing input behaviour for inventory slots and the cluebook button
public class ChestIsOpen : IEvent
{
    public bool IsAChestOpen;

    public ChestIsOpen(bool isAChestOpen)
    {
        IsAChestOpen = isAChestOpen;
    }
}

//Subscribers: ChestUI
//Publishers: InventoryUISlot
//Purpose: Adds an inventory item into the chest's inventory
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

//Subscribers: CorriosonZone
//Publishers: StorytellingDialogueText, ExplorationTimer
//Purpose: Changes the speed at which a specific corrosion zone lowers the player's health
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

//Subscribers: CorriosonZone
//Publishers: StorytellingDialogueText
//Purpose: Restores a corrosion zone's speed back to its default value
public class RestoreCorriosonValue : IEvent
{
}

//Subscribers: CorriosonZone
//Publishers: GameManager
//Purpose: Allows the third corrosion zone's values to be altered after the second moon puzzle area is completed
public class ChangeThirdCorriosonAreaValue : IEvent
{
}

//Subscribers: PlayerStateMachine, ItemDrop, GoalText, PlayerHealth
//Publishers: MoonPuzzleDialogueText, StorytellingDialogueText
//Purpose: Signals the start of a new exploration phase, resetting player position and health, clearing dropped items, and updating goal text
public class NewExplorationPhase : IEvent
{
}

//Subscribers: InteractableItem
//Publishers: MoonPuzzleDialogueText, StorytellingDialogueText
//Purpose: Resets the activeness of certain interactable game objects for a new exploration phase
public class ResetWorldItemsActiveness : IEvent
{
}

//Subscribers: MoonVisibility
//Publishers: GameManager
//Purpose: Stops the moon statue from spinning and stops its sparkle particle effects
public class StopMoonStatueSpin : IEvent
{
}

//Subscribers: MoonVisibility
//Publishers: MoonPuzzleDialogueText
//Purpose: Starts the moon statue spinning along with its sparkle particle effects after all moon puzzles are complete
public class MakeMoonStatueSpin : IEvent
{
}

//Subscribers: LightPropMoon, StreetLamps
//Publishers: GameManager
//Purpose: Illuminates street lights and changes materials on decorative moon statues as puzzle areas are completed
public class NewAreaChange : IEvent 
{
    public int AreaChangesCount;

    public NewAreaChange(int areaChangesCount)
    {
        AreaChangesCount = areaChangesCount;
    }
}

//Subscribers: MoonPuzzleArea
//Publishers: WarningMoonPuzzleUI
//Purpose: Stops the audio acting as sound for the sparkle particle effects at a moon puzzle area
public class StopMoonPuzzleAreaAudio : IEvent 
{
    public int CurrentMoonPuzzleAreaNumber;

    public StopMoonPuzzleAreaAudio(int currentMoonPuzzleAreaNumber)
    {
        CurrentMoonPuzzleAreaNumber = currentMoonPuzzleAreaNumber;
    }
}

#endregion