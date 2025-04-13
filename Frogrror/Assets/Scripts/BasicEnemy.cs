using System;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    public enum FacingDirection
    {
        Left,
        Right,
        Back // To use when the enemy is facing away from the scene
    }

    public FacingDirection facingDirection = FacingDirection.Left;
    public float walkMoveSpeed = 1.5f;
    public float chaseMoveSpeed = 3.0f;

    private BasicEnemyBehavior _enemyBehavior;

    private void Awake()
    {
        _enemyBehavior = GetComponent<BasicEnemyBehavior>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        var player = GameplayManager.Instance.Player;
        if (other.gameObject == player.gameObject)
        {
            if (player.IsDead || player.IsHiding)
            {
                return;
            }
            
            _enemyBehavior.SetTarget(player, false);
        }
    }
}
