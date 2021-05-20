using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private int numberOfLinks = 10;
    [SerializeField]
    private bool isFixedRope = true;
    [SerializeField]
    private PlayerData playerData;

    public int playerPositionOnRope;
    public int playerPositionOnRopeOld;

    [SerializeField]
    private GameObject hook;
    public GameObject ropePrefab;
    public GameObject linkPrefab;

    public GameObject playerPositionNode;

    public bool isRopeSpooling;
    private float totalRopeLengthLive;
    private LineRenderer lineRenderer;

    private List<GameObject> theRopeNodes;
    private List<Vector3> theRopeNodesPositions;


    // Start is called before the first frame update
    void Start()
    {
        ropePrefab = this.gameObject;

        player = GameObject.FindGameObjectWithTag("Player");

        //Vector2 initialRopeConnectPoint = new Vector2(this.transform.position.x, this.transform.position.y - playerPositionOnRope);
        //Vector2 totalRopeDistance = new Vector2(this.transform.position.x, this.transform.position.y - ropeLength);
        //playerPositionNode = Instantiate(nodePrefab, initialRopeConnectPoint, Quaternion.identity);
        //playerPositionNode.layer = playerData.ropeLayerInt;
        //playerPositionNode.transform.parent = this.transform;
        //playerPositionNode.GetComponent<DistanceJoint2D>().enabled = false;

        //playerDistanceJoint.distance = playerPositionOnRope;
        //playerDistanceJoint.connectedBody = playerPositionNode.GetComponent<Rigidbody2D>();

        // DistanceJoint2D nodeDistanceJoint2D = playerPositionNode.GetComponent<DistanceJoint2D>();
        //  Vector2 nodeConnectionPoint = new Vector2(initialRopeConnectPoint.x, initialRopeConnectPoint.y - lowerNodeDistance); ;

        theRopeNodes = new List<GameObject>();
        theRopeNodesPositions = new List<Vector3>();

        CreateRope();
       
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;

        RenderTheLine();
    }

    // Update is called once per frame
    void Update()
    {
        

        if (isFixedRope || isRopeSpooling)
        {
            
        }

        if (playerPositionOnRope != playerPositionOnRopeOld)
        {
           // CreateNode();
        }
        else
        {
            isRopeSpooling = false;
        }

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

            theRopeNodes.Add(aLink);

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

}


