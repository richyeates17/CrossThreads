using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MovementPath : MonoBehaviour
{
    //Enums
    public enum PathTypes
    {
        linear,
        loop
    }

    //Public Variables
    public PathTypes PathType;
    public int movementDirection = 1; //clockwise/forward = 1, counter clockwise/ backwards = -1
    public int movingTo = 0;
    public Transform[] PathSequence;

    //Unity Methods
    private void OnDrawGizmos()
    {
        if(PathSequence == null || PathSequence.Length < 2)
        {
            return;
        }

        for (var i=1; i< PathSequence.Length; i++)
        {
            Gizmos.DrawLine(PathSequence[i - 1].position, PathSequence[i].position);
        }

        if(PathType == PathTypes.loop)
        {
            Gizmos.DrawLine(PathSequence[0].position, PathSequence[PathSequence.Length - 1].position);
        }
    }

    //Coroutine
    public IEnumerator<Transform> GetNextPathPoint()
    {
        if(PathSequence == null || PathSequence.Length <1)
        {
            yield break;
        }

        while(true)
        {
            yield return PathSequence[movingTo];

            if(PathSequence.Length == 1)
            {
                continue;
            }

            if(PathType == PathTypes.linear)
            {
                if(movingTo <= 0)
                {
                    movementDirection = 1;
                }
                else if (movingTo >= PathSequence.Length - 1)
                {
                    movementDirection = -1;
                }
            }

            movingTo = movingTo + movementDirection;

            if (PathType == PathTypes.loop)
            {
                if(movingTo >= PathSequence.Length)
                {
                    movingTo = 0;
                }
                if (movingTo < 0)
                {
                    movingTo = PathSequence.Length - 1;
                }
            }
        }
    }

}
