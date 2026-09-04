using UnityEngine;
using DG.Tweening;

/// <summary>
/// Allows the player game object to tilt along its "X" rotation
/// </summary>
/// 
/// <remarks>
/// This script is meant to be attached to the parent game object containing the player game objects' models, 
/// NOT the ROOT PARENT in the hiearchy as it'll cause conflict with the PlayerStateMachine script when it changes state
/// </remarks>

public class PlayerTilt : MonoBehaviour
{
    [SerializeField]
    private GameObject animatingPlayerObject; //Empty parent game object to tilt the player's body

    [SerializeField]
    private float degreeOfTlitingPlayerBody; //Forward tilting value ("X" rotation)

    [SerializeField]
    private float timeDurationToTiltPlayer;

    private Vector3 newPlayerRotation;
    private Vector3 defaultPlayerRotation;

    private Tween tween;

    void Start()
    {
        newPlayerRotation = new Vector3(degreeOfTlitingPlayerBody,transform.localEulerAngles.y, transform.localEulerAngles.z);
        defaultPlayerRotation = new Vector3(0.0f, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }

    public void BeginTiltingBody()
    {
        tween?.Kill();
        tween = this.transform.DOLocalRotate(newPlayerRotation, timeDurationToTiltPlayer, RotateMode.Fast);
    }

    public void ReturnToDefaultRotation()
    {
        tween?.Kill();
        tween = this.transform.DOLocalRotate(defaultPlayerRotation, timeDurationToTiltPlayer, RotateMode.Fast);
    }
}
