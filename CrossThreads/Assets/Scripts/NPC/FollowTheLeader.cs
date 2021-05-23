using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct PlayerStruct
{
    public Vector3 Position;
    public int FacingDirection;
    public string CurrentAnimation;
}

public class FollowTheLeader : MonoBehaviour
{
    public Player leader; // the game object to follow - assign in inspector

    [SerializeField]
    private int steps; // number of steps to stay behind - assign in inspector

    public PlayerSpineAnimController Anim { get; private set; }
    private int myFacingDirection;
    private string imRunningThisAnimationAlready;

    //private Queue<Vector3> record = new Queue<Vector3>();
    //private Vector3 lastRecord;

    PlayerStruct TheLeader;
    PlayerStruct TheMe;
    private Queue<PlayerStruct> record = new Queue<PlayerStruct>();

    private void Start()
    {
        Anim = GetComponent<PlayerSpineAnimController>();
        TheLeader = new PlayerStruct();
        imRunningThisAnimationAlready = "idle";
        myFacingDirection = 1;
    }

    void FixedUpdate()
    {
        TheLeader.Position = leader.transform.position;
        TheLeader.FacingDirection = leader.FacingDirection;
        TheLeader.CurrentAnimation = leader.GetComponent<PlayerSpineAnimController>().currentAnimation;

        // record details of leader
        record.Enqueue(TheLeader);

        // remove last details from the record and use it for our own
        if (record.Count > steps)
        {
            TheMe = record.Dequeue();
            this.transform.position = TheMe.Position;
            if (TheMe.FacingDirection != myFacingDirection)
              {
                  myFacingDirection *= -1;
                  transform.Rotate(0.0f, 180.0f, 0.0f);
              }
            if(TheMe.CurrentAnimation != imRunningThisAnimationAlready)
            {
                imRunningThisAnimationAlready = TheMe.CurrentAnimation;
                this.Anim.SetBool(TheMe.CurrentAnimation, true);
            }

        }
    }
}
