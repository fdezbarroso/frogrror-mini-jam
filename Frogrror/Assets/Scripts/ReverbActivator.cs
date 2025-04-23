using UnityEngine;

public class ReverbActivator : MonoBehaviour
{
    [SerializeField] private AudioReverbPreset _reverbPreset = AudioReverbPreset.Hallway;

    private void Start()
    {
        GameManager.Instance.AudioManager.ActivateRevertFilter(_reverbPreset);
    }
}
