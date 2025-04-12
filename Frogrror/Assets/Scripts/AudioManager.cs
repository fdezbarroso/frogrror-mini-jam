using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [SerializeField] private AudioSource _ambienceAudioSource;
    
    private Camera _camera;

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
        
        _camera = Camera.main;
    }

    public void StopAmbience()
    {
        _ambienceAudioSource.Stop();
    }

    public void PlayAmbience()
    {
        _ambienceAudioSource.Play();
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        AudioSource.PlayClipAtPoint(clip, _camera.transform.position);
    }
}
