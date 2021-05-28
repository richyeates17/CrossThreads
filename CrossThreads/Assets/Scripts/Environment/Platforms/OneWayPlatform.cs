using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    private PlatformEffector2D effector;

    private float waitTime;
    private float resetTime = 0.5f;
    private bool resetDoIt;

    [SerializeField]
    Player player;

    private bool isPlayerOnMe;

    // Start is called before the first frame update
    void Start()
    {
        effector = GetComponent<PlatformEffector2D>();
        isPlayerOnMe = false;
    }

    // Update is called once per frame
    void Update()
    {
                if (player.InputHandler.NormInputY >= 0)
                {
                    waitTime = 0.5f;
                }

                if (player.InputHandler.NormInputY == -1 && isPlayerOnMe)
                {
                    if (waitTime <= 0)
                    {
                        effector.rotationalOffset = 180f;
                        waitTime = 0.5f;
                        resetDoIt = true;
                    }
                    else
                    {
                        waitTime -= Time.deltaTime;
                    }
                }

                if (resetDoIt)
                {
                    resetTime -= Time.deltaTime;
                    if (resetTime <= 0)
                    {
                        effector.rotationalOffset = 0f;
                        resetTime = 0.5f;
                        resetDoIt = false;
                    }
                }
       
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            player.transform.parent = transform;
            isPlayerOnMe = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            player.transform.parent = null;
            isPlayerOnMe = false;
        }
    }
}
