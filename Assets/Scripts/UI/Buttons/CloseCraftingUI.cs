using UnityEngine;

public class CloseCraftingUI : MonoBehaviour
{
    [SerializeField]
    private GameObject craftingUI;

    public void CloseUIClick()
    {
        craftingUI.SetActive(false);
        EventBus.Instance.Publish(new FreezePlayer(false));
    }
}
