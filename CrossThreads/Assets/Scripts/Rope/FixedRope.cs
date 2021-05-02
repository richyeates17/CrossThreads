using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedRope : MonoBehaviour
{
    #region Fields

    // Declare all the public variables that can be edited from the inspector
    [Header("Rope Settings")]

    [Tooltip("Hide the Last Rope From View")]
    public bool hideLastRope;

    [Tooltip("Always move player to the lower rope attach position upon first collision")]
    public bool alwaysMountToLowerPosition;

    [Tooltip("Allows the player to climb the rope")]
    public bool climbable;

    [Tooltip("Adjust the rigid body mass of all rope segments")]
    public float ropeLinksMass = 50;

    [Tooltip("Adjust the rigid body gravity scale of all rope segments")]
    public float ropeLinksGravityScale = 1;

    [Tooltip("Disables the runtime check for changes to slightly optimize performance")]
    public bool optimizations; // should always be true if not using the adjustable size rope

    [Tooltip("Makes the rope collide with other objects. " +
        "You must make sure you have a layer called Rope (with a capital R) for this to work")]
    public bool enableRopeCollisions;

    [Tooltip("Makes the rope stationary")]
    public bool staticRope;

    [Header("Player Settings")]

    [Tooltip("Instantly teleport the player to the correct rope position")]
    public bool instantMount;

    [Range(1, 10)]
    [Tooltip("Player mounting speed used when instant mount is off")]
    public float playerMountSpeed;

    [Range(1, 10)]
    [Tooltip("Player movement speed when moving upwards after mounting is done")]
    public float playerClimbSpeedOnRope = 4f;

    [Range(1, 10)]
    [Tooltip("Player movement speed when moving downwards after mounting is done")]
    public float playerFallSpeedOnRope = 4f;

    [Min(0.1f)]
    [Tooltip("Time before player can reconnect to the rope")]
    public float reconnectCooldownTime;

    [Tooltip("Hold button to move player on rope")]
    public bool holdButtonToMove;

    [Tooltip("Leave the rope after reaching the last segment of the rope")]
    public bool dismountRopeAfterLastSegment;

    [Tooltip("Player x-axis offset")]
    public float playerX_AxisOffset;

    [Tooltip("Player y-axis offset")]
    public float playerY_AxisOffset;

    [Tooltip("Player's leave rope gravity scale")]
    public float leaveRopeGravityScale = 7f;
    #endregion
}
