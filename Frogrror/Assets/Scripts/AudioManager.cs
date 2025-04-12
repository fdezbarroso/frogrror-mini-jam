using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [SerializeField] private AudioSource _ambienceAudioSource;
    [SerializeField] private AudioSource _soundEffectAudioSource;

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
        _ambienceAudioSource.Stop();
    }

    public void PlayAmbience()
    {
        _ambienceAudioSource.Play();
    }

    public void PlaySoundEffect(AudioClip clip, float volume = 1.0f, float pitch = 1.0f)
    {
        _soundEffectAudioSource.pitch = pitch;
        _soundEffectAudioSource.PlayOneShot(clip, volume);
    }
}
