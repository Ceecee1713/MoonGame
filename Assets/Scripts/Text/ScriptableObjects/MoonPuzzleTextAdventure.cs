using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MoonPuzzleTextAdventure", menuName = "Dialogue Scriptable Objects/Create a Complete Moon Puzzle Text Adventure Sequence")]
public class MoonPuzzleTextAdventure : ScriptableObject
{
    public TextBranches [] TextBranches = new TextBranches[3];
}

[Serializable]
public struct TextBranches
{
    public DialogueData FirstQuestionDialogue;
    public DialogueData SecondQuestionDialogue;
    public DialogueData ThirdQuestionDialogue;
    public DialogueData FinishTextAdventureDialogue;
}