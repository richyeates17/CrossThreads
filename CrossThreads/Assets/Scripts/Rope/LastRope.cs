using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastRope : MonoBehaviour
{
    #region Fields

    // Make a list for all the positions the player could go to on the rope
    List<Transform> newPlayerRopePositions = new List<Transform>();

    // Declare a variable to keep track of the player's current rope position and speed 
    [HideInInspector] public int positionNumber;
    bool playerIsMoving;
    float playerSlideSpeedOnRope=10;
    // I gave it an initial value because of the floating point accuracy error in unity 
    //where something like 0.000001 doesn't equal 0 so the player could get stuck if this doesn't have any value

    // Declare the variables used for rope/character detection
    [HideInInspector] public bool canConnect = true;
    [HideInInspector] public bool connected;
    int numberOfTransformsPerSegment;
    bool movingRope = false;
    [HideInInspector] public bool firstConnection;

    // Declare timing variable so that the player doesnt instantly reconnect to the rope
    float reconnectElapsedTime;

    // Declare the variables used to return character to original state after dismounting the rope
    bool resetGravity;
    float originalPlayerGravityScale;
    float originalPlayerMass;
    bool dismountRope;
    [HideInInspector] public int originalPlayerLayer;

    // Get reference to player and player controller script
    Player player;
    Vector3 playerScale;
    float lookPosition = 1;
    
    // Declare the variables responsible for the player animation and sound effects based on movement
    bool idleAnimation;
    bool climbingAnimation;
    bool fallingAnimation;

    // Rope Settings

    [HideInInspector]
    [Tooltip("Hide the Last Rope From View")]
    public bool hideLastRope;

    [HideInInspector]
    [Tooltip("Always move player to the lower rope attach position upon first collision")]
    public bool alwaysMountToLowerPosition;

    [HideInInspector]
    [Tooltip("Allows the player to climb the rope")]
    public bool climbable;

    [Tooltip("Disables the runtime check for changes to slightly optimize performance")]
    bool optimizations;

    // Player Settings

    [Tooltip("Instantly teleport the player to the correct rope position")]
    bool instantMount;

    [Tooltip("Player mounting speed used when instant mount is off")]
    float playerMountSpeed;
    
    [Tooltip("Player movement speed when moving upwards after mounting is done")]
    float playerClimbSpeedOnRope = 4f;

    [Tooltip("Player movement speed when moving downwards after mounting is done")]
    float playerFallSpeedOnRope = 4f;

    [Tooltip("Time before player can reconnect to the rope")]
    float reconnectCooldownTime;

    [Tooltip("Hold button to move player on rope")]
    bool holdButtonToMove;

    [Tooltip("Leave the rope after reaching the last segment of the rope")]
    bool dismountRopeAfterLastSegment;

    [HideInInspector]
    [Tooltip("Player x-axis offset")]
    public float playerX_AxisOffset;

    [HideInInspector]
    [Tooltip("Player y-axis offset")]
    public float playerY_AxisOffset;

    [Tooltip("Player's leave rope gravity scale")]
    float leaveRopeGravityScale = 7f;

    protected int xInput;
    protected int yInput;

    private bool jumpInput;

    #endregion

    #region Unity Methods
    void Awake()
    {
        AdjustRopeSettings();
    }

    void Start()
    {
        // Assign the player using his tag and grab some components and values from him
        player = FindObjectOfType<Player>();
        originalPlayerGravityScale = player.GetComponent<Rigidbody2D>().gravityScale;
        originalPlayerMass = player.GetComponent<Rigidbody2D>().mass;
        playerScale = player.transform.localScale;
        originalPlayerLayer = player.gameObject.layer;

        // Check if create rope script is being used
        if (!transform.parent.parent.TryGetComponent<CreateRope>(out CreateRope createRope))
        {
            // If the create rope script (for adjustable size rope) is not being used then assign the connected rigidbody
            // In this example, the connected rigidbody is the player
            GetComponent<HingeJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
        }

        else
        {
            hideLastRope = createRope.hideLastRope;
        }

        if (hideLastRope)
        {
            // Remove the sprite & capsule collider and store the number of player-rope positions
            GetComponent<SpriteRenderer>().sprite = null;
            GetComponent<CapsuleCollider2D>().enabled = false;
            numberOfTransformsPerSegment = transform.childCount;
        }

        // Add the player-rope positions to the array
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            for (int j = 0; j < transform.childCount; j++)
            {
                newPlayerRopePositions.Add(transform.parent.GetChild(i).GetChild(j).transform);
            }
        }
    }

    void Update()
    {

        xInput = player.InputHandler.NormInputX;
        yInput = player.InputHandler.NormInputY;
        jumpInput = player.InputHandler.JumpInput;

        // Check if optimizations is turned on to stop reading values more than once during runtime
        if (!optimizations)
        {            
            AdjustRopeSettings();
        }

        // Set the reconnect timer up so that you can only reconnect to the rope when it's finished
        if (reconnectElapsedTime >= reconnectCooldownTime)
        {
            canConnect = true;
            if (!player.alreadyConnected && dismountRope)
            {
                dismountRope = false;
                player.gameObject.layer = originalPlayerLayer;
            }
        }

        // Increase the timer till it's done
        else if (reconnectElapsedTime < reconnectCooldownTime)
        {
            // Increase the elapsed time if it's less than the reconnect time
            reconnectElapsedTime += Time.deltaTime;
        }

        // Check for connection with the rope and adjust the player position and rotation
        if (connected)
        {
            // Mark the player as already connected so that he doesn't connect to multiple ropes at once
            player.alreadyConnected = true;

            // Check if player moved to a different segment
            if (player.transform.parent != newPlayerRopePositions[positionNumber].parent)
            {
                ChangePlayerParentAndScale();
            }
            // Check if rope is climbable or not so that you can assign the player-rope position
            if (!climbable)
            {
                // attach the player to the last segment
                positionNumber = newPlayerRopePositions.Count - 1 - numberOfTransformsPerSegment;
            }

            MoveAndRotatePlayer();
        }

        // Reset the gravity scale when player is on the ground
        else if (resetGravity && player.CheckIfGrounded())
        {
            resetGravity = false;
            player.GetComponent<Rigidbody2D>().gravityScale = originalPlayerGravityScale;
        }

        // Check for keyboard input to make the player dismount the rope
        if (jumpInput && connected)
        {
            DismountRope();

            // Adjust the gravity scale to get a floating effect
            player.GetComponent<Rigidbody2D>().gravityScale = leaveRopeGravityScale;

            // Call the jump method in the player controller script and give it a jump height value
            player.StateMachine.ChangeState(player.JumpState);

        }

        // Dismount the player from the rope if he tries to go even lower than the last attach position
        if (yInput <= 0.1 && positionNumber == newPlayerRopePositions.Count - 1 - numberOfTransformsPerSegment
            && dismountRopeAfterLastSegment && connected)
        {
            DismountRope();

        }

        // Check if move rope script is disabled
        if (!transform.parent.GetComponent<MoveRope>().enabled)
        {
            movingRope = false;
            if (connected)
            {
                player.GetComponent<Rigidbody2D>().mass = originalPlayerMass;
            }
        }

        // Check if move rope script is enabled
        else if (transform.parent.GetComponent<MoveRope>().enabled)
        {
            if (connected)
            {
                // Nullify the player's mass so that he doesn't affect the rope's movement
                player.GetComponent<Rigidbody2D>().mass = 0;
                if (!movingRope)
                {
                    // Dismount the character if he was already connected before the rope started moving to avoid glitches
                    DismountRope();

                    // Instantly enable the character to reconnect again if he touches the rope
                    reconnectElapsedTime = reconnectCooldownTime;
                }
            }
            movingRope = true;
        }
    }

    #endregion

    #region Custom Methods

    /// <summary>
    /// Detach the player from the rope
    /// </summary>
    public void DismountRope()
    {
        // Unparent the rope
        player.transform.SetParent(null);

        // Adjust the scale
        if(player.transform.localScale.x > 0)
        {
            player.transform.localScale = new Vector3(Mathf.Abs(playerScale.x), playerScale.y, playerScale.z);
        }

        else
        {
            player.transform.localScale = new Vector3(-Mathf.Abs(playerScale.x), playerScale.y, playerScale.z);
        }

        // Reset all the variables and reconnect timer
        resetGravity = true;
        canConnect = false;
        reconnectElapsedTime = 0;
        connected = false;
        player.alreadyConnected = false;
        dismountRope = true;
        idleAnimation = false;

        // Disable the hinge joint to free the player
        GetComponent<HingeJoint2D>().enabled = false;

        // Reset the player rotation
        player.transform.rotation = Quaternion.identity;

        // Reset the player mass if the rope was moving autonomously
        player.GetComponent<Rigidbody2D>().mass = originalPlayerMass;
    }

    /// <summary>
    /// Change the player-rope position based on user input
    /// </summary>
    void HandlePlayerPosition()
    {
        // You can adjust this if statement if you don't want the player to climb above and/or fall below a certain segment
        if (yInput == 1 || (yInput == 1 && holdButtonToMove) && positionNumber > 0)
        {
            // Decrease the position number to climb up
            positionNumber--;

            // Change the player's movement speed to the climbing speed
            playerSlideSpeedOnRope = playerClimbSpeedOnRope;

            // Change the values of the variables responsible for the animations and sound effects
            playerIsMoving = true;

            // Reset these animations to false so that they can play again
            idleAnimation = false;
            fallingAnimation = false;
        }
        else if ((yInput == -1) ||
            (yInput == -1 || yInput == -1 && holdButtonToMove)
            && positionNumber < newPlayerRopePositions.Count - 1 - numberOfTransformsPerSegment)
        {
            // Increase the position number to fall down
            positionNumber++;

            // Change the player's movement speed to the falling speed
            playerSlideSpeedOnRope = playerFallSpeedOnRope;

            // Change the values of the variables responsible for the animations and sound effects
            playerIsMoving = true;

            // Reset these animations to false so that they can play again
            idleAnimation = false;
            climbingAnimation = false;
        }
    }

    /// <summary>
    /// Move and rotate the player when attached to the rope
    /// </summary>
    void MoveAndRotatePlayer()
    {
        // Only move when the player hasn't reached his destination yet
        if (!Mathf.Approximately(player.transform.localPosition.y,
            newPlayerRopePositions[positionNumber].localPosition.y+playerY_AxisOffset))
        {
            if (firstConnection)
            {               
                if (!instantMount)
                {
                    // Smoothly mount the player to the rope
                    player.transform.localPosition = new Vector2(playerX_AxisOffset, Mathf.MoveTowards(player.transform.localPosition.y,
                        newPlayerRopePositions[positionNumber].localPosition.y + playerY_AxisOffset, playerMountSpeed * Time.deltaTime));

                    playerIsMoving = true;
                }
                else
                {                   
                    // Instantly connect the player to the rope
                    player.transform.localPosition = new Vector2(playerX_AxisOffset, newPlayerRopePositions[positionNumber].localPosition.y+playerY_AxisOffset);
                }
            }
            else
            {
                // Move the player through the rope smoothly
                player.transform.localPosition = new Vector2(playerX_AxisOffset, Mathf.MoveTowards(player.transform.localPosition.y,
                    newPlayerRopePositions[positionNumber].localPosition.y + playerY_AxisOffset, playerSlideSpeedOnRope * Time.deltaTime));
            }
        }

        // Adjust the player rotation
        player.transform.localEulerAngles = newPlayerRopePositions[positionNumber].localEulerAngles;

        HandlePlayerState();

        // Check if player has reached his destination
        if (Mathf.Approximately(player.transform.localPosition.y,
            newPlayerRopePositions[positionNumber].localPosition.y + playerY_AxisOffset))
        {
            firstConnection = false;
            playerIsMoving = false;
     
            if(climbable)
            {
                // Assign the new player destination
                HandlePlayerPosition();
            }
        }

        // Change the player's facing direction based on the user input
        if (player.FacingDirection > 0)
        {
            lookPosition = 1;
        }

        else if (player.FacingDirection < 0)
        {
            lookPosition = -1;
        }
    }

    /// <summary>
    /// Check if create rope script is being used since you can't access the last rope script from
    /// the inspector before runtime to adjust the variables when using the Adjustable Size Rope
    /// </summary>
    void AdjustRopeSettings()
    {
        // Check if create rope is script is being used
        if (transform.parent.parent.TryGetComponent<CreateRope>(out CreateRope createRope))
        {
            // Read all the values from the create rope script
            climbable = createRope.climbable;
            leaveRopeGravityScale = createRope.leaveRopeGravityScale;
            playerClimbSpeedOnRope = createRope.playerClimbSpeedOnRope;
            playerFallSpeedOnRope = createRope.playerFallSpeedOnRope;
            reconnectCooldownTime = createRope.reconnectCooldownTime;
            holdButtonToMove = createRope.holdButtonToMove;
            dismountRopeAfterLastSegment = createRope.dismountRopeAfterLastSegment;
            playerX_AxisOffset = createRope.playerX_AxisOffset;
            playerY_AxisOffset = createRope.playerY_AxisOffset;
            alwaysMountToLowerPosition = createRope.alwaysMountToLowerPosition;
            instantMount = createRope.instantMount;
            playerMountSpeed = createRope.playerMountSpeed;
            optimizations = createRope.optimizations;
        }

        else
        {
            // Read all the values from the create rope script
            FixedRope fixedRope = transform.parent.parent.GetComponent<FixedRope>();
            climbable = fixedRope.climbable;
            leaveRopeGravityScale = fixedRope.leaveRopeGravityScale;
            playerClimbSpeedOnRope = fixedRope.playerClimbSpeedOnRope;
            playerFallSpeedOnRope = fixedRope.playerFallSpeedOnRope;
            reconnectCooldownTime = fixedRope.reconnectCooldownTime;
            holdButtonToMove = fixedRope.holdButtonToMove;
            dismountRopeAfterLastSegment = fixedRope.dismountRopeAfterLastSegment;
            playerX_AxisOffset = fixedRope.playerX_AxisOffset;
            playerY_AxisOffset = fixedRope.playerY_AxisOffset;
            alwaysMountToLowerPosition = fixedRope.alwaysMountToLowerPosition;
            instantMount = fixedRope.instantMount;
            playerMountSpeed = fixedRope.playerMountSpeed;
            optimizations = fixedRope.optimizations;
        }
    }

    /// <summary>
    /// Changes the parent of the player to the segment he is currently on and negates the scale changes that result from changing parents
    /// </summary>
    void ChangePlayerParentAndScale()
    {
        // Set the parent of the player so that he moves with the rope
        player.transform.SetParent(newPlayerRopePositions[positionNumber].transform.parent, true);       

        // Adjust the player scale when changing parents so that the scale doesn't change
        player.transform.localScale = new Vector3(Mathf.Abs(playerScale.x) * lookPosition / transform.localScale.x,
            playerScale.y / transform.localScale.y, playerScale.z / transform.localScale.z);
    }

    /// <summary>
    /// Sends the player state to the player controller script based on user input and player position on rope
    /// </summary>
    void HandlePlayerState()
    {
        // Check if player is moving
        if (playerIsMoving)
        {
            // Check if player is above the rope position he is heading towards and isn't already playing the falling animation
            if (player.transform.localPosition.y > newPlayerRopePositions[positionNumber].localPosition.y && !fallingAnimation)
            {
                // Set the falling animation to true so that the animation plays only once
                fallingAnimation = true;

            }
            // Check if player is below the rope position he is heading towards and isn't already playing the climbing animation
            else if (player.transform.localPosition.y < newPlayerRopePositions[positionNumber].localPosition.y && !climbingAnimation)
            {
                // Set the climbing animation to true so that the animation plays only once
                climbingAnimation = true;

            }

        }

        else if(!idleAnimation)
        {
            // Set the idle animation to true so that the animation plays only once
            idleAnimation = true;

            // Reset these animations to false so that they can play again
            fallingAnimation = false;
            climbingAnimation = false;

        }        
    }
    #endregion
}
