using UnityEngine;
using DG.Tweening;

public class HidingPlace : MonoBehaviour, IInteractable
{
    [SerializeField] private float _playerHiddenAlpha = 0.5f;
    [SerializeField] private float _fadeDuration = 0.25f;
    [SerializeField] private Sprite _playerHiddenSprite;
    [SerializeField] private AudioClip _playerHidingSound;
    
    private SpriteRenderer _spriteRenderer;
    private Sprite _originalSprite;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalSprite = _spriteRenderer.sprite;
    }

    public string GetName()
    {
        return gameObject.name;
    }

    public void Interact()
    {
        var player = GameplayManager.Instance.Player;

        if (player.IsHiding)
        {
            player.Show();
            _spriteRenderer.DOFade( 1f, _fadeDuration);
            _spriteRenderer.sprite = _originalSprite;
        }
        else
        {
            player.Hide();
            _spriteRenderer.DOFade( _playerHiddenAlpha, _fadeDuration);
            _spriteRenderer.sprite = _playerHiddenSprite;
        }
        
        AudioManager.Instance.PlaySoundEffect(_playerHidingSound, 1.3f);
    }
}
