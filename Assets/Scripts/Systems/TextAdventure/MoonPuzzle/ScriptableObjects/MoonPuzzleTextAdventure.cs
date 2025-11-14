using System;
using UnityEngine;
using UnityEngine.UI;

//This scriptable object holds all the three unique moon puzzle text adventures.
//Each "TextBranch" is one unique moon puzzle text adventure.

[CreateAssetMenu(fileName = "MoonPuzzleTextAdventure", menuName = "Dialogue Scriptable Objects/Create a Complete Moon Puzzle Text Adventure Sequence")]
public class MoonPuzzleTextAdventure : ScriptableObject
{
    public TextBranches [] TextBranches = new TextBranches[3];
}

[Serializable]
public struct TextBranches
{
    public MoonPuzzleDialogueData FirstQuestionDialogue;
    public MoonPuzzleDialogueData SecondQuestionDialogue;
    public MoonPuzzleDialogueData ThirdQuestionDialogue;
    public MoonPuzzleDialogueData FinishTextAdventureDialogue;

    public Sprite MoonFragmentSprite;
}
