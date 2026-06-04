using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Dialogue for storytelling, NPCs and structuring the cluebook </summary>

[CreateAssetMenu(fileName = "StorytellingDialogueData", menuName = "Dialogue Scriptable Objects/Create a New Dialogue Branch For Storytelling")]
public class StorytellingDialogueData : ScriptableObject
{
    [Serializable]
    public class DialogueLine
    {
        [TextArea(2, 5)] public string message;
        public string[] uniqueWordsToColour;
        public Color wordColour = Color.white;
    }

    public DialogueLine[] Messages;
}
