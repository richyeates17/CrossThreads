using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeLinks : MonoBehaviour
{
    #region Fields

    // Declare the last rope script as well as the player controller script
    LastRope lastRope;
    Player player;

    // Declare variables to adjust the rope mass and gravity scale for all ropes
    float ropeLinksMass;
    float ropeGravityScale;

    // Declare a variable to store the rigidbody of the rope
    Rigidbody2D rb2d;

    // Declare variables used for detecting the move rope script and resetting the rope's position
    bool movingRope = false;
    Vector2 originalPosition;

    // Declare variables used to set up the position formula based on your preferences
    int numberOfTransformsPerSegment;
    int transformIndexToAttachToInEachSegment;
    Vector2 contactPoint;
    float distance;
    float closestDistance;

    // Declare some miscellaneous variables based on user preferences
    bool optimizations;
    bool enableRopeCollisions;
    bool staticRope;
    #endregion

    #region Unity Methods

    void Start()
    {        
        //Assign the rigidbody to the variable
        rb2d = GetComponent<Rigidbody2D>();

        // Assign the number of rope attach positions per segment
        numberOfTransformsPerSegment = transform.childCount;

        // Assign the player controller script
        player = FindObjectOfType<Player>();

        // Store the original position
        originalPosition = transform.position;

        // Set the anchor position of the segment to the bottom based on the sprite y extent
        GetComponent<HingeJoint2D>().anchor = new Vector2(0, -transform.parent.GetComponentInChildren<SpriteRenderer>().sprite.bounds.extents.y);

        // Check if this is the first segment
        if (transform.GetSiblingIndex() == 0)
        {
            // Adjust the second hinge joint anchor position to the top
            GetComponents<HingeJoint2D>()[1].anchor = new Vector2(0, GetComponent<SpriteRenderer>().sprite.bounds.extents.y);
        }

        else
        {
            // Adjust the other links collider sizes and offset to match the first segment
            GetComponent<CapsuleCollider2D>().size = transform.parent.GetComponentInChildren<CapsuleCollider2D>().size;
            GetComponent<CapsuleCollider2D>().offset = transform.parent.GetComponentInChildren<CapsuleCollider2D>().offset;
        }

        AdjustRopeSettings();

        CheckForMovingRope();

    }

    void Update()
    {
        // Check if optimizations is turned on
        if (optimizations)
        {
            // Exit the update method
            return;
        }

        // Check if autonomous moving rope script is being used
        CheckForMovingRope();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check for collision with the player and if player is already attached to a rope
        if (collision.gameObject.CompareTag("Player") && !player.alreadyConnected)
        {
            lastRope = transform.parent.GetComponentInChildren<LastRope>();
            if (lastRope.canConnect)
            {
                // Store the player contact point with the rope
                contactPoint = collision.ClosestPoint(collision.transform.position);

                AssignPlayerOnRopePosition();

                MountRope();

                // Check if move rope script is being used
                if (transform.parent.TryGetComponent<MoveRope>(out MoveRope moveRope))
                {
                    if (moveRope.enabled == true)

                        // Nullify the player mass so the rope doesn't stop moving
                        collision.gameObject.GetComponent<Rigidbody2D>().mass = 0;
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !player.alreadyConnected)
        {
            player.StateMachine.ChangeState(player.RopeState);

            // Change the player's layer to rope to avoid physical collisions with the rope
            collision.gameObject.layer = gameObject.layer;

            // Call the on trigger function manually instead of repeating the same code lines for efficiency purposes
            OnTriggerEnter2D(collision.collider);
        }
    }
    #endregion

    #region Custom Methods

    /// <summary>
    /// Assigns the player position on the rope based on which rope segment the player collided with
    /// </summary>
    void AssignPlayerOnRopePosition()
    {
        if (lastRope.climbable)
        {
            // Reset the value of closest distance to infinity
            closestDistance = Mathf.Infinity;

            // Check if always mount to lower position option is on
            if (lastRope.alwaysMountToLowerPosition)
            {
                // Set a default unreachable value for the index
                transformIndexToAttachToInEachSegment = transform.childCount;

                for (int i = 0; i < numberOfTransformsPerSegment; i++)
                {
                    // Check if player contact point is above rope attach position
                    if (contactPoint.y >= transform.GetChild(i).position.y + lastRope.playerY_AxisOffset)
                    {
                        // Calculate the distance between the player contact point and the rope attach positions
                        distance = contactPoint.y - transform.GetChild(i).position.y + lastRope.playerY_AxisOffset;

                        // Compare the distance with the closest distance previously calculated (or infinity if none)
                        if (distance <= closestDistance)
                        {
                            // Set the index to the closest rope attach position to the player's contact point
                            transformIndexToAttachToInEachSegment = i;

                            // Change the closest distance to equal the new distance for later comparisons
                            closestDistance = distance;
                        }
                    }
                }

                // Check if the unreachable value was changed or not in order to know if there was an attach position below the character or not
                if (transformIndexToAttachToInEachSegment == transform.childCount &&
                    ((lastRope.hideLastRope && transform.GetSiblingIndex() == transform.parent.childCount - 2) || transform.GetSiblingIndex() == transform.parent.childCount - 1))
                {
                    // Set the index to the lowest attach position of the rope segment if this was the last visible segment
                    transformIndexToAttachToInEachSegment = transform.childCount - 1;
                }
            }
            else
            {
                for (int i = 0; i < numberOfTransformsPerSegment; i++)
                {
                    // Calculate the distance between the player contact point and the rope attach positions
                    distance = Mathf.Abs(contactPoint.y - transform.GetChild(i).position.y + lastRope.playerY_AxisOffset);

                    // Compare the distance with the closest distance previously calculated (or infinity if none)
                    if (distance <= closestDistance)
                    {
                        // Set the index to the closest rope attach position to the player's contact point
                        transformIndexToAttachToInEachSegment = i;

                        // Change the closest distance to equal the new distance for later comparisons
                        closestDistance = distance;
                    }
                }
            }

            if (transformIndexToAttachToInEachSegment != transform.childCount)
            {
                // Get which rope segment the player collided with
                lastRope.positionNumber = numberOfTransformsPerSegment * transform.GetSiblingIndex() + transformIndexToAttachToInEachSegment;
            }
        }

        else
        {
            // If rope is not climbable then player-rope position is the last rope segment
            lastRope.positionNumber = numberOfTransformsPerSegment * transform.parent.childCount - 1;
        }
    }

    /// <summary>
    /// Attach the player to the rope
    /// </summary>
    void MountRope()
    {
        // Enable the hinge component, connected boolean, and first connection boolean in the last rope script
        lastRope.connected = true;
        lastRope.firstConnection = true;
        transform.parent.GetComponentsInChildren<HingeJoint2D>()[transform.parent.childCount].enabled = true;
    }

    /// <summary>
    /// Checks if move rope script is being used
    /// </summary>
    void CheckForMovingRope()
    {
        // Check if autonomous moving rope script is being used
        if (transform.parent.TryGetComponent<MoveRope>(out MoveRope moveRope))
        {
            if (moveRope.enabled == true)
            {
                // Change the rope links' mass so that the rope actually moves automatically
                rb2d.mass = 1;
                rb2d.gravityScale = 0;
                if (!movingRope)
                {
                    movingRope = true;
                    // Reset the rope position and rotation to avoid glitches if move rope was activated during during runtime
                    transform.rotation = Quaternion.identity;
                    transform.position = originalPosition;
                }
            }
            else
            {
                movingRope = false;
                // Reset the rope's mass if you no longer want the rope to move autonomously
                rb2d.mass = ropeLinksMass;
                rb2d.gravityScale = ropeGravityScale;
            }
        }
    }

    /// <summary>
    /// Reads all the rope settings from the create rope or last rope scripts
    /// </summary>
    void AdjustRopeSettings()
    {
        // Read all the values from the create rope script if it's being used
        if (transform.parent.parent.TryGetComponent<CreateRope>(out CreateRope createRope))
        {
            optimizations = createRope.optimizations;
            ropeLinksMass = createRope.ropeLinksMass;
            ropeGravityScale = createRope.ropeLinksGravityScale;
            enableRopeCollisions = createRope.enableRopeCollisions;
            staticRope = createRope.staticRope;
        }

        else
        {
            // Read all the values from the fixed rope script
            FixedRope fixedRope = transform.parent.parent.GetComponent<FixedRope>();
            optimizations = fixedRope.optimizations;
            ropeLinksMass = fixedRope.ropeLinksMass;
            ropeGravityScale = fixedRope.ropeLinksGravityScale;
            enableRopeCollisions = fixedRope.enableRopeCollisions;
            staticRope = fixedRope.staticRope;
        }

        if (enableRopeCollisions)
        {
            // Set the the layer of the segment to Rope
            gameObject.layer = LayerMask.NameToLayer("Rope");

            // Enable rope collision with other game objects and disable the rope collision with itself to make it swing freely
            Physics2D.IgnoreLayerCollision(gameObject.layer, gameObject.layer);
            GetComponent<CapsuleCollider2D>().isTrigger = false;
        }
        
        if(staticRope)
        {
            // Change the rigid body type to static to prevent it from swinging
            rb2d.bodyType = RigidbodyType2D.Static;
        }
    }
    #endregion
}
