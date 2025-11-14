using UnityEngine;

public class ChestButton : MonoBehaviour
{
    [SerializeField]
    private GameObject chestUI;

    public void OnCloseChestUIClick()
    {
        EventBus.Instance.Publish(new FreezePlayer(false));
        EventBus.Instance.Publish(new MaintainPlayerHealth(false));
        EventBus.Instance.Publish(new ChestIsOpen(false));
        chestUI.SetActive(false);
    }
}
