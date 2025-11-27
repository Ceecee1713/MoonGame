using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; 
using DG.Tweening;

public class SwitchSceneButton : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup currentCanvasGroup;

    [SerializeField]
    private QuitButton quitButton;
    
    [SerializeField]
    private string sceneNameToLoadOnClick;

    private bool _hasBeenClicked = false;
    private bool _calledCoroutine = false;
    private bool _allowClicking = false;
    private bool _preventInput = false;

    private const float DELAY = 0.5f;
    private const float CHANGE_SCENE_DELAY = 1.5f;

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
    
    public void OnSwitchSceneClick()
    {
        if(_hasBeenClicked == true || _preventInput == true)
            return;

        if(_allowClicking == true)
        {
            _hasBeenClicked = true;
            _preventInput = true;
            quitButton.PreventInput();
            //Play sound here
            Invoke("ChangeScene", CHANGE_SCENE_DELAY);
        }
    }

    private void ChangeScene()
    {
        SceneManager.LoadSceneAsync(sceneNameToLoadOnClick);
    }

    IEnumerator AllowClicking()
    {
        yield return new WaitForSeconds(DELAY);
        _allowClicking = true;
        yield return null;
    }
}
