using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }
    
    [SerializeField] private Player _player;
    
    [Header("UI")]
    [SerializeField] private DialogueUI _dialogueUI;
    
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
}
