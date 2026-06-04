using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the cluebook UI functionality - only when it's set active and/or inactive (publishing events to other scripts)
/// </summary>
/// 
/// <remarks>
/// 
/// This script works together with the "PlayerHealth", "ExplorationTimer" scripts
/// See <see cref="PlayerHealth"/> - Maintaining / not maintain player health
/// See <see cref="ExplorationTimer"/> - Pause/unpause exploration timer countdown
/// 
/// </remarks>

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
        EventBus.Instance.Publish(new MaintainPlayerHealth(true)); //Publish to "PlayerHealth"
        EventBus.Instance.Publish(new PauseExplorationTimer(true)); //Publish to "ExplorationTimer"
    }

    void OnDisable()
    {
        if(warningMoonPopUpUI.activeSelf == true)
            return;

        if(textAdventureUI.activeSelf == false)
        {
            EventBus.Instance.Publish(new FreezePlayer(false));
            EventBus.Instance.Publish(new MaintainPlayerHealth(false)); //Publish to "PlayerHealth"
            EventBus.Instance.Publish(new PauseExplorationTimer(false)); //Publish to "ExplorationTimer"

            //Allow Player Inputs
            _allowPlayerInputs = true;
            EventBus.Instance.Publish(new ActivatePlayerInputs(_allowPlayerInputs));
        }
    }
}
