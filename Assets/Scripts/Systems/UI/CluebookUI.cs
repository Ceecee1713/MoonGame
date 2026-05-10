using UnityEngine;
using UnityEngine.UI;

public class CluebookUI : MonoBehaviour
{
    [Header ("UI Information")]
    [SerializeField]
    private GameObject textAdventureUI;
    [SerializeField]
    private GameObject warningMoonPopUpUI;

    private bool _allowPlayerInputs = false;

    void OnEnable()
    {
        //Prevent Player Inputs
        _allowPlayerInputs = false;
        EventBus.Instance.Publish(new ActivatePlayerInputs(_allowPlayerInputs));

        EventBus.Instance.Publish(new FreezePlayer(true));
        EventBus.Instance.Publish(new MaintainPlayerHealth(true));
        EventBus.Instance.Publish(new PauseExplorationTimer(true));
    }

    void OnDisable()
    {
        if(warningMoonPopUpUI.activeSelf == true)
            return;

        if(textAdventureUI.activeSelf == false)
        {
            EventBus.Instance.Publish(new FreezePlayer(false));
            EventBus.Instance.Publish(new MaintainPlayerHealth(false));
            EventBus.Instance.Publish(new PauseExplorationTimer(false));

            //Allow Player Inputs
            _allowPlayerInputs = true;
            EventBus.Instance.Publish(new ActivatePlayerInputs(_allowPlayerInputs));
        }
    }
}
