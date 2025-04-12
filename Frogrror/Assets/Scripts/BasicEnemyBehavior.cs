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

    public float attackRange = 1.0f;
    public float detectionRange = 3.0f;
    public float idleDirectionChangeTime = 3.0f;
    public float patrolPointWaitTime = 2.0f;

    public PatrolPoint[] patrolPoints;

    [SerializeField] private EnemyState state = EnemyState.Idle;
    [SerializeField] private float patrolPointArrivalThreshold = 0.1f;

    private BasicEnemy _enemyData;
    private Player _player;

    private float _distanceToPlayer = 0.0f;
    private float _idleDirectionTimer = 0.0f;
    private float _patrolPointWaitTimer = 0.0f;

    private int _lastPatrolPointIndex = 0;

    private bool _isWaitingAtPatrolPoint = false;

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
                else if (patrolPoints.Length > 0)
                {
                    ChangeState(EnemyState.Patrol);
                }

                break;

            case EnemyState.Patrol:
                if (IsPlayerVisible())
                {
                    ChangeState(EnemyState.Chase);
                }
                else if (patrolPoints.Length == 0)
                {
                    ChangeState(EnemyState.Idle);
                }

                break;

            case EnemyState.Chase:
                if (_distanceToPlayer <= attackRange)
                {
                    ChangeState(EnemyState.Attack);
                }
                else if (!IsPlayerVisible())
                {
                    ChangeState(patrolPoints.Length > 0 ? EnemyState.Patrol : EnemyState.Idle);
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

            case EnemyState.Patrol:
                Debug.Log("State: Patrol");
                _isWaitingAtPatrolPoint = false;
                _patrolPointWaitTimer = 0f;
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

            case EnemyState.Patrol:
                PatrolBehavior();
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

    private void PatrolBehavior()
    {
        if (patrolPoints.Length == 0)
        {
            return;
        }

        Vector2 targetPosition = patrolPoints[(_lastPatrolPointIndex + 1) % patrolPoints.Length].transform.position;
        float distanceToTarget = Vector2.Distance(transform.position, targetPosition);

        if (_isWaitingAtPatrolPoint)
        {
            _patrolPointWaitTimer -= Time.deltaTime;

            if (_patrolPointWaitTimer <= 0)
            {
                _isWaitingAtPatrolPoint = false;
                _patrolPointWaitTimer = patrolPointWaitTime;
            }
        }
        else if (distanceToTarget <= patrolPointArrivalThreshold)
        {
            _lastPatrolPointIndex = (_lastPatrolPointIndex + 1) % patrolPoints.Length;
            _isWaitingAtPatrolPoint = true;
            _patrolPointWaitTimer = patrolPointWaitTime;
        }
        else
        {
            MoveTo(targetPosition, _enemyData.walkMoveSpeed);
        }
    }

    private void ChaseBehavior()
    {
        MoveTo(_player.transform.position, _enemyData.chaseMoveSpeed);
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

    private void MoveTo(Vector2 target, float speed)
    {
        Vector2 moveVector = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

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
}