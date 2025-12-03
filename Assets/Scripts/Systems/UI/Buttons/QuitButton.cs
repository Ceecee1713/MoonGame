using System.Collections;
using UnityEngine;

//This script is to be used on quit buttons for the start menu UI screen
//And the pause menu UI screen

public class QuitButton : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup currentCanvasGroup;

    [SerializeField]
    private SceneButton sceneButton;

    private bool _dontRepeat = false;
    private bool _calledCoroutine = false;
    private bool _allowClicking = false;
    private bool _preventInput = false;

    private const float QUIT_DELAY = 2.0f;
    private const float DELAY = 0.5f;

    void Update()
    {
        if(_calledCoroutine == true)
            return;

        if(currentCanvasGroup.alpha == 1.0f && _calledCoroutine == false)
        {
            StartCoroutine(AllowClicking());
            _calledCoroutine = true;
        }
    }

    public void PreventInput()
    {
        _preventInput = true;
    }

    public void OnQuitClick()
    {
        if(_dontRepeat || _preventInput == true)
            return;

        if(_allowClicking == true)
        {
            _dontRepeat = true;
            _preventInput = true;
            sceneButton.PreventInput();
            //Play sound here
            Invoke("Quit", QUIT_DELAY);
        }
    }

    private void Quit()
    {
        //UnityEditor.EditorApplication.isPlaying = false; 
	    Application.Quit();
    }

    IEnumerator AllowClicking()
    {
        yield return new WaitForSeconds(DELAY);
        _allowClicking = true;
        yield return null;
    }
}
