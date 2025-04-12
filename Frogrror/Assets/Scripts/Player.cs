using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour, IEnemyTarget
{
    private const float SpriteAlignmentCompensation = 0.565f;

    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private int _hiddenOrderInLayer = -1;

    [SerializeField] private List<AudioClip> _footStepSounds = new List<AudioClip>();
    [SerializeField] private float _footStepDelay = 0.2f;

    [SerializeField] private AudioClip _deathSound;

    [SerializeField] private GameObject _lanternLight;
    [SerializeField] private Transform _normalLightContainer;
    [SerializeField] private Transform _flippedLightContainer;

    private InputAction _moveAction;
    private InputAction _interactAction;
    private InputAction _activateLampAction;

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private Vector3 _originalSpritePosition;

    private int _originalOrderInLayer;

    private InteractableHandler _interactableHandler;

    private readonly List<Item> _items = new List<Item>();

    public event Action<Item> OnItemAdded;

    public bool IsHiding { get; private set; }

    public GameObject GameObject => gameObject;
    public Transform Transform => transform;

    public bool IsDead { get; private set; }

    private float _footStepTimer;

    private bool _hasLamp;
    private bool _lampActive;

    private Transform _lanternContainer;

    private void Awake()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _interactAction = InputSystem.actions.FindAction("Jump");
        _activateLampAction = InputSystem.actions.FindAction("Crouch");

        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _originalOrderInLayer = _spriteRenderer.sortingOrder;
        _originalSpritePosition = _spriteRenderer.transform.localPosition;
        _animator = GetComponentInChildren<Animator>();

        _lanternContainer = _lanternLight.transform.parent;
    }

    private void Update()
    {
        if (_interactAction.WasPerformedThisFrame() && _interactableHandler != null)
        {
            _interactableHandler.Interact();
            return;
        }

        if (_activateLampAction.WasPerformedThisFrame())
        {
            ToggleLamp();
            return;
        }

        if (CanMove())
        {
            Move();
        }
    }

    private void ToggleLamp()
    {
        if (!_hasLamp)
        {
            return;
        }

        _lampActive = !_lampActive;

        _animator.SetBool("LampActive", _lampActive);
        _lanternLight.SetActive(_lampActive);
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

        if (moveValue.x > 0f && _spriteRenderer.flipX)
        {
            _spriteRenderer.flipX = false;
            _spriteRenderer.transform.localPosition = _originalSpritePosition;
            _lanternContainer = _normalLightContainer;
        }
        else if (moveValue.x < 0f && !_spriteRenderer.flipX)
        {
            _spriteRenderer.flipX = true;
            _spriteRenderer.transform.localPosition =
                _originalSpritePosition + Vector3.right * -SpriteAlignmentCompensation;
            _lanternContainer = _flippedLightContainer;
        }

        _lanternLight.transform.SetParent(_lanternContainer);
        _lanternLight.transform.localPosition = Vector3.zero;

        var movement = (Vector3)moveValue * (_speed * Time.deltaTime);

        if (Physics2D.Raycast(transform.position, movement.normalized, 0.5f,
                1 << LayerMask.NameToLayer("NonWalkable")))
        {
            movement = Vector3.zero;
        }

        transform.position += movement;

        if (movement.magnitude > 0f)
        {
            _animator.SetBool("Move", true);

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
            _animator.SetBool("Move", false);

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
        if (other.TryGetComponent(out InteractableHandler interactableHandler) &&
            _interactableHandler == interactableHandler)
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

        if (!_hasLamp)
        {
            _hasLamp = item.ID == "Lamp";
        }

        _items.Add(item);
        OnItemAdded?.Invoke(item);
    }

    public void Kill()
    {
        IsDead = true;

        _animator.SetTrigger("Dead");

        AudioManager.Instance.PlaySoundEffect(_deathSound);

        GameplayManager.Instance.GameOver();

        enabled = false;
    }

    public void Teleport(Vector3 position)
    {
        transform.position = position;
    }

    public bool HasScissors()
    {
        return _items.Any(i => i.ID == "Scissors");
    }
}