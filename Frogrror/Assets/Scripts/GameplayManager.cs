using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }
    
    [SerializeField] private Player _player;
    
    [Header("UI")]
    [SerializeField] private DialogueUI _dialogueUI;
    [SerializeField] private GameOverScreen _gameOverScreen;
    [SerializeField] private SceneChanger _sceneChanger;
    
    public Player Player => _player;
    public DialogueUI DialogueUI => _dialogueUI;
    public SceneChanger SceneChanger => _sceneChanger;
    
    public IEnemyTarget CurrentEnemyTarget { get; private set; }

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
        
        _sceneChanger = FindAnyObjectByType<SceneChanger>();

        CurrentEnemyTarget = null;
    }

    public void GameOver()
    {
        if (_gameOverScreen == null)
        {
            return;
        }
        
        _gameOverScreen.Show();
    }


    public void SetEnemyTarget(IEnemyTarget enemyTarget)
    {
        CurrentEnemyTarget = enemyTarget;
    }
}
