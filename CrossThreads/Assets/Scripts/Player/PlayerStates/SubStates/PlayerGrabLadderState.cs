using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrabLadderState : PlayerTouchingLadderState
{
        private Vector2 holdPosition;
        private Vector2 desiredPosition;
        private bool areWeThereYet;

    public PlayerGrabLadderState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

      //  float y = player.transform.position.y;
      //  desiredPosition = new Vector2(ladderCenter, y);

        holdPosition = new Vector2(ladderCenter, player.transform.position.y);

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

            if (yInput > 0)
            {
                stateMachine.ChangeState(player.LadderClimbState);
            }
            else if ((yInput < 0 || !grabInput))
            {
                stateMachine.ChangeState(player.LadderDescendState);
            }
        }

    }

    private void HoldPosition()
    {

            player.transform.position = holdPosition;

            player.SetVelocityX(0f);
            player.SetVelocityY(0f);

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
