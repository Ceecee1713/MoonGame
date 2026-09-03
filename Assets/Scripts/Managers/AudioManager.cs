using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

/// <summary>
/// Manages all audio in the game, including background noise and sound effects
/// </summary>
/// 
/// <remarks>
/// Singleton — access globally via <see cref="AudioManager.Instance"/>.
/// No other script should play audio directly
/// 
/// 
/// NOTE TO SELF: AudioManager.Instance can return null during Application.Quit().
/// This is currently safe because no script calls AudioManager.Instance from OnDestroy() or OnDisable().
/// If you ever add an audio call inside OnDestroy()/OnDisable() or anywhere that runs during Quit,
/// Use: if (AudioManager.Exists) before referencing this script
/// See the same fix already applied to EventBus for reference: if (EventBus.Exists)
/// 
///</remarks>

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField]
    private AudioSource backgroundNoiseSource; //Looping background noise that must be assigned in Inspector
    [SerializeField]
    private AudioSource soundEffectSource; //One-shot sound effects that must be assigned in Inspector

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void FadeVolumeOfAudioSource(AudioSource audioSource, float duration, float targetVolume)
    {
        StopAllCoroutines();
        StartCoroutine(FadeAudioSource(audioSource, duration, targetVolume));
    }

    public void StopSoundEffect(AudioClip audioClip)
    {
        if(audioClip == soundEffectSource.clip)
            soundEffectSource.Stop();
    }

    public void PlaySoundEffect(AudioClip audioClip)
    {
        soundEffectSource.clip = audioClip; 
        soundEffectSource.Play();
    }

    public void PlayBackgroundNoise(AudioClip audioClip)
    {
        backgroundNoiseSource.clip = audioClip; 
        backgroundNoiseSource.Play();
    }

    /// <summary>
    /// Sets the background noise volume with no fading
    /// Use <see cref="FadeVolumeOfAudioSource"/> for a fading transistion.
    /// </summary>
    public void SetVolumeForBackgroundNoise(float desiredVolume)
    {
        backgroundNoiseSource.volume = desiredVolume;
    }

    //Stops all audio and coroutines on scene load 
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines();
        backgroundNoiseSource.Stop();
        soundEffectSource.Stop();
    }

    //Lerps an AudioSource's volume from its current value to targetVolume over duration seconds.
    private IEnumerator FadeAudioSource(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float startingVolume = audioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startingVolume, targetVolume, currentTime / duration);
            yield return null;
        }

        audioSource.volume = targetVolume;

        if (targetVolume <= 0f)
            audioSource.Stop();
    }
}
