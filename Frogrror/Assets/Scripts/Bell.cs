using UnityEngine;

public class Bell : MonoBehaviour
{
    [SerializeField] private AudioClip _bellSound;

    private Animator _animator;
    private IEnemyTarget _enemyTarget;

    [SerializeField] private BasicEnemyBehavior _enemy;
    
    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _enemyTarget = GetComponent<IEnemyTarget>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Item item) && item.ID == "Rock")
        {
            _animator.SetBool("Shake", true);
            
            _enemy.SetTarget(_enemyTarget, true);
            
            AudioManager.Instance.PlaySoundEffect(_bellSound, 0.5f);
        }

        if (other.gameObject == GameplayManager.Instance.Player.gameObject)
        {
            _animator.SetBool("Shake", false);
        }
    }
}
