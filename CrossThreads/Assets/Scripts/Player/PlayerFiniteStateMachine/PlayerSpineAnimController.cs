using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class PlayerSpineAnimController : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset idle;
    public AnimationReferenceAsset run;
    public AnimationReferenceAsset jump;
    public AnimationReferenceAsset land;
    public AnimationReferenceAsset wallSlide;
    public AnimationReferenceAsset wallGrab;
    public AnimationReferenceAsset wallClimb;
    public AnimationReferenceAsset ledgeGrab;
    public AnimationReferenceAsset ledgeClimb;
    public AnimationReferenceAsset crouchIdle;
    public AnimationReferenceAsset crouchMove;
    public string currentState;
    public string currentAnimation;
    public string endedAnimation;

    //sets character animation
    public void SetAnimation(AnimationReferenceAsset animation, bool loop, float timeScale)
    {
        if(animation.name.Equals(currentAnimation))
        {
            return;
        }
        skeletonAnimation.state.SetAnimation(0, animation, loop).TimeScale = timeScale;
    }

    //used to call animation state change from Player State
    public void SetBool(string state, bool loop)
    {
        if (state == "idle")
        {
            SetAnimation(idle, loop, 1f);
        }

        if (state == "move")
        {
            SetAnimation(run, loop, 1f);
        }

        if (state == "inAir")
        {
            SetAnimation(jump, loop, 1f);
        }

        if (state == "land")
        {
            SetAnimation(land, loop, 1f);
        }

        if (state == "wallSlide")
        {
            SetAnimation(wallSlide, loop, 1f);
        }

        if (state == "wallGrab")
        {
            SetAnimation(wallGrab, loop, 1f);
        }

        if (state == "wallClimb")
        {
            SetAnimation(wallClimb, loop, 1f);
        }

        if (state == "ledgeGrab")
        {
            SetAnimation(ledgeGrab, loop, 1f);
        }

        if (state == "ledgeClimb")
        {
            SetAnimation(ledgeClimb, loop, 1f);
        }

        if (state == "crouchIdle")
        {
            SetAnimation(crouchIdle, loop, 1f);
        }

        if (state == "crouchMove")
        {
            SetAnimation(crouchMove, loop, 1f);
        }

        currentAnimation = state;
    }

    

}
