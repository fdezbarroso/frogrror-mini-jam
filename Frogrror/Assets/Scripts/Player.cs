using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 5.0f;

    private InputAction _moveAction;

    private void Awake()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
    }

    private void Update()
    {
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
}
