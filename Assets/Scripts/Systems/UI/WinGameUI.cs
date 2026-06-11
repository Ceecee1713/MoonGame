using UnityEngine;

/// <summary>
/// Manages the win menu UI functionality - only its display
/// </summary>
/// 
/// <remarks>
/// This script is to be attached to the win game UI
/// 
/// This script works together with "PlayerStateMachine" , "PlayerHealth" and "ExplorationTimer" scripts
/// See <see cref="PlayerStateMachine"/> - Publishing "FreezePlayer" to freeze player
/// See <see cref="PlayerHealth"/> - Publishing "MaintainPlayerHealth" to maintain player's current health
/// See <see cref="ExplorationTimer"/> - Publishing "PauseExplorationTimer" to pause exploration timer countdown
/// 
/// </remarks>

public class WinGameUI : MonoBehaviour
{
    void OnEnable()
    {
        EventBus.Instance.Publish(new FreezePlayer(true)); //Publish to "PlayerStateMachine"
        EventBus.Instance.Publish(new MaintainPlayerHealth(true)); //Publish to "PlayerHealth"
        EventBus.Instance.Publish(new PauseExplorationTimer(true)); //Publish to "ExplorationTimer'
    }

    void OnDisable()
    {

    }
}
