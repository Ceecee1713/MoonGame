using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogue Scriptable Objects/Create a New Dialogue Branch")]
public class DialogueData : ScriptableObject
{
    [TextArea(2,5)] public string [] Messages;
    public string ButtonOneText, ButtonTwoText, ButtonThreeText;

    [Header("Correct Button Number To Advance Dialogue")]
    [Range(1, 3)]
    public int correctButtonNumber;
}
