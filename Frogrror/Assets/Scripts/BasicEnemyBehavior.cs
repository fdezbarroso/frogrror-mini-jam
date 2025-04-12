using System;
using UnityEngine;

public class BasicEnemyBehavior : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Patrol,
        Suspicious,
        Chase,
        Attack
    }

    public float attackRange;
    public float detectionRange;
    public float idleDirectionChangeTime;

    [SerializeField] private EnemyState state = EnemyState.Idle;

    private BasicEnemy _enemyData;
    private Player _player;
    private float _distanceToPlayer;
    private float _idleDirectionTimer;

    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _enemyData = GetComponent<BasicEnemy>();
        _player = GameplayManager.Instance.Player;
        _spriteRenderer = GetComponent<SpriteRenderer>();

        ChangeState(state);
    }

    private void Update()
    {
        if (!_player)
        {
            return;
        }

        _distanceToPlayer = Vector2.Distance(transform.position, _player.transform.position);

        switch (state)
        {
            case EnemyState.Idle:
                if (IsPlayerVisible())
                {
                    ChangeState(EnemyState.Chase);
                }

                break;

            case EnemyState.Chase:
                if (_distanceToPlayer <= attackRange)
                {
                    ChangeState(EnemyState.Attack);
                }
                else if (!IsPlayerVisible())
                {
                    ChangeState(EnemyState.Idle);
                }

                break;

            case EnemyState.Attack:
                if (_distanceToPlayer > attackRange)
                {
                    ChangeState(EnemyState.Chase);
                }

                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        ExecuteState();
    }

    private void ChangeState(EnemyState newState)
    {
        state = newState;

        switch (state)
        {
            case EnemyState.Idle:
                Debug.Log("State: Idle");
                break;

            case EnemyState.Chase:
                Debug.Log("State: Chase");
                break;

            case EnemyState.Attack:
                Debug.Log("State: Attack");
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ExecuteState()
    {
        switch (state)
        {
            case EnemyState.Idle:
                IdleBehavior();
                break;

            case EnemyState.Chase:
                ChaseBehavior();
                break;

            case EnemyState.Attack:
                AttackBehavior();
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void IdleBehavior()
    {
        _idleDirectionTimer -= Time.deltaTime;

        if (_idleDirectionTimer <= 0)
        {
            if (_enemyData.facingDirection == BasicEnemy.FacingDirection.Left)
            {
                _spriteRenderer.flipX = true;
                _enemyData.facingDirection = BasicEnemy.FacingDirection.Right;
            }
            else if (_enemyData.facingDirection == BasicEnemy.FacingDirection.Right)
            {
                _spriteRenderer.flipX = false;
                _enemyData.facingDirection = BasicEnemy.FacingDirection.Left;
            }

            _idleDirectionTimer = idleDirectionChangeTime;
        }
    }

    private void ChaseBehavior()
    {
        Vector2 moveVector = Vector2.MoveTowards(transform.position, _player.transform.position,
            _enemyData.moveSpeed * Time.deltaTime);

        if (moveVector.x > transform.position.x)
        {
            _spriteRenderer.flipX = true;
            _enemyData.facingDirection = BasicEnemy.FacingDirection.Right;
        }
        else if (moveVector.x < transform.position.x)
        {
            _spriteRenderer.flipX = false;
            _enemyData.facingDirection = BasicEnemy.FacingDirection.Left;
        }

        transform.position = moveVector;
    }

    private void AttackBehavior()
    {
        _player.Kill();
    }

    private bool IsPlayerVisible()
    {
        if (!_player || !_enemyData || _player.IsHiding || _distanceToPlayer > detectionRange)
        {
            return false;
        }

        switch (_enemyData.facingDirection)
        {
            case BasicEnemy.FacingDirection.Left:
                return transform.position.x > _player.transform.position.x;

            case BasicEnemy.FacingDirection.Right:
                return transform.position.x <= _player.transform.position.x;

            case BasicEnemy.FacingDirection.Back:
                return false;

            default:
                return false;
        }
    }
}