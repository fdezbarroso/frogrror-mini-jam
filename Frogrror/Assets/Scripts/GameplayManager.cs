using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }
    
    [SerializeField] private Player _player;
    
    [Header("UI")]
    [SerializeField] private DialogueUI _dialogueUI;
    [SerializeField] private GameOverScreen _gameOverScreen;
    
    public Player Player => _player;
    public DialogueUI DialogueUI => _dialogueUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GameOver()
    {
        if (_gameOverScreen == null)
        {
            return;
        }
        
        _gameOverScreen.Show();
    }
}
