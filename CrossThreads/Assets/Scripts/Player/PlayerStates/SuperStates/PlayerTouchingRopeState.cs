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
    protected GameObject theCollidedRopeLink;
    protected bool jumpingTimedown;
    protected Vector2 playerToPivotDirection;
    protected Rope theRope;

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
        player.SetVelocityX(0f);
        player.SetVelocityY(0f);
 
        theCollidedRopeLink = player.collidedRopeNode;
        theCollidedRopeLink.tag = "MoveToLink";
        player.transform.parent = theCollidedRopeLink.transform;
        playerToPivotDirection = (theCollidedRopeLink.transform.parent.transform.position - player.transform.position).normalized;

        theRope = player.collidedRopeNode.transform.parent.GetComponent<Rope>();

    }

    public override void Exit()
    {
        base.Exit();
        player.SetVelocityX(0f);
        player.SetVelocityY(0f);
        player.transform.parent = null;
    }

    public override void LogicUpdate()
    {

        base.LogicUpdate();

        player.transform.position = theCollidedRopeLink.transform.position;
        
        playerToPivotDirection = (theCollidedRopeLink.transform.parent.transform.position - player.transform.position).normalized;

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
