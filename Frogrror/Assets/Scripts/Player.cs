using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 5.0f;

    private InputAction _moveAction;
    private InputAction _interactAction;
    
    private InteractableHandler _interactableHandler;

    private void Awake()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _interactAction = InputSystem.actions.FindAction("Jump");
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
        
        transform.position += (Vector3)moveValue * (_speed * Time.deltaTime);
    }

    private bool CanMove()
    {
        return true;
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
        if (other.TryGetComponent(out InteractableHandler interactableHandler))
        {
            _interactableHandler = interactableHandler;
            _interactableHandler.HideInstructions();
        }
    }
}
