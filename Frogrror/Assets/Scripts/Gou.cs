using UnityEngine;

public class Gou : MonoBehaviour
{
    [SerializeField] private AudioClip _soundEffect;
    [SerializeField] private Animator _secretAnimator;
    
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = GameplayManager.Instance.Player;
        if (other.gameObject == player.gameObject)
        {
            _animator.SetTrigger("Interact");
            _secretAnimator.SetTrigger("Interact");
            
            AudioManager.Instance.PlaySoundEffect(_soundEffect);
            player.Kill(null, false);
            
            enabled = false;
        }
    }
}
