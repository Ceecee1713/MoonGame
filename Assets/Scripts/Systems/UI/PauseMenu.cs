using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject [] uisToCheckFor; //storytellingUI, textAdventureUI, cluebookUI, craftingUI, win and lose UIs

    private bool _allowPlayerInputs = false;

    void Awake()
    {
        EventBus.Instance.Subscribe<PauseGame>(DisplayPauseMenu);
    }

    void Start()
    {
        this.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        EventBus.Instance.Publish(new FreezePlayer(true));
        EventBus.Instance.Publish(new MaintainPlayerHealth(true));
        EventBus.Instance.Publish(new PauseExplorationTimer(true));

        //Prevent Player Inputs
        _allowPlayerInputs = false;
        EventBus.Instance.Publish(new ActivatePlayerInputs(_allowPlayerInputs));
    }

    void OnDisable()
    {
        EventBus.Instance.Publish(new FreezePlayer(false));
        EventBus.Instance.Publish(new MaintainPlayerHealth(false));
        EventBus.Instance.Publish(new PauseExplorationTimer(false));

        //Allow Player Inputs
        _allowPlayerInputs = true;
        EventBus.Instance.Publish(new ActivatePlayerInputs(_allowPlayerInputs));
    }

    private void DisplayPauseMenu(PauseGame pauseGame)
    {
        for(int i = 0; i < uisToCheckFor.Length; i++)
        {
            if(uisToCheckFor[i].activeSelf == true)
                return;
        }
            this.gameObject.SetActive(true);
    }
}
