using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private int _hiddenOrderInLayer = -1;

    private InputAction _moveAction;
    private InputAction _interactAction;
    private SpriteRenderer _spriteRenderer;
    
    private int _originalOrderInLayer;
    
    private InteractableHandler _interactableHandler;
    
    private readonly List<Item> _items = new List<Item>();
    
    public event Action<Item> OnItemAdded;
    
    public bool IsHiding { get; private set; }

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
        
        transform.position += (Vector3)moveValue * (_speed * Time.deltaTime);
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
}
