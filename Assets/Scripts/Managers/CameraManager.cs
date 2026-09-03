using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private CinemachineInputAxisController cineMachineInputAxisController;

    private GameObject currentUI;
    private GameObject previousUI;

    private bool freezeCamera; 

    void Start()
    {
        EventBus.Instance.Subscribe<FreezeCameraWithActiveUI>(ChangeCanvases);
    }

    private void ChangeCanvases(FreezeCameraWithActiveUI freezeCameraWithActiveUI) //Add comment about publishers
    {
        freezeCamera = freezeCameraWithActiveUI.FreezeCamera;
        currentUI = freezeCameraWithActiveUI.CurrentUI;

        if(previousUI == null)
            previousUI = currentUI;

        if(freezeCamera == true)
            cineMachineInputAxisController.enabled = false; //Prevent Cinemachine camera to move

        else if (freezeCamera == false && currentUI == previousUI)
        {
            cineMachineInputAxisController.enabled = true; //Allow Cinemachine camera to move
            previousUI = null;
        }
            
        //Mouse.current.WarpCursorPosition(new Vector2(Screen.width / 2f, Screen.height / 2f));
    }
}
