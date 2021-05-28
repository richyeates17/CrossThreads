using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class Player : MonoBehaviour
{
    #region State Variables
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerInAirState InAirState { get; private set; }
    public PlayerLandState LandState { get; private set; }
    public PlayerWallSlideState WallSlideState { get; private set; }
    public PlayerWallGrabState WallGrabState { get; private set; }
    public PlayerWallClimbState WallClimbState { get; private set; }
    public PlayerWallJumpState WallJumpState { get; private set; }
    public PlayerLedgeClimbState LedgeClimbState { get; private set; }
    public PlayerDashState DashState { get; private set; }
    public PlayerCrouchIdleState CrouchIdleState { get; private set; } 
    public PlayerCrouchMoveState CrouchMoveState { get; private set; }
    public PlayerClimbingLadderState LadderClimbState { get; private set; }
    public PlayerGrabLadderState LadderGrabState { get; private set; }
    public PlayerDescendLadderState LadderDescendState { get; private set; }
    public PlayerGrabRopeState RopeGrabState { get; private set; }
    public PlayerClimbRopeState RopeClimbState { get; private set; }
    public PlayerDescendRopeState RopeDescendState { get; private set; }

    [SerializeField]
    private PlayerData playerData;
    #endregion

    #region Check Transforms
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private Transform wallCheck;
    [SerializeField]
    private Transform ledgeCheck;
    [SerializeField]
    private Transform ceilingCheck;
    [SerializeField]
    private Transform slopeCheck;
    #endregion

    #region Components
    public PlayerSpineAnimController Anim { get; private set; }
    public PlayerInputHandler InputHandler { get; private set; }
    public Rigidbody2D RB { get; private set; }
    public Transform DashDirectionIndicator { get; private set; }
    public CapsuleCollider2D MovementCollider { get; private set; }
    #endregion

    #region Other Variables
    public Vector2 CurrentVelocity { get; private set; }
    public int FacingDirection { get; private set; }
    public bool isHanging;
    public float slopeSideAngle;
    public bool isOnSlope;
    public bool canWalkOnSlope;
    public bool isOnGround;

    private Vector2 slopeNormalPerp;
    private float slopeDownAngleOld;
    private float slopeDownAngle;
    private float startTime;
    private bool jumpOffSlope;
    private Vector2 workspace;
    [SerializeField]
    PhysicsMaterial2D playerMat;
    [SerializeField]
    PhysicsMaterial2D fullFriction;

    public GameObject collidedLadder;
    public GameObject collidedRopeNode;
    #endregion

    #region Unity Callback Functions
    private void Awake()
    {
        StateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState(this, StateMachine, playerData, "idle");
        MoveState = new PlayerMoveState(this, StateMachine, playerData, "move");
        JumpState = new PlayerJumpState(this, StateMachine, playerData, "inAir");
        InAirState = new PlayerInAirState(this, StateMachine, playerData, "inAir");
        LandState = new PlayerLandState(this, StateMachine, playerData, "land");
        WallSlideState = new PlayerWallSlideState(this, StateMachine, playerData, "wallSlide");
        WallGrabState = new PlayerWallGrabState(this, StateMachine, playerData, "wallGrab");
        WallClimbState = new PlayerWallClimbState(this, StateMachine, playerData, "wallClimb");
        WallJumpState = new PlayerWallJumpState(this, StateMachine, playerData, "inAir");
        LedgeClimbState = new PlayerLedgeClimbState(this, StateMachine, playerData, "ledgeGrab");
        DashState = new PlayerDashState(this, StateMachine, playerData, "inAir");
        CrouchIdleState = new PlayerCrouchIdleState(this, StateMachine, playerData, "crouchIdle");
        CrouchMoveState = new PlayerCrouchMoveState(this, StateMachine, playerData, "crouchMove");
        LadderClimbState = new PlayerClimbingLadderState(this, StateMachine, playerData, "ladderClimb");
        LadderDescendState = new PlayerDescendLadderState(this, StateMachine, playerData, "ladderDescend");
        LadderGrabState = new PlayerGrabLadderState(this, StateMachine, playerData, "ladderGrab");
        RopeGrabState = new PlayerGrabRopeState(this, StateMachine, playerData, "ropeGrab");
        RopeClimbState = new PlayerClimbRopeState(this, StateMachine, playerData, "ropeClimb");
        RopeDescendState = new PlayerDescendRopeState(this, StateMachine, playerData, "ropeDescend");
    }

    private void Start()
    {

        Anim = GetComponent<PlayerSpineAnimController>();
        InputHandler = GetComponent<PlayerInputHandler>();
        RB = GetComponent<Rigidbody2D>();
        DashDirectionIndicator = transform.Find("DashDirectionIndicator");
        MovementCollider = GetComponent<CapsuleCollider2D>();

        FacingDirection = 1;

        StateMachine.Initialize(IdleState);

        //called to set a listener delegate for animation finished
        Anim.skeletonAnimation.AnimationState.End += delegate
        {
            if (Anim.currentAnimation == "land")
            {
                AnimationFinishTrigger();
            }
            if (Anim.currentAnimation == "ledgeClimb")
            {
                AnimationFinishTrigger();
            }
        };
    }

    private void Update()
    {
        CurrentVelocity = RB.velocity;
        StateMachine.CurrentState.LogicUpdate();

        CheckIfOnSlopeHorizontal();
        CheckIfOnSlopeVertical();
        ResetJumpOffSlopeAfterTime();

    }

    private void FixedUpdate()
    {
        SetPhysicsMaterial();
        StateMachine.CurrentState.PhysicsUpdate();
    }
    #endregion

    #region Set Functions

    public void SetVelocityZero()
    {
        RB.velocity = Vector2.zero;
        CurrentVelocity = Vector2.zero;
    }

    public void SetVelocity(float velocity, Vector2 angle, int direction)
    {
        angle.Normalize();
        workspace.Set(angle.x * velocity * direction, angle.y * velocity);
        RB.velocity = workspace;
        CurrentVelocity = workspace;
    }

    public void SetVelocity(float velocity, Vector2 direction)
    {
        workspace = direction * velocity;
        RB.velocity = workspace;
        CurrentVelocity = workspace;
    }

    public void SetVelocityX(float velocity)
    {
        if (isOnSlope && canWalkOnSlope && CheckIfGrounded() && !jumpOffSlope)
        {
            workspace.Set((-velocity * slopeNormalPerp.x), (slopeNormalPerp.y * -velocity));
            RB.velocity = workspace;
            CurrentVelocity = workspace;
        }
        else if (canWalkOnSlope)
        {
            workspace.Set(velocity, CurrentVelocity.y);
            RB.velocity = workspace;
            CurrentVelocity = workspace;
        }
    }

    public void SetVelocityY(float velocity)
    {
        workspace.Set(CurrentVelocity.x, velocity);
        RB.velocity = workspace;
        CurrentVelocity = workspace;
    }
    #endregion

    #region Check Functions

    public bool CheckForCeiling()
    {
        return Physics2D.OverlapCircle(ceilingCheck.position, playerData.groundCheckRadius, playerData.whatIsGround);
    }

    public bool CheckIfGrounded()
    {
        Collider2D hit1 = Physics2D.OverlapCircle(groundCheck.position, playerData.groundCheckRadius, playerData.whatIsGround);
        Collider2D hit2 = Physics2D.OverlapCircle(groundCheck.position, playerData.groundCheckRadius, playerData.whatIsPlatform);
        if (hit1||hit2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CheckIfTouchingWall()
    {
        return Physics2D.Raycast(wallCheck.position, Vector2.right * FacingDirection, playerData.wallCheckDistance, playerData.whatIsGround);
    }

    public bool CheckIfTouchingLedge()
    {
        return Physics2D.Raycast(ledgeCheck.position, Vector2.right * FacingDirection, playerData.wallCheckDistance, playerData.whatIsGround);
    }

    public bool CheckIfTouchingLadder()
    {
        RaycastHit2D hit1 = Physics2D.Raycast(ledgeCheck.position, Vector2.up, playerData.wallCheckDistance, playerData.whatisLadder);
        Collider2D hit2 = Physics2D.OverlapCircle(groundCheck.position, playerData.groundCheckRadius, playerData.whatisLadder);
        if (hit1)
        {
            collidedLadder = hit1.collider.gameObject;
            return true;
        }
        else if (hit2)
        {
            collidedLadder = hit2.gameObject;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CheckIfTouchingRope()
    {
        Collider2D hit2 = Physics2D.OverlapCircle(this.transform.position, playerData.groundCheckRadius, playerData.whatisRope);

        if (hit2)
        {
            collidedRopeNode = hit2.gameObject;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CheckIfTouchingWallBack()
    {
        return Physics2D.Raycast(wallCheck.position, Vector2.right * -FacingDirection, playerData.wallCheckDistance, playerData.whatIsGround);
    }

    public void CheckIfShouldFlip(int xInput)
    {
        if (xInput != 0 && xInput != FacingDirection)
        {
            Flip();
        }
    }

    //handle slopes

    public void CheckIfOnSlopeHorizontal()
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(slopeCheck.position, Vector2.right * FacingDirection, playerData.slopeCheckDistance, playerData.whatIsGround);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(slopeCheck.position, Vector2.right * -FacingDirection, playerData.slopeCheckDistance, playerData.whatIsGround);

        if (slopeHitFront && !(CheckIfTouchingWall() || CheckIfTouchingWallBack()))
        {
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
            isOnSlope = true;
        }
        else if (slopeHitBack && !(CheckIfTouchingWall() || CheckIfTouchingWallBack()))
        {
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
            isOnSlope = true;
        }
        else
        {
            slopeSideAngle = 0.0f;
            isOnSlope = false;
        }
    }

    public void CheckIfOnSlopeVertical()
    {
        RaycastHit2D hit = Physics2D.Raycast(slopeCheck.position, Vector2.down, playerData.slopeCheckDistance, playerData.whatIsGround);

        if (hit)
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;

            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeDownAngle != slopeDownAngleOld && !(CheckIfTouchingWall() || CheckIfTouchingWallBack()))
            {
                isOnSlope = true;
            }

            slopeDownAngleOld = slopeDownAngle;

            Debug.DrawRay(hit.point, slopeNormalPerp, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.green);

            if (slopeDownAngle > playerData.maxSlopeAngle || slopeSideAngle > playerData.maxSlopeAngle && !(CheckIfTouchingWall() || CheckIfTouchingWallBack()))
            {
                canWalkOnSlope = false;
            }
            else
            {
                canWalkOnSlope = true;
            }
         }
    }
    #endregion

    #region Other Functions

    public void JumpOffSlope ()
    {
        startTime = Time.time;
        isOnSlope = false;
        jumpOffSlope = true;
        canWalkOnSlope = true;
    }

    private void ResetJumpOffSlopeAfterTime()
    {
        if (jumpOffSlope && Time.time > startTime + 0.2f)
        {
            jumpOffSlope = false;
        }
    }

    public void SetPhysicsMaterial()
    {
        if (isOnSlope)
        {
            RB.sharedMaterial = fullFriction;
        }
        else
        {
            RB.sharedMaterial = playerMat;
        }
    }

    public void SetColliderHeight(float height)
    {
        Vector2 center = MovementCollider.offset;
        workspace.Set(MovementCollider.size.x, height);

        center.y += (height - MovementCollider.size.y) / 2;

        MovementCollider.size = workspace;
        MovementCollider.offset = center;
    }

    public Vector2 DetermineCornerPosition()
    {
        RaycastHit2D xHit = Physics2D.Raycast(wallCheck.position, Vector2.right * FacingDirection, playerData.wallCheckDistance, playerData.whatIsGround);
        float xDist = xHit.distance;
        workspace.Set((xDist + 0.015f) * FacingDirection, 0f);
        RaycastHit2D yHit = Physics2D.Raycast(ledgeCheck.position + (Vector3)(workspace), Vector2.down, ledgeCheck.position.y - wallCheck.position.y + 0.015f, playerData.whatIsGround);
        float yDist = yHit.distance;

        workspace.Set(wallCheck.position.x + (xDist * FacingDirection), ledgeCheck.position.y - yDist);
        return workspace;
    }

    private void AnimationTriggerFunction()
    {
        StateMachine.CurrentState.AnimationTrigger();
    }

    public void AnimationFinishTrigger()
    {
        StateMachine.CurrentState.AnimationFinishTrigger();
    }

    private void Flip()
    {
        FacingDirection *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }
    #endregion

}
