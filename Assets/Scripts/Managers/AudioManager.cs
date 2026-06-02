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
/// All audio is routed through this manager; no other script should play audio directly
/// AudioSources must be assigned in the Inspector.
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

    /// <summary>
    /// Gradually fades an AudioSource to a target volume over time
    /// Cancels any fading of audio already in progress before starting the new one
    /// </summary>
    /// 
    /// <param name="audioSource">The AudioSource to fade.</param>
    /// <param name="duration">Fade duration in seconds.</param>
    /// <param name="targetVolume">Target volume in the range [0, 1]. Reaching 0 stops the source </param>

    public void FadeVolumeOfAudioSource(AudioSource audioSource, float duration, float targetVolume)
    {
        StopAllCoroutines();
        StartCoroutine(FadeAudioSource(audioSource, duration, targetVolume));
    }

    /// <summary>
    /// Stops the sound effect source if it's currently playing
    /// </summary>
    /// 
    ///<param name="audioClip">The audio clip to stop </param>
    
    public void StopSoundEffect(AudioClip audioClip)
    {
        if(audioClip == soundEffectSource.clip)
            soundEffectSource.Stop();
    }

    /// <summary>
    /// Plays a sound effect, replacing any currently playing sound effect
    /// </summary>
    /// 
    ///<param name="audioClip">The audioclip to play</param>
    
    public void PlaySoundEffect(AudioClip audioClip)
    {
        soundEffectSource.clip = audioClip; 
        soundEffectSource.Play();
    }

    /// <summary>
    /// Plays a background noise clip, replacing any currently playing background noise
    /// </summary>
    /// 
    ///<param name="audioClip">The audioclip to play</param>

    public void PlayBackgroundNoise(AudioClip audioClip)
    {
        backgroundNoiseSource.clip = audioClip; 
        backgroundNoiseSource.Play();
    }

    /// <summary>
    /// Sets the background noise volume with no fading
    /// Use <see cref="FadeVolumeOfAudioSource"/> for a smooth transition.
    /// </summary>
    /// 
    ///<param name="desiredVolume">Target volume in the range [0, 1]. 0 for mute, 1 for maximum volume </param>

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
    //Automatically stops the source if targetVolume reaches 0.
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
