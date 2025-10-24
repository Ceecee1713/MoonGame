using System.Collections;
using UnityEngine;

public class CraftingUI : MonoBehaviour
{
    [SerializeField]
    private GameObject warningMessage;

    [SerializeField]
    private float _timeDurationToShowWarningMessage = 4.0f;

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
