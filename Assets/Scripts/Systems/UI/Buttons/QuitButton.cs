using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the quit button for the start menu UI and pause menu UI
/// </remarks>

public class QuitButton : MonoBehaviour
{
    [SerializeField]
    private AudioClip buttonClickSFX;

    [SerializeField]
    private CanvasGroup currentCanvasGroup;

    [SerializeField]
    private SceneButton sceneButton;

    private bool _hasBeenClicked = false;
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
        if(_hasBeenClicked == true|| _preventInput == true)
            return;

        if(_allowClicking == true)
        {
            AudioManager.Instance.PlaySoundEffect(buttonClickSFX);
            
            _hasBeenClicked = true;
            _preventInput = true;
            sceneButton.PreventInput();
            Invoke("Quit", QUIT_DELAY);
        }
    }

    private void Quit()
    {
	    Application.Quit();
    }

    private IEnumerator AllowClicking()
    {
        yield return new WaitForSeconds(DELAY);
        _allowClicking = true;
        yield return null;
    }
}
