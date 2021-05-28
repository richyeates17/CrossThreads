using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    //Enums
    public enum MovementType
    {
        MoveTowards,
        LerpTowards
    }

    //Public Variables
    public MovementType Type = MovementType.MoveTowards;
    public MovementPath MyPath;
    public float Speed = 1f;
    public float MaxDistanceToGoal = 0.1f;

    //Private Variables
    private IEnumerator<Transform> pointInPath;

    //Unity Methods
    void Start()
    {
        if(MyPath == null)
        {
            Debug.LogError("Movement Path cannot be null", gameObject);
            return;
        }

        pointInPath = MyPath.GetNextPathPoint();

        pointInPath.MoveNext();

        if(pointInPath.Current == null)
        {
            Debug.LogError("A path must have points to follow",gameObject);
            return;
        }

        transform.position = pointInPath.Current.position;
    }

    void Update()
    {
        if(pointInPath == null || pointInPath.Current == null)
        {
            return;
        }

        if(Type == MovementType.MoveTowards)
        {
            transform.position = Vector3.MoveTowards(transform.position, pointInPath.Current.position, Time.deltaTime * Speed);
        }
        else if (Type == MovementType.LerpTowards)
        {
            transform.position = Vector3.Lerp(transform.position, pointInPath.Current.position, Time.deltaTime * Speed);
        }

        var distanceSquared = (transform.position - pointInPath.Current.position).sqrMagnitude;
        if(distanceSquared < MaxDistanceToGoal * MaxDistanceToGoal)
        {
            pointInPath.MoveNext();
        }
    }

}
