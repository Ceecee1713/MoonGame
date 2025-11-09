using UnityEngine;

public class ReturnTextAdventureButton : MonoBehaviour
{
    public void OnRestartTextAdventureClick()
    {
        EventBus.Instance.Publish(new RestartTextAdventure());
    }
}
