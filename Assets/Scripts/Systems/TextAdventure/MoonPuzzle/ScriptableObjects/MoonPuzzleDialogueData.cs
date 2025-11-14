using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoonPuzzleDialogueData", menuName = "Dialogue Scriptable Objects/Create a New Dialogue Branch For A Moon Puzzle")]
public class MoonPuzzleDialogueData : ScriptableObject
{
    [TextArea(2,5)] public string [] Messages;
    public string ButtonOneText, ButtonTwoText, ButtonThreeText;

    [Header("Correct Button Number To Advance Dialogue")]
    [Range(1, 3)]
    public int correctButtonNumber;
}
