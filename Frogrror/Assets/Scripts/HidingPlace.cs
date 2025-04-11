using UnityEngine;
using DG.Tweening;

public class HidingPlace : MonoBehaviour, IInteractable
{
    [SerializeField] private float _playerHiddenAlpha = 0.5f;
    [SerializeField] private float _fadeDuration = 0.25f;
    
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
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
        }
        else
        {
            player.Hide();
            _spriteRenderer.DOFade( _playerHiddenAlpha, _fadeDuration);
        }
    }
}
