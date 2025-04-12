using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _ambienceAudioSource;
    
    public void StopAmbience()
    {
        _ambienceAudioSource.Stop();
    }

    public void PlayAmbience()
    {
        _ambienceAudioSource.Play();
    }
}
