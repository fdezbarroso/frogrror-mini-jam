using System;
using System.Collections.Generic;
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

    public List<PatrolPoint> patrolPoints;

    [SerializeField] private EnemyState state = EnemyState.Idle;

    [SerializeField] private float pointArrivalThreshold = 0.1f;

    [SerializeField] private AudioClip _chaseSound;
    [SerializeField] private AudioClip _attackSound;

    [SerializeField] private float _maxIdleTime = 5f;

    private BasicEnemy _enemyData;
    private IEnemyTarget _target;
    
    private Light2D _visionCone;

    private float _distanceToTarget = 0.0f;
    private float _idleDirectionTimer = 0.0f;
    private float _patrolPointWaitTimer = 0.0f;
    private float _suspiciousLookAroundTimer = 0.0f;
    private float _attackDelayTimer = 0.0f;

    private int _lastPatrolPointIndex = 0;
    private int _suspiciousLookCount = 0;

    private bool _isWaitingAtPatrolPoint = false;

    private Vector2 _lastKnownTargetPosition;
    private Vector2 _startingPosition;

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private Vector3 _originalSpritePosition;

    private bool _chaseUntilKill;
    
    private float _idleTimer = 0.0f;

    private void Awake()
    {
        for (var i = patrolPoints.Count - 1; i >= 0; i--)
        {
            if (patrolPoints[i] == null)
            {
                patrolPoints.RemoveAt(i);
            }
        }
    }

    private void Start()
    {
        _target = GameplayManager.Instance.Player;
        
        _enemyData = GetComponent<BasicEnemy>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _visionCone = GetComponentInChildren<Light2D>();
        
        _originalSpritePosition = _spriteRenderer.transform.localPosition;

        _animator = GetComponentInChildren<Animator>();

        _startingPosition = transform.position;

        _visionCone.pointLightInnerRadius = detectionRange;
        _visionCone.pointLightOuterRadius = suspiciousRange * 1.2f;

        ChangeState(state);
    }

    private void Update()
    {
        if (_target == null || !_spriteRenderer || !_visionCone || !_enemyData || _target.IsDead)
        {
            return;
        }

        _distanceToTarget = Vector2.Distance(transform.position, _target.Transform.position);
        
        var verticalDistance = Mathf.Abs(transform.position.y - _target.Transform.position.y);
        
        _distanceToTarget += verticalDistance * 1.5f;

        switch (state)
        {
            case EnemyState.Idle:
                if (IsPlayerVisible() || (_distanceToTarget < hearingRange && !_target.IsHiding) || _chaseUntilKill)
                {
                    ChangeState(EnemyState.Chase);
                }
                else if (IsPlayerSuspicious())
                {
                    ChangeState(EnemyState.Suspicious);
                }
                else if (patrolPoints.Count > 1 || _idleTimer >= _maxIdleTime)
                {
                    ChangeState(EnemyState.Patrol);
                }
                else if (patrolPoints.Count == 1)
                {
                    float distanceToTarget = Vector2.Distance(transform.position, patrolPoints[0].transform.position);

                    if (distanceToTarget > pointArrivalThreshold)
                    {
                        ChangeState(EnemyState.Patrol);
                    }
                }

                break;

            case EnemyState.Patrol:
                if (IsPlayerVisible() || (_distanceToTarget < hearingRange && !_target.IsHiding) || _chaseUntilKill)
                {
                    ChangeState(EnemyState.Chase);
                }
                else if (IsPlayerSuspicious())
                {
                    ChangeState(EnemyState.Suspicious);
                }
                else if (patrolPoints.Count == 0)
                {
                    if (Vector2.Distance(transform.position, _startingPosition) <= pointArrivalThreshold)
                    {
                        ChangeState(EnemyState.Idle);
                    }
                }
                else if (patrolPoints.Count == 1 && _isWaitingAtPatrolPoint)
                {
                    ChangeState(EnemyState.Idle);
                }

                break;

            case EnemyState.Suspicious:
                if (IsPlayerVisible() || (_distanceToTarget < hearingRange && !_target.IsHiding) || _chaseUntilKill)
                {
                    ChangeState(EnemyState.Chase);
                }
                else if (_suspiciousLookCount >= suspiciousLookAroundCount)
                {
                    ChangeState(EnemyState.Patrol);
                }

                break;

            case EnemyState.Chase:
                if (_distanceToTarget <= attackRange)
                {
                    ChangeState(EnemyState.Attack);
                }
                else if (!_chaseUntilKill && !IsPlayerVisible() && !IsPlayerSuspicious())
                {
                    ChangeState(EnemyState.Suspicious);
                }

                break;

            case EnemyState.Attack:
                if (_attackDelayTimer <= 0f)
                {
                    ChangeState(EnemyState.Idle);
                }

                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        ExecuteState();
    }

    private void ChangeState(EnemyState newState)
    {
        if (state == newState)
        {
            return;
        }
        
        if (state == EnemyState.Chase && newState == EnemyState.Suspicious)
        {
            _lastKnownTargetPosition = _target.Transform.position;
        }

        state = newState;

        switch (state)
        {
            case EnemyState.Idle:
                Debug.Log("State: Idle");

                _idleTimer = 0f;
                _idleDirectionTimer = idleDirectionChangeTime;
                break;

            case EnemyState.Patrol:
                Debug.Log("State: Patrol");

                _isWaitingAtPatrolPoint = false;
                break;

            case EnemyState.Suspicious:
                Debug.Log("State: Suspicious");

                _lastKnownTargetPosition = _target.Transform.position;
                _suspiciousLookAroundTimer = suspiciousLookAroundTime;
                _suspiciousLookCount = 0;
                break;

            case EnemyState.Chase:
                Debug.Log("State: Chase");
                
                AudioManager.Instance.PlaySoundEffect(_chaseSound);
                break;

            case EnemyState.Attack:
                Debug.Log("State: Attack");
                
                _animator.SetTrigger("Attack");
                
                _attackDelayTimer = attackDelay;
                
                AudioManager.Instance.PlaySoundEffect(_attackSound);
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
                _visionCone.color = new Color(0x48 / 255.0f, 0x68 / 255.0f, 0x45 / 255.0f);
                _visionCone.intensity = 30.0f;
                IdleBehavior();
                break;

            case EnemyState.Patrol:
                _visionCone.color = new Color(0x48 / 255.0f, 0x68 / 255.0f, 0x45 / 255.0f);
                _visionCone.intensity = 30.0f;
                PatrolBehavior();
                break;

            case EnemyState.Suspicious:
                _visionCone.color = new Color(0x4c / 255.0f, 0x1e / 255.0f, 0x62 / 255.0f);
                _visionCone.intensity = 30.0f;
                SuspiciousBehavior();
                break;

            case EnemyState.Chase:
                _visionCone.color = new Color(0x4c / 255.0f, 0x1e / 255.0f, 0x62 / 255.0f);
                _visionCone.intensity = 60.0f;
                ChaseBehavior();
                break;

            case EnemyState.Attack:
                _visionCone.color = new Color(0x4c / 255.0f, 0x1e / 255.0f, 0x62 / 255.0f);
                _visionCone.intensity = 60.0f;
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
        
        _idleTimer += Time.deltaTime;
    }

    private void PatrolBehavior()
    {
        if (patrolPoints.Count == 0)
        {
            MoveTo(_startingPosition, _enemyData.walkMoveSpeed, "Move");
            return;
        }

        Vector2 targetPosition = patrolPoints[(_lastPatrolPointIndex + 1) % patrolPoints.Count].transform.position;
        float distanceToTarget = Vector2.Distance(transform.position, targetPosition);

        if (_isWaitingAtPatrolPoint)
        {
            _patrolPointWaitTimer -= Time.deltaTime;

            if (_patrolPointWaitTimer <= 0)
            {
                _lastPatrolPointIndex = (_lastPatrolPointIndex + 1) % patrolPoints.Count;

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

    private void SuspiciousBehavior()
    {
        if (IsPlayerSuspicious())
        {
            _lastKnownTargetPosition = _target.Transform.position;
            _suspiciousLookCount = 0;
            _suspiciousLookAroundTimer = suspiciousLookAroundTime;
        }

        float distanceToLastSeen = Vector2.Distance(transform.position, _lastKnownTargetPosition);

        if (distanceToLastSeen > pointArrivalThreshold)
        {
            MoveTo(_lastKnownTargetPosition, _enemyData.walkMoveSpeed, "Move");
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
        MoveTo(_target.Transform.position, _enemyData.chaseMoveSpeed, "Chase");
    }

    private void AttackBehavior()
    {
        _attackDelayTimer -= Time.deltaTime;

        if (_attackDelayTimer <= 0.0f)
        {
            if (_distanceToTarget <= attackRange)
            {
                _target.Kill(this, true);
            }
        }
    }

    private bool IsPlayerVisible()
    {
        if (_target == null || !_target.GameObject.activeInHierarchy || !_enemyData || _target.IsHiding ||
            _distanceToTarget > detectionRange)
        {
            return false;
        }

        switch (_enemyData.facingDirection)
        {
            case BasicEnemy.FacingDirection.Left:
                return transform.position.x > _target.Transform.position.x;

            case BasicEnemy.FacingDirection.Right:
                return transform.position.x <= _target.Transform.position.x;

            case BasicEnemy.FacingDirection.Back:
                return false;

            default:
                return false;
        }
    }

    private bool IsPlayerSuspicious()
    {
        if (_target == null || !_target.GameObject.activeInHierarchy || !_enemyData || _target.IsHiding ||
            _distanceToTarget <= detectionRange ||
            _distanceToTarget > suspiciousRange)
        {
            return false;
        }

        switch (_enemyData.facingDirection)
        {
            case BasicEnemy.FacingDirection.Left:
                return transform.position.x > _target.Transform.position.x;

            case BasicEnemy.FacingDirection.Right:
                return transform.position.x <= _target.Transform.position.x;

            case BasicEnemy.FacingDirection.Back:
                return false;

            default:
                return false;
        }
    }

    private void MoveTo(Vector2 target, float speed, string animationParam)
    {
        target.y = transform.position.y;
        
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

    private void StopAnimation(string animationParam)
    {
        _animator.SetBool(animationParam, false);
    }

    public void SetTarget(IEnemyTarget target, bool chaseUntilKill)
    {
        _target = target;
        _chaseUntilKill = chaseUntilKill;
    }
}