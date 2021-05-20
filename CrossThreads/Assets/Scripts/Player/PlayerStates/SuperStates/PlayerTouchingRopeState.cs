using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTouchingRopeState : PlayerState
{
    protected bool isGrounded;
    protected bool isTouchingRope;
    protected bool grabInput;
    protected bool jumpInput;
    protected int xInput;
    protected int yInput;
    protected GameObject theRopeObject;
    protected bool jumpingTimedown;
    protected Vector2 playerToPivotDirection;

    public PlayerTouchingRopeState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = player.CheckIfGrounded();
        isTouchingRope = player.CheckIfTouchingRope();

    }

    public override void Enter()
    {
        base.Enter();
        theRopeObject = player.collidedRope;
        player.transform.parent = theRopeObject.transform;
        playerToPivotDirection = (theRopeObject.transform.parent.transform.position - player.transform.position).normalized;
    }

    public override void Exit()
    {
        base.Exit();
        player.transform.parent = null;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        player.transform.position = theRopeObject.transform.position;
        playerToPivotDirection = (theRopeObject.transform.parent.transform.position - player.transform.position).normalized;

        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;
        grabInput = player.InputHandler.GrabInput;
        jumpInput = player.InputHandler.JumpInput;

        if (jumpInput)
        {
            stateMachine.ChangeState(player.JumpState);
        }
        else if (isGrounded && !grabInput)
        {
            stateMachine.ChangeState(player.IdleState);
        }
        else if (!isTouchingRope || !grabInput )
        {
            stateMachine.ChangeState(player.InAirState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
