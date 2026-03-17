using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField]
    private AudioSource backgroundNoiseSource;
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

    public void SetVolumeForBackgroundNoise(float desiredVolume)
    {
        backgroundNoiseSource.volume = desiredVolume;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines();
        backgroundNoiseSource.Stop();
        soundEffectSource.Stop();
    }

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
