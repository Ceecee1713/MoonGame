using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CompleteTextAdventure", menuName = "Dialogue Scriptable Objects/Create a New Text Adventure Sequence")]
public class CompleteTextAdventure : ScriptableObject
{
    public TextBranches [] TextBranches = new TextBranches[4];
}

[Serializable]
public struct TextBranches
{
    public DialogueData FirstQuestionDialogue;
    public DialogueData SecondQuestionDialogue;
    public DialogueData ThirdQuestionDialogue;
    public DialogueData FinishTextAdventureDialogue;
}