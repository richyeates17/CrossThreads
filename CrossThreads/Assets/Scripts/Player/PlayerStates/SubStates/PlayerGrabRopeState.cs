using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrabRopeState : PlayerTouchingRopeState
{
    private Vector2 holdPosition;
    private Vector2 desiredPosition;
    private Rigidbody2D theRopeNode;

    private bool areWeThereYet;
    private float swingForce;

    public PlayerGrabRopeState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        swingForce = playerData.swingForce;
        theRopeNode = theRopeObject.GetComponent<Rigidbody2D>();
        HoldPosition();

    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!isExitingState)
        {
            HoldPosition();

            if (xInput < 0)
            {
                player.CheckIfShouldFlip(xInput);
                Vector2 perpendicularDirection = new Vector2(-playerToPivotDirection.y, playerToPivotDirection.x);
                var leftPerpPos = (Vector2)player.transform.position - perpendicularDirection * -2f;
                Debug.DrawLine(player.transform.position, leftPerpPos, Color.green, 0f);
                var force = perpendicularDirection * swingForce;
                theRopeNode.AddForce(force, ForceMode2D.Force);
            }
            else if (xInput > 0)
            {
                player.CheckIfShouldFlip(xInput);
                Vector2 perpendicularDirection = new Vector2(playerToPivotDirection.y, -playerToPivotDirection.x);

                var rightPerpPos = (Vector2)player.transform.position + perpendicularDirection * 2f;
                Debug.DrawLine(player.transform.position, rightPerpPos, Color.red, 0f);
                var force = perpendicularDirection * swingForce;
                theRopeNode.AddForce(force, ForceMode2D.Force);

            }

            if (yInput >= 1)
            {
                stateMachine.ChangeState(player.RopeClimbState);
            }
            else if (yInput <= -1)
            {
                stateMachine.ChangeState(player.RopeDescendState);
            }
        }

    }

    private void HoldPosition()
    {

        player.transform.position = player.collidedRope.transform.position;

        player.SetVelocityX(0f);
        player.SetVelocityY(0f);

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
