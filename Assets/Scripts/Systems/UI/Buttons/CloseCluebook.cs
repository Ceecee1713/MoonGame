using UnityEngine;

public class CloseCluebook : MonoBehaviour
{
    [SerializeField]
    private GameObject cluebookUI;

    [SerializeField]
    private GameObject textAdventureUI;

    public void CloseClubookClick()
    {
        if(textAdventureUI.activeSelf == false)
        {
            EventBus.Instance.Publish(new FreezePlayer(false));
            EventBus.Instance.Publish(new MaintainPlayerHealth(false));
            EventBus.Instance.Publish(new PauseExplorationTimer(false));
        }
        
        cluebookUI.SetActive(false);
    }
}
