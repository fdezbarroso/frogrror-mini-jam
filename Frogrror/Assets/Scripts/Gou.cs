using UnityEngine;

public class Gou : MonoBehaviour
{
    [SerializeField] private AudioClip _soundEffect;
    
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
            _animator.SetTrigger("Transform");
            
            AudioManager.Instance.PlaySoundEffect(_soundEffect);
            player.Kill(null);
            
            enabled = false;
        }
    }
}
