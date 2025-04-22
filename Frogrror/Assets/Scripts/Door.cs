using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Header("Lock")]
    [SerializeField] private string requiredItemId;
    [SerializeField] private string interactMessageClear;
    [SerializeField] private string interactMessageNoRequiredItem;
    [SerializeField] private Sprite doorClearedSprite;
    [SerializeField] private AudioClip clearDoorSound;

    [SerializeField] private string _nextSceneName;
    
    [SerializeField] private AudioClip openDoorSound;

    private bool _isCleared;

    private Player _player;
    private SceneChanger _sceneChanger;
    private AudioManager _audioManager;
    private SpriteRenderer _spriteRenderer;
    private InteractableHandler _interactableHandler;

    private void Start()
    {
        _player = GameplayManager.Instance.Player;
        _sceneChanger = GameManager.Instance.SceneChanger;
        _audioManager = GameManager.Instance.AudioManager;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _interactableHandler = GetComponent<InteractableHandler>();

        _isCleared = string.IsNullOrEmpty(requiredItemId);
    }

    public string GetName()
    {
        return gameObject.name;
    }

    public void Interact()
    {
        if (_isCleared)
        {
            _player.enabled = false;
            _audioManager.PlaySoundEffect(openDoorSound);
            _sceneChanger.ChangeScene(_nextSceneName);
        }
        else if (_player.GetItemById(requiredItemId) != null)
        {
            GameplayManager.Instance.DialogueUI.ShowMessage(interactMessageClear);

            _audioManager.PlaySoundEffect(clearDoorSound);

            _spriteRenderer.sprite = doorClearedSprite;
            _interactableHandler.ShowInstructions();
            _isCleared = true;
            
            _player.RemoveItemById(requiredItemId);
        }
        else
        {
            GameplayManager.Instance.DialogueUI.ShowMessage(interactMessageNoRequiredItem);
        }
    }
}