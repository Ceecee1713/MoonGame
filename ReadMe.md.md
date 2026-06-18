# Moon Game

A crafting and exploration game set within a single, contained terrain.

## Overview

The player explores a small open world, gathering resources and crafting items to bring back light to their village tied to three moon puzzle areas. Progress is driven by a day/night-style loop: each Exploration Phase ends with a prayer to the Moon Statue called the Prayer Phase, advancing the story and unlocking new areas, materials, and goals. Along the way, the player collects clue fragments for a cluebook to aid them in the various moon puzzles, deciphers them at a crafting table, and manages a corrosion mechanic that continuously affects their health depending on location and story progress.

## Built With

- Unity 6000.0.68f1 (URP)

**Non-Default Packages:**
- DOTween (Imported)
- AI Navigation 2.0.10
- Cinemachine 3.1.4
- Custom NUnit 2.6.2
- Input System 1.18.0
- Mono Cecil 1.11.5
- Performance Testing API 3.2.0
- Shader Graph 17.0.4
- Splines 2.8.2
- Unity Light Transport Library 1.0.1
- Unity Version Control 2.0.0

Do note: Originally, the repo was made using an older version of Unity but then was changed to using the newer versions as seen through my Git branches swapping between versions

## Project Structure

- `Assets/Scripts/` — all C# scripts, organized by system (Inventory, Crafting, Cluebook, Moon Puzzles, Storytelling, Environment, UI, Player)
- `Assets/Scripts/AddItemToInventory.cs` — central file containing every event used by the EventBus, with comments documenting each event's publishers, subscribers, and purpose
- `Assets/Prefabs/` — reusable game objects (interactable items, UI elements, environment pieces)
- `Assets/ScriptableObjects/` — dialogue data, item data, moon puzzle data, and other shared data assets shared across multiple scripts
- `Assets/Utilities/` — containing the input system mapping for the game

## Architecture

The game is built around an event-driven architecture using a central `EventBus` (`EventBus.cs`). Scripts communicate by publishing and subscribing to events rather than calling each other directly, which keeps systems loosely coupled. Scripts only call each other directly when they're attached to the same parent game object. `IEvent` is within `EventBus.cs`.

- All events are defined as classes implementing `IEvent` inside `AddItemToInventory.cs`
- Each event in `AddItemToInventory.cs` is documented with its publishers, subscribers, and purpose
- Scripts that depend on other scripts document those relationships in their XML `<remarks>` blocks, including `<see cref>` references for quick navigation between related scripts

**Singletons:**
Some manager-style scripts inherit from a generic Singleton<T> base class (`Singleton.cs`), ensuring only one instance of that script exists in a scene and that it's globally accessible via ScriptName.Instance.
EventBus and AudioManager both inherit from Singleton<T>
Singleton<T> handles creating an instance automatically if one doesn't exist, destroying duplicate instances, and persisting across scene loads (unless _destroyOnLoad is set to true, as EventBus does)

**Player State Machine:**
The player's movement and behaviour are managed through a state machine pattern.
`BaseState` (in the StateMachine namespace) defines the interface every state implements: Enter(), Update(), Exit(), and FixedUpdate()
`BaseStateMachine` holds the current state and handles transitioning between states via ChangeState(), calling Exit() on the old state and Enter() on the new one
PlayerStateMachine inherits from BaseStateMachine and manages the player's specific states: PlayerIdleState, PlayerWanderState, and PlayerPauseState
Each player state implements BaseState and contains the logic for that specific behaviour (idling, wandering/moving, or being paused/frozen)

`EventBus`, `BaseStateMachine`, `BaseState`, `Singleton.cs` are all within a folder called Core (Assets -> Scripts -> Core)
`AddItemToInventory.cs` is within a folder called Events (Assets -> Scripts -> Events)

## Key Systems

**Inventory & Crafting** — `InventoryUI` and `InventoryUISlot` manage the player's inventory, inventory display and item handling. 
`CraftManager`, `CraftButton`, and `DecipherClueButton` handle checking inventory against crafting recipes and crafting items or deciphering clues.

**Cluebook** — `CluebookManager` tracks 9 clues, each split into 2 gibberish fragments (18 total). 
Fragments are found via `NPC` interactions, and completed clues can be deciphered at the crafting table via `DecipherClueButton`.

**Moon Puzzles** — Three decorative moon statue areas (`MoonPuzzleArea`) each lead into a branching text adventure (`MoonPuzzleDialogueText`, `MoonTextAdventureButton`). Completing all three triggers the endgame sequence via `GameManager`.

**Storytelling & Dialogue** — `StorytellingDialogueText` and `DialogueCanvas` handle narrative dialogue, the prayer phase, and the day-to-day progression loop.
`GoalText` keeps the player's current objective updated on the main UI.

**Environment** — `CorriosonZone`, `SafeZone`, and `FirstSafeZone` manage zones that continuously raise or lower the player's health. 
`ExplorationTimer` tracks the time limit for each exploration phase, after which corrosion speeds increase.

**Player** — `PlayerStateMachine` manages player movement and state (idle, wandering, paused), `PlayerInputController` handles input and publishes player-driven events, 
and `PlayerHealth` manages the health slider and game-over conditions.

**UI** — `CanvasManager` handles fading and swapping between fullscreen UI canvases. 
Supporting UIs include the pause menu, chest UI, win/lose screens, and the warning UI shown before entering a moon puzzle.

## Setup

- Clone the repository
- Open the project in Unity (see `ProjectSettings` for the exact version used)
- Open Assets -> Scenes -> GameScenes -> StartScene
- Select scene named StartScene and press play 

Do note: You cannot directly play the scene where all the gameplay takes place in. The actual gameplay scene is called Game under GameScenes folder.
This is because there's no AudioManager existing in the scene. AudioManager is inside StartScene and will be persistent across scenes. 
StartScene is a scene that ONLY contains the start menu

## Known Issues

- Sometimes, the arragement of inventory items in the inventory display can be a little off after crafting items. 
For example, the crafted item does not go into the first inventory slot where the first inventory slot had an inventory item that was consumed as materials but are now removed

- `AddItemToInventory.cs` (which contains all event definitions) should be renamed to `Events.cs` to better reflect its contents

- When crafting items or deciphering clues at the crafting table, when the correct amount of materials are in inventory,
some materials in inventory will not be consumed for the crafting but still yield a crafted item or decipered clue. This happens when inventory is full or not full

- Sometimes, the dialogue in the storytelling UI doesn't fully clear upon first typing of a dialogue message. This means the first dialogue message may be the first message of the previous
dialogue from before instead of the new, current dialogue that's meant to be set to for the textmeshprougi

- Sometimes, if you proceed through the dialogue too fast, the dialogue may glitch and prompt different dialogue scenarios. 
For example, speeding through the prayer dialogue at the moon statue MAY trigger the beginning dialogue when you first start the game 

- You cannot craft items in a full inventory even if some materials in the full inventory will be consumed

- Sometimes, you may be unable to craft items even if you have the correct type(s) and amount(s) of crafting material. The best option is to drop those materials from inventory
and grab new items from around the world and craft your item with the newly aquired materials

## Notes

- Anything within Testing folder (Assets -> Scripts -> Testing) is not used in the actual game but were used during development in test scenes 
- "Corrosion" is mispelled as "Corrioson" throughout the codebase (variable names, class names, and comments)