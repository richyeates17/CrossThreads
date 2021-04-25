using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Base Data")]
public class PlayerData : ScriptableObject
{
    [Header("Move State")]
    public float movementVelocity = 20f;

    [Header("Jump State")]
    public float jumpVelocity = 30f;
    public int amountOfJumps = 1;

    [Header("In Air State")]
    public float coyoteTime = 0.2f;
    public float variableJumpHeightMulitplier = 0.5f;

    [Header("Check Variables")]
    public float groundCheckRadius = 0.6f;
    public LayerMask whatIsGround;
}
