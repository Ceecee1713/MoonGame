using UnityEngine;

/// <summary>
/// Attach script to pass an event to temporarily freeze the camera's movements when canvas group alpha is 1f 
/// </summary>
/// 
/// <remarks>
/// This script works together with the "CameraManager" and "InteractableItem" scripts
/// See <see cref="CameraManager"/> - Publishing the "FreezeCameraWithActiveUI" event
/// </remarks>

public class FreezeWithActiveUI : MonoBehaviour
{
    private bool freezeCamera;
    private bool doNotRepeat = false; 

    private float currentAlpha;
    private float previousAlpha;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if(doNotRepeat)
            return;

        currentAlpha = canvasGroup.alpha;

        if(currentAlpha == 1.0f)
        {
            previousAlpha = currentAlpha;
            FreezeCamera();
            doNotRepeat = true;
        }
    }

    private void FreezeCamera()
    {
        freezeCamera = true;
        EventBus.Instance.Publish(new FreezeCameraWithActiveUI(this.gameObject, freezeCamera)); //Publish to 'CameraManager'
    }

    void OnDisable()
    {
        if(previousAlpha == 1.0f && EventBus.Exists)
        {
            doNotRepeat = false;
            freezeCamera = false;
            EventBus.Instance.Publish(new FreezeCameraWithActiveUI(this.gameObject, freezeCamera)); //Publish to 'CameraManager'
        }
    }
}
