using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crank : MonoBehaviour
{
    private Rope rope;
    public int numLinks;
    public int maxLinks = 15;
    public int minLinks = 5;
    public Player player;

    private void Awake()
    {
        rope = transform.parent.GetComponent<Rope>();
        numLinks = rope.numberOfLinks;

    }

    //Testing

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && numLinks > minLinks && player.InputHandler.NormInputY == 1)
        {
            rope.RemoveLink();
            numLinks = rope.numberOfLinks;
        }

        if (collision.gameObject.tag == "Player" && numLinks < maxLinks && player.InputHandler.NormInputY == -1)
        {
            rope.AddLink();
            numLinks = rope.numberOfLinks;
        }

    }
}
