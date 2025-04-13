using System;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactMessageScissors;
    [SerializeField] private string interactMessageNoScissors;

    [SerializeField] private Transform targetPosition;

    [SerializeField] private Sprite doorClearedSprite;

    [SerializeField] private AudioClip clearDoorSound;
    [SerializeField] private AudioClip openDoorSound;

    private bool _isCleared = false;

    private Player _player;
    private SceneChanger _sceneChanger;
    private AudioManager _audioManager;
    private SpriteRenderer _spriteRenderer;
    private InteractableHandler _interactableHandler;

    private void Start()
    {
        _player = GameplayManager.Instance.Player;
        _sceneChanger = GameplayManager.Instance.SceneChanger;
        _audioManager = AudioManager.Instance;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _interactableHandler = GetComponent<InteractableHandler>();
    }

    public string GetName()
    {
        return gameObject.name;
    }

    public void Interact()
    {
        if (!_player || !_sceneChanger || !_audioManager || !clearDoorSound || !openDoorSound || !_spriteRenderer ||
            !doorClearedSprite || !_interactableHandler)
        {
            return;
        }

        if (_isCleared)
        {
            _player.enabled = false;
            _audioManager.PlaySoundEffect(openDoorSound);
            _sceneChanger.Show(() =>
            {
                _player.Teleport(targetPosition.position);
                _sceneChanger.Hide(() => { _player.enabled = true; });
            });
        }
        else if (_player.HasScissors())
        {
            GameplayManager.Instance.DialogueUI.ShowMessage(interactMessageScissors);

            _audioManager.PlaySoundEffect(clearDoorSound);

            _spriteRenderer.sprite = doorClearedSprite;
            _interactableHandler.ShowInstructions();
            _isCleared = true;
        }
        else
        {
            GameplayManager.Instance.DialogueUI.ShowMessage(interactMessageNoScissors);
        }
    }
}