using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LanternLight : MonoBehaviour
{
    [SerializeField] private Light2D _light2D;
    [SerializeField] private float _minIntensity;
    [SerializeField] private float _duration;

    private float _maxIntensity;
    
    private void Awake()
    {
        _maxIntensity = _light2D.intensity;
    }

    private void Update()
    {
        var intensity = Mathf.Lerp(_minIntensity, _maxIntensity, Mathf.PingPong(Time.time / _duration, 1f));
        _light2D.intensity = intensity;
    }
}
