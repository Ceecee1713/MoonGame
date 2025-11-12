using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue Scriptable Objects/Create A Single String Message")]
public class Dialogue : ScriptableObject
{
    [TextArea(2,5)] public string Message;
}
