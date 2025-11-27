using UnityEngine;

public class OpenCluebook : MonoBehaviour
{
    [SerializeField]
    private GameObject cluebookUI;

    public void OpenClubookClick()
    {
        cluebookUI.SetActive(true);

        EventBus.Instance.Publish(new FreezePlayer(true));
        EventBus.Instance.Publish(new MaintainPlayerHealth(true));
        EventBus.Instance.Publish(new PauseExplorationTimer(true));
    }
}
