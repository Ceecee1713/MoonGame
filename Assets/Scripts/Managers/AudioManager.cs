using UnityEngine;
using UnityEngine.SceneManagement; 

public class AudioManager : Singleton<AudioManager>
{
    //[SerializeField]
    //private AllSFXs allSoundEffects;

    [SerializeField]
    private AudioSource backgroundNoiseSource;
    [SerializeField]
    private AudioSource environmentNoiseSource;
    [SerializeField]
    private AudioSource soundEffectSource;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void StopSoundEffect(AudioClip audioClip)
    {
        if(audioClip == soundEffectSource.clip)
            soundEffectSource.Stop();
    }

    public void PlaySoundEffect(AudioClip audioClip)
    {
        //if (!allSoundEffects.IsValidClip(audioClip)) 
            //return;

        soundEffectSource.clip = audioClip; 
        soundEffectSource.Play();
    }

    public void PlayEnvironmentNoise(AudioClip audioClip)
    {
        //if (!allSoundEffects.IsValidClip(audioClip)) 
            //return;

        environmentNoiseSource.clip = audioClip; 
        environmentNoiseSource.Play();
    }

    public void StopEnvironmentNoise(AudioClip audioClip)
    {
        if(audioClip == environmentNoiseSource.clip)
            environmentNoiseSource.Stop();
    }

    /*
    public void FadeEnvironmentNoise(AudioClip audioClip)
    {
        environmentNoiseSource.clip = audioClip; 
        environmentNoiseSource.Play();
    }
    */

    public void PlayBackgroundNoise(AudioClip audioClip)
    {
        backgroundNoiseSource.clip = audioClip; 
        backgroundNoiseSource.Play();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        backgroundNoiseSource.Stop();
        soundEffectSource.Stop();
        environmentNoiseSource.Stop();
    }
}
