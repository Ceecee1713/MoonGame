using System.Collections;
using UnityEngine;

public class CraftingUI : MonoBehaviour
{
    [SerializeField]
    private GameObject warningMessage;

    [SerializeField]
    private float _timeDurationToShowWarningMessage = 4.0f;

    private bool _allowPlayerInputs = false;

    void OnEnable()
    {
        //Prevent Player Inputs
        _allowPlayerInputs = false;
        EventBus.Instance.Publish(new ActivatePlayerInputs(_allowPlayerInputs));
    }

    void OnDisable()
    {
        //Allow Player Inputs
        _allowPlayerInputs = true;
        EventBus.Instance.Publish(new ActivatePlayerInputs(_allowPlayerInputs));
    }

    public void DisplayWarningMessage()
    {
        StopAllCoroutines();
        StartCoroutine(ShowWarningMessage());
    }

    IEnumerator ShowWarningMessage()
    {
        warningMessage.SetActive(true);
        EventBus.Instance.Publish(new StopCraftingTemporarily(true));

        yield return new WaitForSeconds(_timeDurationToShowWarningMessage);

        warningMessage.SetActive(false);
        EventBus.Instance.Publish(new StopCraftingTemporarily(false));
    }
}
