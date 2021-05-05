using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Base Data")]
public class PlayerData : ScriptableObject
{
    [Header("Move State")]
    public float movementVelocity = 20f;
    public float slopeCheckDistance = 0.5f;
    public float maxSlopeAngle = 80f;

    [Header("Jump State")]
    public float jumpVelocity = 30f;
    public int amountOfJumps = 1;

    [Header("Wall Jump State")]
    public float wallJumpVelocity = 20f;
    public float wallJumpTime = 0.4f;
    public Vector2 wallJumpAngle = new Vector2(1, 2);

    [Header("In Air State")]
    public float coyoteTime = 0.2f;
    public float variableJumpHeightMulitplier = 0.5f;

    [Header("Ladder State")]
    public LayerMask whatisLadder;

    [Header("Wall Slide State")]
    public float wallSlideVelocity = 10f;

    [Header("Wall Climb State")]
    public float wallClimbVelocity = 10f;

    [Header("Ledge Climb State")]
    public Vector2 startOffset;
    public Vector2 stopOffset;

    [Header("Dash State")]
    public float dashCooldown = 0.5f;
    public float maxHoldTime = 1f;
    public float holdTimeScale = 0.25f;
    public float dashTime = 0.2f;
    public float dashVelocity = 50f;
    public float drag = 10f;
    public float dashEndYMultiplier = 0.2f;
    public float distanceBetweenAfterImages = 0.5f;

    [Header("Crouch States")]
    public float crouchMovementVelocity = 10f;
    public float crouchColliderHeight = 3.1f;
    public float standColliderHeight = 6.2f;

    [Header("Check Variables")]
    public float groundCheckRadius = 0.6f;
    public LayerMask whatIsGround;
    public float wallCheckDistance = 0.5f;
}
