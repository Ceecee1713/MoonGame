using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class SceneButton : MonoBehaviour
{
    [SerializeField]
    private AudioClip buttonClickSFX;

    [SerializeField]
    private CanvasGroup currentCanvasGroup;

    [SerializeField]
    private QuitButton quitButton;
    
    [SerializeField]
    private string mainMenuSceneName;

    private Scene _currentScene;

    private string _currentSceneName;

    private bool _hasBeenClicked = false;
    private bool _calledCoroutine = false;
    private bool _allowClicking = false;
    private bool _preventInput = false;

    private const float DELAY = 0.2f;
    private const float CHANGE_SCENE_DELAY = 1.5f;

    void Start()
    {
        _currentScene = SceneManager.GetActiveScene();
        _currentSceneName = _currentScene.name;
    }

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
            AudioManager.Instance.PlaySoundEffect(buttonClickSFX);
            
            _hasBeenClicked = true;
            _preventInput = true;
            quitButton.PreventInput();
            Invoke("ChangeScene", CHANGE_SCENE_DELAY);
        }
    }

    public void OnPlayAgainClick()
    {
        if(_hasBeenClicked || _preventInput == true)
            return;

        if(_allowClicking == true)
        {
            _hasBeenClicked = true;
            _preventInput = true;
            quitButton.PreventInput();
            Invoke("LoadCurrentScene", CHANGE_SCENE_DELAY);
        }
    }

    private void LoadCurrentScene()
    {
        SceneManager.LoadSceneAsync(_currentSceneName);
    }

    private void ChangeScene()
    {
        SceneManager.LoadSceneAsync(mainMenuSceneName);
    }

    IEnumerator AllowClicking()
    {
        yield return new WaitForSeconds(DELAY);
        _allowClicking = true;
        yield return null;
    }
}
