using UnityEngine;

[CreateAssetMenu(fileName = "CorriosonValues", menuName = "Environment Scriptable Objects/Create Values for the Corrioson Areas")]
public class CorriosonValues : ScriptableObject
{
    public float SpeedToLowerHealthWhenTimerIsUp; 
    public float SpeedToLowerHealthWhenAreaIsCleared; 
    public float SpeedToLowerHealthForBadPrayerEffect; 
    public float SpeedToLowerHealthForGoodPrayerEffect; 
}
