using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogue Scriptable Objects/Create a New Dialogue Branch")]
public class DialogueData : ScriptableObject
{
    [TextArea(2,5)] public string [] Messages;
    public string ButtonOneText, ButtonTwoText, ButtonThreeText;
    public string ButtonFourText = "Try Again";
}
