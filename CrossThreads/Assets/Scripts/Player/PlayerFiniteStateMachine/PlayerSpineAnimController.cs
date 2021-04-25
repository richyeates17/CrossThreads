using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class PlayerSpineAnimController : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset idle;
    public AnimationReferenceAsset run;
    public string currentState;
    public string currentAnimation;

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

        currentAnimation = state;
    }
}
