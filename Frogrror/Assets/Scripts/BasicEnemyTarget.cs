using UnityEngine;

public class BasicEnemyTarget : MonoBehaviour, IEnemyTarget
{
    [SerializeField] private AudioClip _deathSound;
    [SerializeField] private string _killedMessage;

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
            GameManager.Instance.AudioManager.PlaySoundEffect(_deathSound);
        }
        
        GameplayManager.Instance.DialogueUI.ShowMessage(_killedMessage);
        
        enemy.SetTarget(GameplayManager.Instance.Player, false);
    }
}
