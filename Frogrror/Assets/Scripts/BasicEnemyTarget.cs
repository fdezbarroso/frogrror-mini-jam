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
    
    public void Kill()
    {
        IsDead = true;
        
        _animator.SetTrigger("Dead");
        
        GameplayManager.Instance.SetEnemyTarget(GameplayManager.Instance.Player);
    }
}
