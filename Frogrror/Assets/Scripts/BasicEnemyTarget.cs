using UnityEngine;

public class BasicEnemyTarget : MonoBehaviour, IEnemyTarget
{
    [SerializeField] private AudioClip _deathSound;

    private Animator _animator;
    
    public GameObject GameObject => gameObject;
    public Transform Transform => transform;
    public bool IsDead { get; private set; }
    public bool IsHiding => false;
    
    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }
    
    public void Kill(BasicEnemyBehavior enemy, bool killed = true)
    {
        IsDead = true;
        
        _animator.SetTrigger("Dead");

        if (_deathSound != null)
        {
            AudioManager.Instance.PlaySoundEffect(_deathSound);
        }
        
        enemy.SetTarget(GameplayManager.Instance.Player, false);
    }
}
