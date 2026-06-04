using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages ALL THREE moon puzzle dialogues and their messaging for the correct answer and incorrect answers
/// for the three choice buttons on the moon puzzle text adventure UI
/// </summary>

[CreateAssetMenu(fileName = "MoonPuzzleTextAdventure", menuName = "Dialogue Scriptable Objects/Create a Complete Moon Puzzle Text Adventure Sequence")]
public class MoonPuzzleTextAdventure : ScriptableObject
{
    public TextBranches [] TextBranches = new TextBranches[3];
}

/// <summary> Each "TextBranch" is one unique moon puzzle text adventure. </summary>
[Serializable]
public struct TextBranches
{
    public MoonPuzzleDialogueData FirstQuestionDialogue;
    public MoonPuzzleDialogueData SecondQuestionDialogue;
    public MoonPuzzleDialogueData ThirdQuestionDialogue;
    public MoonPuzzleDialogueData FinishTextAdventureDialogue;
}
