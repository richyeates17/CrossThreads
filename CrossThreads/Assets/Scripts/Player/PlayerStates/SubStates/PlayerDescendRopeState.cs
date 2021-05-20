using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDescendRopeState : PlayerTouchingRopeState
{

    public PlayerDescendRopeState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        DistanceJoint2D ropeDistance = theRopeObject.transform.parent.GetComponent<DistanceJoint2D>();
       
        ropeDistance.distance += Time.deltaTime * playerData.ropeClimbSpeed;

        if (yInput == 0)
        {
            stateMachine.ChangeState(player.RopeGrabState);
        }
        else if (yInput > 0)
        {
            stateMachine.ChangeState(player.RopeClimbState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
