using System;
using UnityEngine;

public class Goat : MonoBehaviour, IInteractable
{
    [SerializeField] private string _petMessage;
    [SerializeField] private InteractableHandler _interactableHandler;
    [SerializeField] private AudioClip _petSound;
    
    private Animator _animator;
    private IEnemyTarget _enemyTarget;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _enemyTarget = GetComponent<IEnemyTarget>();
    }

    public string GetName()
    {
        return gameObject.name;
    }

    private void Update()
    {
        if (_enemyTarget.IsDead)
        {
            Destroy(_interactableHandler);
            enabled = false;
        }
    }

    public void Interact()
    {
        _animator.SetTrigger("Pet");
        
        GameplayManager.Instance.Player.Interact();
        
        AudioManager.Instance.PlaySoundEffect(_petSound);
        
        GameplayManager.Instance.DialogueUI.ShowMessage(_petMessage);
    }
}
