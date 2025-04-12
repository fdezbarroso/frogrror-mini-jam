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
}
