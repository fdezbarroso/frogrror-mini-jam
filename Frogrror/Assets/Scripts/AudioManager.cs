using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource ambienceAudioSource;
    [SerializeField] private AudioSource soundEffectAudioSource;
    [SerializeField] private AudioSource soundtrackAudioSource;
    [SerializeField] private AudioReverbFilter soundEffectReverbFilter;

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

    public void ActivateRevertFilter(AudioReverbPreset reverbPreset)
    {
        soundEffectReverbFilter.reverbPreset = reverbPreset;
        soundEffectReverbFilter.enabled = true;
    }

    public void DeactivateRevertFilter()
    {
        soundEffectReverbFilter.enabled = false;
    }
}