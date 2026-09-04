using UnityEngine;

/// <summary>
/// Manages the losing menu UI AND win game UI functionality - only its display
/// </summary>
/// 
/// <remarks>
/// This script is to be attached to the losing game UI AND win game UI
/// 
/// This script works together with "PlayerStateMachine" , "PlayerHealth" and "ExplorationTimer" scripts
/// See <see cref="PlayerStateMachine"/> - Publishing "FreezePlayer" to freeze player
/// See <see cref="PlayerHealth"/> - Publishing "MaintainPlayerHealth" to maintain player's current health
/// See <see cref="ExplorationTimer"/> - Publishing "PauseExplorationTimer" to pause exploration timer countdown
/// See <see cref="CameraManager"/> - Publishing "FreezeCameraWithActiveUI" to temporarily freeze the camera movement
/// See <see cref="PlayerInputController"/> - Publishing "StopAllPlayerInputs" to stop all player input
/// </remarks>

public class EndScreenUI : MonoBehaviour
{
    void OnEnable()
    {
        EventBus.Instance.Publish(new FreezePlayer(true)); //Publish to "PlayerStateMachine"
        EventBus.Instance.Publish(new MaintainPlayerHealth(true)); //Publish to "PlayerHealth"
        EventBus.Instance.Publish(new PauseExplorationTimer(true)); // Publish to "ExplorationTimer'
        EventBus.Instance.Publish(new FreezeCameraWithActiveUI(this.gameObject, true)); //Publish to 'CameraManager'

        EventBus.Instance.Publish(new StopAllPlayerInputs()); //Publish to 'PlayerInputController'
    }
}
