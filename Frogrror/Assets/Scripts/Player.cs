using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private int _hiddenOrderInLayer = -1;
    
    [SerializeField] private List<AudioClip> _footStepSounds = new List<AudioClip>();
    [SerializeField] private float _footStepDelay = 0.2f;

    private InputAction _moveAction;
    private InputAction _interactAction;
    private SpriteRenderer _spriteRenderer;
    
    private int _originalOrderInLayer;
    
    private InteractableHandler _interactableHandler;
    
    private readonly List<Item> _items = new List<Item>();
    
    public event Action<Item> OnItemAdded;
    
    public bool IsHiding { get; private set; }

    private float _footStepTimer;

    private void Awake()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _interactAction = InputSystem.actions.FindAction("Jump");
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalOrderInLayer =  _spriteRenderer.sortingOrder;
    }

    private void Update()
    {
        if (_interactAction.WasPerformedThisFrame() && _interactableHandler != null)
        {
            _interactableHandler.Interact();
            return;
        }
        
        if (CanMove())
        {
            Move();
        }
    }

    private void Move()
    {
        var moveValue = _moveAction.ReadValue<Vector2>();
        if (!CanMoveVertically())
        {
            moveValue.y = 0f;
        }
        
        var absX = Mathf.Abs(moveValue.x);
        var absY = Mathf.Abs(moveValue.y);

        if (absX >= absY)
        {
            moveValue.y = 0f;
        }
        else
        {
            moveValue.x = 0f;
        }

        if (moveValue.x > 0f)
        {
            _spriteRenderer.flipX = true;
        }
        else if (moveValue.x < 0f)
        {
            _spriteRenderer.flipX = false;
        }

        var movement = (Vector3)moveValue * (_speed * Time.deltaTime);
        transform.position += movement;

        if (movement.magnitude > 0f)
        {
            if (_footStepTimer > 0f)
            {
                _footStepTimer -= Time.deltaTime;
                return;
            }

            var footStepSound = _footStepSounds[Random.Range(0, _footStepSounds.Count)];
            var pitch = Random.Range(0.8f, 1.2f);
            AudioManager.Instance.PlaySoundEffect(footStepSound, 0.5f, pitch);
            
            _footStepTimer = _footStepDelay;
        }
        else
        {
            _footStepTimer = 0f;
        }
    }

    private bool CanMove()
    {
        return !IsHiding;
    }

    private bool CanMoveVertically()
    {
        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out InteractableHandler interactableHandler))
        {
            _interactableHandler = interactableHandler;
            _interactableHandler.ShowInstructions();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out InteractableHandler interactableHandler) && _interactableHandler == interactableHandler)
        {
            _interactableHandler.HideInstructions();
            _interactableHandler = null;
        }
    }

    public void Hide()
    {
        IsHiding = true;
        _spriteRenderer.sortingOrder = _hiddenOrderInLayer;
    }

    public void Show()
    {
        IsHiding = false;
        _spriteRenderer.sortingOrder = _originalOrderInLayer;
    }
    
    public void TakeItem(Item item)
    {
        item.gameObject.SetActive(false);
        
        _items.Add(item);
        OnItemAdded?.Invoke(item);
    }

    public void Kill()
    {
        GameplayManager.Instance.GameOver();
    }

    public void Teleport(Vector3 position)
    {
        transform.position = position;
    }
}
