using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class BasicEnemyBehavior : MonoBehaviour
{
    private const float SpriteAlignmentCompensation = 0.565f;

    public enum EnemyState
    {
        Idle,
        Patrol,
        Suspicious,
        Chase,
        Attack
    }

    public float attackRange = 1.0f;
    public float attackDelay = 1.0f;

    public float detectionRange = 3.0f;
    public float suspiciousRange = 5.0f;
    public float hearingRange = 2.0f;
    public float idleDirectionChangeTime = 3.0f;
    public float patrolPointWaitTime = 2.0f;
    public float suspiciousLookAroundTime = 1.0f;

    public int suspiciousLookAroundCount = 3;

    public PatrolPoint[] patrolPoints;

    [SerializeField] private EnemyState state = EnemyState.Idle;

    [SerializeField] private float pointArrivalThreshold = 0.1f;

    private BasicEnemy _enemyData;
    private Player _player;
    private Light2D _visionCone;

    private float _distanceToPlayer = 0.0f;
    private float _idleDirectionTimer = 0.0f;
    private float _patrolPointWaitTimer = 0.0f;
    private float _suspiciousLookAroundTimer = 0.0f;
    private float _attackDelayTimer = 0.0f;

    private int _lastPatrolPointIndex = 0;
    private int _suspiciousLookCount = 0;

    private bool _isWaitingAtPatrolPoint = false;

    private Vector2 _lastKnownPlayerPosition;
    private Vector2 _startingPosition;

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private Vector3 _originalSpritePosition;

    private void Start()
    {
        _enemyData = GetComponent<BasicEnemy>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _visionCone = GetComponentInChildren<Light2D>();

        _player = GameplayManager.Instance.Player;
        _originalSpritePosition = _spriteRenderer.transform.localPosition;

        _animator = GetComponentInChildren<Animator>();

        _startingPosition = transform.position;

        _visionCone.pointLightInnerRadius = detectionRange;
        _visionCone.pointLightOuterRadius = suspiciousRange;

        ChangeState(state);
    }

    private void Update()
    {
        if (!_player || !_spriteRenderer || !_visionCone || !_enemyData || _player.IsDead)
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
                else if (IsPlayerSuspicious() || _distanceToPlayer < hearingRange)
                {
                    ChangeState(EnemyState.Suspicious);
                }
                else if (patrolPoints.Length > 1)
                {
                    ChangeState(EnemyState.Patrol);
                }
                else if (patrolPoints.Length == 1)
                {
                    float distanceToTarget = Vector2.Distance(transform.position, patrolPoints[0].transform.position);

                    if (distanceToTarget > pointArrivalThreshold)
                    {
                        ChangeState(EnemyState.Patrol);
                    }
                }

                break;

            case EnemyState.Patrol:
                if (IsPlayerVisible())
                {
                    ChangeState(EnemyState.Chase);
                }
                else if (IsPlayerSuspicious() || _distanceToPlayer < hearingRange)
                {
                    ChangeState(EnemyState.Suspicious);
                }
                else if (patrolPoints.Length == 0)
                {
                    if (Vector2.Distance(transform.position, _startingPosition) <= pointArrivalThreshold)
                    {
                        ChangeState(EnemyState.Idle);
                    }
                }
                else if (patrolPoints.Length == 1 && _isWaitingAtPatrolPoint)
                {
                    ChangeState(EnemyState.Idle);
                }

                break;

            case EnemyState.Suspicious:
                if (IsPlayerVisible())
                {
                    ChangeState(EnemyState.Chase);
                }
                else if (_suspiciousLookCount >= suspiciousLookAroundCount)
                {
                    ChangeState(EnemyState.Patrol);
                }

                break;

            case EnemyState.Chase:
                if (_distanceToPlayer <= attackRange)
                {
                    ChangeState(EnemyState.Attack);
                }
                else if (!IsPlayerVisible())
                {
                    ChangeState(EnemyState.Suspicious);
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

                _idleDirectionTimer = idleDirectionChangeTime;
                break;

            case EnemyState.Patrol:
                Debug.Log("State: Patrol");

                _isWaitingAtPatrolPoint = false;
                break;

            case EnemyState.Suspicious:
                Debug.Log("State: Suspicious");

                if (IsPlayerSuspicious())
                {
                    _lastKnownPlayerPosition = _player.transform.position;
                }

                _suspiciousLookAroundTimer = suspiciousLookAroundTime;
                _suspiciousLookCount = 0;
                break;

            case EnemyState.Chase:
                Debug.Log("State: Chase");
                break;

            case EnemyState.Attack:
                Debug.Log("State: Attack");
                _attackDelayTimer = attackDelay;
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ExecuteState()
    {
        StopAnimation("Move");

        if (state != EnemyState.Chase)
        {
            StopAnimation("Chase");
        }

        switch (state)
        {
            case EnemyState.Idle:
                IdleBehavior();
                break;

            case EnemyState.Patrol:
                PatrolBehavior();
                break;

            case EnemyState.Suspicious:
                SuspiciousBehavior();
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
            FlipFacingDirection();

            _idleDirectionTimer = idleDirectionChangeTime;
        }
    }

    private void PatrolBehavior()
    {
        if (patrolPoints.Length == 0)
        {
            MoveTo(_startingPosition, _enemyData.walkMoveSpeed, "Move");
            return;
        }

        Vector2 targetPosition = patrolPoints[(_lastPatrolPointIndex + 1) % patrolPoints.Length].transform.position;
        float distanceToTarget = Vector2.Distance(transform.position, targetPosition);

        if (_isWaitingAtPatrolPoint)
        {
            _patrolPointWaitTimer -= Time.deltaTime;

            if (_patrolPointWaitTimer <= 0)
            {
                _lastPatrolPointIndex = (_lastPatrolPointIndex + 1) % patrolPoints.Length;

                _isWaitingAtPatrolPoint = false;
                _patrolPointWaitTimer = patrolPointWaitTime;
            }
        }
        else if (distanceToTarget <= pointArrivalThreshold)
        {
            _isWaitingAtPatrolPoint = true;
            _patrolPointWaitTimer = patrolPointWaitTime;
        }
        else
        {
            MoveTo(targetPosition, _enemyData.walkMoveSpeed, "Move");
        }
    }

    private void StopAnimation(string animationParam)
    {
        _animator.SetBool(animationParam, false);
    }

    private void SuspiciousBehavior()
    {
        float distanceToLastSeen = Vector2.Distance(transform.position, _lastKnownPlayerPosition);

        if (distanceToLastSeen > pointArrivalThreshold)
        {
            MoveTo(_lastKnownPlayerPosition, _enemyData.walkMoveSpeed, "Move");
        }
        else
        {
            if (_suspiciousLookAroundTimer <= 0.0f)
            {
                FlipFacingDirection();

                _suspiciousLookCount++;
                _suspiciousLookAroundTimer = suspiciousLookAroundTime;
            }

            _suspiciousLookAroundTimer -= Time.deltaTime;
        }
    }

    private void ChaseBehavior()
    {
        MoveTo(_player.transform.position, _enemyData.chaseMoveSpeed, "Chase");
    }

    private void AttackBehavior()
    {
        if (_attackDelayTimer <= 0.0f)
        {
            _animator.SetTrigger("Attack");
            _player.Kill();
            _attackDelayTimer = attackDelay;
        }
        else
        {
            _attackDelayTimer -= Time.deltaTime;
        }
    }

    private bool IsPlayerVisible()
    {
        if (!_player || !_player.gameObject.activeInHierarchy || !_enemyData || _player.IsHiding ||
            _distanceToPlayer > detectionRange)
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

    private bool IsPlayerSuspicious()
    {
        if (!_player || !_player.gameObject.activeInHierarchy || !_enemyData || _player.IsHiding ||
            _distanceToPlayer <= detectionRange ||
            _distanceToPlayer > suspiciousRange)
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

    private void MoveTo(Vector2 target, float speed, string animationParam)
    {
        Vector2 moveVector = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if ((moveVector.x > transform.position.x && _enemyData.facingDirection == BasicEnemy.FacingDirection.Left) ||
            (moveVector.x < transform.position.x && _enemyData.facingDirection == BasicEnemy.FacingDirection.Right))
        {
            FlipFacingDirection();
        }

        transform.position = moveVector;

        _animator.SetBool(animationParam, true);
    }

    private void FlipFacingDirection()
    {
        if (_enemyData.facingDirection == BasicEnemy.FacingDirection.Left)
        {
            _spriteRenderer.flipX = false;
            _spriteRenderer.transform.localPosition = _originalSpritePosition;
            _enemyData.facingDirection = BasicEnemy.FacingDirection.Right;
        }
        else if (_enemyData.facingDirection == BasicEnemy.FacingDirection.Right)
        {
            _spriteRenderer.flipX = true;
            _spriteRenderer.transform.localPosition =
                _originalSpritePosition + Vector3.right * -SpriteAlignmentCompensation;
            _enemyData.facingDirection = BasicEnemy.FacingDirection.Left;
        }
        
        _visionCone.transform.Rotate(0.0f, 0.0f, 180.0f);
    }
}