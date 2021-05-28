using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField]
    private Player player;
    public int numberOfLinks = 10;
    //[SerializeField]
    //private bool isFixedRope = true;
    [SerializeField]
    private PlayerData playerData;

    [SerializeField]
    private float ropeUpperMass = 0.3f;
    [SerializeField]
    private float ropeLowerMass = 0.01f;
    [SerializeField]
    private float ropeUpperMaxAngle = 5f;
    [SerializeField]
    private float ropeLowerMaxAngle = 60f;

    public int integerOfTheLinkThePlayerIsCurrentlyOn;
    public int integerOfTheLinkThePlayerIsMovingTo;

    [SerializeField]
    private GameObject hook;
    public GameObject ropePrefab;
    public GameObject linkPrefab;

    private LineRenderer lineRenderer;

    private List<GameObject> theRopeNodes;
    private List<Vector3> theRopeNodesPositions;

    // Start is called before the first frame update
    void Start()
    {
        ropePrefab = this.gameObject;

        theRopeNodes = new List<GameObject>();
        theRopeNodesPositions = new List<Vector3>();

        CreateRope();
       
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;

        RenderTheLine();

        integerOfTheLinkThePlayerIsCurrentlyOn = (int)Mathf.Round(numberOfLinks / 2);
        ControlTheLinkRotationBasedOnConnectedLink();
    }

    // Update is called once per frame
    void Update()
    {
        ControlTheLinkRotationBasedOnConnectedLink();
        RenderTheLine();
    }

    private void CreateRope()
    {

        Rigidbody2D previousLinkRB = hook.GetComponent<Rigidbody2D>();

        for (int i = 0; i < numberOfLinks; i++)
        {
            GameObject aLink = Instantiate(linkPrefab, transform);
            HingeJoint2D joint = aLink.GetComponent<HingeJoint2D>();
            joint.connectedBody = previousLinkRB;
            previousLinkRB = aLink.GetComponent<Rigidbody2D>();

            aLink.layer = playerData.ropeLayerInt;
            aLink.transform.parent = this.transform;

            theRopeNodes.Insert(i,aLink);

            if (i >= numberOfLinks)
            {
                aLink.GetComponent<HingeJoint2D>().enabled = false;
            }
        }
    }

    private void RenderTheLine()
    {
        theRopeNodesPositions.Clear();
        theRopeNodesPositions.Add(hook.transform.position);
        for (int i = 1; i < theRopeNodes.Count; i++)
        {
            theRopeNodesPositions.Add(theRopeNodes[i].transform.position);
        }

        lineRenderer.positionCount = theRopeNodesPositions.Count;
        lineRenderer.SetPositions(theRopeNodesPositions.ToArray());

    }

    public void AddLink()
    {
        GameObject aLink = Instantiate(linkPrefab, transform);
        aLink.layer = playerData.ropeLayerInt;
        aLink.transform.parent = this.transform;

        theRopeNodes.Insert(0, aLink);
    
        HingeJoint2D joint = aLink.GetComponent<HingeJoint2D>();
        joint.connectedBody = hook.GetComponent<Rigidbody2D>();

        HingeJoint2D movedLinkHJ = theRopeNodes[1].GetComponent<HingeJoint2D>();
        movedLinkHJ.connectedBody = aLink.GetComponent<Rigidbody2D>();
        numberOfLinks++;
    }

    public void RemoveLink()
    {
        GameObject removeLink = theRopeNodes[0];
        GameObject newTopLink = theRopeNodes[1];
         
        theRopeNodes.RemoveAt(0);

        HingeJoint2D joint = newTopLink.GetComponent<HingeJoint2D>();
        joint.connectedBody = hook.GetComponent<Rigidbody2D>();

        Destroy(removeLink);

        numberOfLinks--;
    }

    public void ControlTheLinkRotationBasedOnConnectedLink()
    {
        for (int i=0; i<theRopeNodes.Count; i++)
        {
            if (theRopeNodes[i].tag == "MoveToLink")
            {
                integerOfTheLinkThePlayerIsMovingTo = i;
            }
            if (theRopeNodes[i].tag == "CurrentLink")
            {
                integerOfTheLinkThePlayerIsCurrentlyOn = i;
            }
        }

        if (integerOfTheLinkThePlayerIsMovingTo != integerOfTheLinkThePlayerIsCurrentlyOn)
        {

            theRopeNodes[integerOfTheLinkThePlayerIsCurrentlyOn].tag = "Untagged";
            theRopeNodes[integerOfTheLinkThePlayerIsMovingTo].tag = "CurrentLink";
            integerOfTheLinkThePlayerIsCurrentlyOn = integerOfTheLinkThePlayerIsMovingTo;

            HingeJoint2D hj;
            JointAngleLimits2D limits;
            Rigidbody2D rb;

            GameObject[] links = theRopeNodes.ToArray();

            for (int i = 1; i < links.Length; i++)
            {
                if(i<integerOfTheLinkThePlayerIsCurrentlyOn)
                {
                    hj = links[i].GetComponent<HingeJoint2D>();
                    limits = hj.limits;
                    limits.max = -ropeLowerMaxAngle;
                    limits.min = ropeLowerMaxAngle;
                    hj.limits = limits;

                    rb = links[i].GetComponent<Rigidbody2D>();
                    rb.mass = ropeUpperMass;
                } 
                else
                {
                    hj = links[i].GetComponent<HingeJoint2D>();
                    limits = hj.limits;
                    limits.max = -ropeUpperMaxAngle;
                    limits.min = ropeUpperMaxAngle;
                    hj.limits = limits;

                    rb = links[i].GetComponent<Rigidbody2D>();
                    rb.mass = ropeLowerMass;
                }

            }
        }
    }

}


