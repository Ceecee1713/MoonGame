using UnityEngine;

public class PlaySound : MonoBehaviour
{
    [SerializeField]
    private AudioClip audioClip;

    public void PlaySoundEffect()
    {
        if(audioClip == null)
        {
            Debug.Log("Audio is null");
            return;
        }

        AudioManager.Instance.PlaySoundEffect(audioClip);
    }
}
