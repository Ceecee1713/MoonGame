using UnityEngine;

[CreateAssetMenu(fileName = "CorriosonValues", menuName = "Environment Scriptable Objects/Create Values for the Corrioson Areas")]
public class CorriosonValues : ScriptableObject
{
    public float DefaultStartingSpeed; 
    public float SpeedToLowerHealthWhenTimerIsUp; 
    public float SpeedToLowerHealthForBadPrayerEffect; 
    public float SpeedToLowerHealthForGoodPrayerEffect; 
}
