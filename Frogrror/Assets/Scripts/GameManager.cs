using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private SceneChanger _sceneChanger;

    public AudioManager AudioManager => _audioManager;
    public SceneChanger SceneChanger => _sceneChanger;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
