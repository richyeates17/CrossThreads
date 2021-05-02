using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRope : MonoBehaviour
{
    #region Fields

    // Declare a variable for the rigid body 2D component
    Rigidbody2D rb2d;

    // Declare the parameters responsible for the rope movement
    public float moveSpeed = 150;
    public float leftAngle = -0.3f;
    public float rightAngle = 0.3f;
    bool movingClockwise = true;
    #endregion

    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        // Assign the rigid body 2D to the rigid body of the first link
        rb2d = GetComponentInChildren<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }
    #endregion

    #region Custom Methods

    /// <summary>
    /// Changes the rope direction based on the angle
    /// </summary>
    public void ChangeMoveDir()
    {
        // Check if rope's current angle is greater than the assigned right angle to change the rope direction
        if (transform.GetChild(0).rotation.z >= rightAngle)
        {
            movingClockwise = false;
        }
        // Check if rope's current angle is less than the assigned left angle to change the rope direction
        else if (transform.GetChild(0).rotation.z <= leftAngle)
        {
            movingClockwise = true;
        }
    }

    /// <summary>
    /// Moves the rope
    /// </summary>
    public void Move()
    {
        ChangeMoveDir();

        // Rotate the rope using angular velocity
        if (movingClockwise)
        {
            rb2d.angularVelocity = moveSpeed * 50 * Time.fixedDeltaTime;
        }

        else if (!movingClockwise)
        {
            rb2d.angularVelocity = -1 * moveSpeed * 50 * Time.fixedDeltaTime;
        }
    }
    #endregion
}
