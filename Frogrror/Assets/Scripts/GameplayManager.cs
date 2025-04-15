using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    [SerializeField] private Player _player;

    [Header("UI")]
    [SerializeField] private DialogueUI _dialogueUI;
    [SerializeField] private InventoryUI _inventoryUI;
    [SerializeField] private GameOverScreen _gameOverScreen;

    public Player Player => _player;
    public DialogueUI DialogueUI => _dialogueUI;
    public InventoryUI InventoryUI => _inventoryUI;

    private bool _isPaused;

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

    public void GameOver(bool killed = true)
    {
        if (_gameOverScreen == null)
        {
            return;
        }

        _gameOverScreen.Show(killed);
    }

    public void TogglePause()
    {
        _isPaused = !_isPaused;
        Time.timeScale = _isPaused ? 0 : 1;
        // TODO: toggle pause menu
    }

    public bool IsGamePaused()
    {
        return _isPaused;
    }
}