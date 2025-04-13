using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource ambienceAudioSource;
    [SerializeField] private AudioSource soundEffectAudioSource;
    [SerializeField] private AudioSource soundtrackAudioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void StopAmbience()
    {
        ambienceAudioSource.Stop();
    }

    public void PlayAmbience()
    {
        ambienceAudioSource.Play();
    }

    public void StopSoundtrack()
    {
        soundtrackAudioSource.Stop();
    }

    public void PlaySoundtrack()
    {
        soundtrackAudioSource.Play();
    }

    public void PlaySoundEffect(AudioClip clip, float volume = 1.0f, float pitch = 1.0f)
    {
        soundEffectAudioSource.pitch = pitch;
        soundEffectAudioSource.PlayOneShot(clip, volume);
    }
}