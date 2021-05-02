using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateRope : MonoBehaviour
{
    #region Fields

    // Declare variables to hold the rope gameobjects
    Transform links;
    [Tooltip("This is the first segment of the rope. Please don't change this")]
    public GameObject firstSegment;
    GameObject rope;
    GameObject newRope;
    GameObject emptyGameObjectPrefab;

    // Declare variables to store the first segment's initial position and rotation
    Vector3 linkOneOriginalPosition;
    Quaternion linkOneOriginalRotation;

    // Declare a variable to hold the player gameobject
    GameObject player;

    // Declare the new rope segment spawn location and offset
    Vector2 spawnLocation;
    float spawnLocationOffset;

    //Declare variables that are used for assigning the rope attach positions
    float ropeAttachPosition;
    float segmentSize;

    // Declare the joint components
    HingeJoint2D hingeJoint2D;
    DistanceJoint2D distanceJoint2D;

    // Declare all the public variables that can be edited from the inspector
    [Header("Rope Settings")]

    [Range(2, 50, order = 1)] [Tooltip("Number of rope segments")] [Delayed] 
    public int ropeSegments;

    [Tooltip("Hide the Last Rope From View")] 
    public bool hideLastRope;

    [Tooltip("Always move player to the lower rope attach position upon first collision")] 
    public bool alwaysMountToLowerPosition;

    [Tooltip("Automatically create evenly spaced rope attach positions. " +
        "You must make sure that your first rope segment doesn't have any child game objects under it before enabling this")] 
    public bool autoAssignRopeAttachPositions;

    [Min(2)] [Tooltip("Number of rope attach positions to create")] 
    public int numberOfRopeAttachPositions;

    [Tooltip("Vertical offset of the rope attach positions")] 
    public float ropeAttachPositionOffset;

    [Tooltip("Allows the player to climb the rope")] 
    public bool climbable;

    [Tooltip("Adjust the rigid body mass of all rope segments")]
    public float ropeLinksMass = 50;

    [Tooltip("Adjust the rigid body gravity scale of all rope segments")]
    public float ropeLinksGravityScale = 1;

    [Tooltip("Disables the runtime check for changes to slightly optimize performance")]
    public bool optimizations;

    [Tooltip("Makes the rope collide with other objects. " +
            "You must make sure you have a layer called Rope (with a capital R) for this to work")]
    public bool enableRopeCollisions;

    [Tooltip("Makes the rope stationary")]
    public bool staticRope;

    [Header("Player Settings")]

    [Tooltip("Instantly teleport the player to the correct rope position")] 
    public bool instantMount;

    [Range(1, 10)] [Tooltip("Player mounting speed used when instant mount is off")] 
    public float playerMountSpeed;

    [Range(1, 10)]
    [Tooltip("Player movement speed when moving upwards after mounting is done")]
    public float playerClimbSpeedOnRope = 4f;

    [Range(1, 10)]
    [Tooltip("Player movement speed when moving downwards after mounting is done")]
    public float playerFallSpeedOnRope = 4f;

    [Min(0.1f)] [Tooltip("Time before player can reconnect to the rope")] 
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

    #region Unity Methods

    void Awake()
    {
        links = transform.GetChild(1);
        // Store the original position and rotation
        linkOneOriginalPosition = firstSegment.transform.position;
        linkOneOriginalRotation = firstSegment.transform.rotation;

        // Grab the empty game object prefab from the resources folder
        emptyGameObjectPrefab = (GameObject)Resources.Load("AutoRopeAttachPosition");

        // Assign the player using his tag
        player = GameObject.FindGameObjectWithTag("Player");

        // Assign the distance joint found in the anchor
        distanceJoint2D = transform.GetChild(0).GetComponent<DistanceJoint2D>();

        // Calculate the offset from the sprite y size
        spawnLocationOffset = firstSegment.GetComponent<SpriteRenderer>().bounds.size.y - 0.01f;

        BuildRope();

        ConnectRope();
        
    }

    private void Update()
    {
        // Check if optimizations option is turned on
        if(optimizations)
        {
            // Exit from the update method
            return;
        }

        // Check if rope size was changed
        if (ropeSegments != links.childCount)
        {
            if (links.GetComponentInChildren<LastRope>().connected)
            {
                // Disconnect the player from the rope so that he can connect again automatically after rope is built
                links.GetComponentInChildren<LastRope>().DismountRope();
                player.layer = links.GetComponentInChildren<LastRope>().originalPlayerLayer;
            }

            for (int i = 1; i < links.childCount; i++)
            {
                // Destroy the previous rope segments except the first link because it contains this script
                // this is why the for loop started at 1 not 0
                Destroy(links.GetChild(i).gameObject);
            }

        }
    }

    private void LateUpdate()
    {
        // Check if optimizations option is turned on
        if (optimizations)
        {
            // Exit from the fixed update method
            return;
        }

        // Create the new rope if the rope size was changed
        if (ropeSegments != links.childCount)
        {
            // Reset the first segment's position and rotation in order to connect the other links properly
            firstSegment.transform.position = linkOneOriginalPosition;
            firstSegment.transform.rotation = linkOneOriginalRotation;

            BuildRope();

            ConnectRope();
        }
    }

    void OnDrawGizmosSelected()
    {
        // This gives a visual representation of the rope attach position limits in the editor
        Gizmos.color=Color.red;
        Gizmos.DrawWireCube(firstSegment.transform.position, new Vector2(0, firstSegment.GetComponent<SpriteRenderer>().bounds.size.y - ropeAttachPositionOffset));
    }
    #endregion

    #region Custom Methods

    /// <summary>
    /// Creates the rope based on given rope size
    /// </summary>
    void BuildRope()
    {
        // Check if auto assign rope attach positions is on in order to assign the attach positions of the first link automatically
        if(autoAssignRopeAttachPositions)
        {
            AssignRopePositions(firstSegment.gameObject);
        }

        for (int i = 0; i < ropeSegments - 1; i++)
        {
            // Assign the rope segments spawn locations based on their current position and offset
            spawnLocation = (Vector2)links.GetChild(i).transform.position - new Vector2(0, spawnLocationOffset);

            if (i < ropeSegments - 2)
            {
                // Get the rope link prefab from the Resources folder if last rope part isn't reached yet
                if (autoAssignRopeAttachPositions)
                {
                    rope = (GameObject)Resources.Load("AutoAssignRopeLink");
                }
                else
                {

                    rope = (GameObject)Resources.Load("RopeLink");
                }
            }

            else
            {
                // Get the last rope prefab from the Resources folder if this is the last loop
                if (autoAssignRopeAttachPositions)
                {
                    rope = (GameObject)Resources.Load("AutoAssignLastRope");
                }
                else
                {

                    rope = (GameObject)Resources.Load("LastRope");
                }
            }

            // Spawn the rope pieces in their corresponding locations
            newRope = Instantiate(rope, spawnLocation, Quaternion.identity,links);


            // Assign the rope pieces parent and scale
            newRope.transform.localScale = firstSegment.transform.localScale;

            // Change the sprite and color of the links to match the first segment
            newRope.GetComponent<SpriteRenderer>().sprite = firstSegment.GetComponent<SpriteRenderer>().sprite;
            newRope.GetComponent<SpriteRenderer>().color =firstSegment.GetComponent<SpriteRenderer>().color;

            AssignRopePositions(newRope);
        }
    }

    /// <summary>
    /// Connects the rope segments to the right rigidbodies
    /// </summary>
    void ConnectRope()
    {
        for (int i = 0; i < ropeSegments; i++)
        {
            // Get the rope segments and their respective hinge joints to assign the connected rigidbody of each hinge joint
            rope = links.GetChild(i).gameObject;
            hingeJoint2D = rope.GetComponent<HingeJoint2D>();

            if (i < ropeSegments - 1)
            {
                // Each hinge joint is assigned the rigidbody of the next piece except the last rope's hinge joint
                hingeJoint2D.connectedBody = links.GetChild(i + 1).gameObject.GetComponent<Rigidbody2D>();
            }

            else
            {
                // Last rope's hinge joint is assigned the player's rigidbody and is also connected to the distance joint of the anchor
                hingeJoint2D.connectedBody = player.GetComponent<Rigidbody2D>();
                distanceJoint2D.connectedBody = rope.GetComponent<Rigidbody2D>();
            }
        }
    }

    /// <summary>
    /// Assign the rope positions to their respectful locations
    /// </summary>
    /// <param name="newRope"></param>
    void AssignRopePositions(GameObject newRope)
    {      
        if (!autoAssignRopeAttachPositions)
        {
            // Adjust all the player-rope attach locations to be the same as the first link
            for (int z = 0; z < newRope.transform.childCount; z++)
            {
                newRope.transform.GetChild(z).localPosition = firstSegment.transform.GetChild(z).localPosition;
            }
        }

        else
        {
            // Calculate the parameters used for assigning the rope attach postions
            ropeAttachPosition = firstSegment.GetComponent<SpriteRenderer>().bounds.extents.y - ropeAttachPositionOffset / 2;
            segmentSize =firstSegment.GetComponent<SpriteRenderer>().bounds.size.y - ropeAttachPositionOffset;

            // Create the empty child game objects used as rope attach posiions
            for (int i = 0; i < numberOfRopeAttachPositions; i++)
            {
                Instantiate(emptyGameObjectPrefab, new Vector2(newRope.transform.position.x,
                    newRope.transform.position.y + ropeAttachPosition), Quaternion.identity, newRope.transform);
                ropeAttachPosition -= segmentSize / (numberOfRopeAttachPositions - 1);
            }
        }
    }
    #endregion
}
