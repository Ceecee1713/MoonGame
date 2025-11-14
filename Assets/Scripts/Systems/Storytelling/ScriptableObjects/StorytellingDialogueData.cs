using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StorytellingDialogueData", menuName = "Dialogue Scriptable Objects/Create a New Dialogue Branch For Storytelling")]
public class StorytellingDialogueData : ScriptableObject
{
    [TextArea(2,5)] public string [] Messages;
}
