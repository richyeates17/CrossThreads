using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private float ropeLength = 10f;
    [SerializeField]
    public float lowerNodeDistance = 0.5f;
    [SerializeField]
    private bool isFixedRope = true;
    [SerializeField]
    private PlayerData playerData;

    public int numberOfNodes = 5;

    public float playerPositionOnRope = 5f;
    public float playerPositionOnRopeOld;
    public GameObject ropePrefab;
    public GameObject nodePrefab;
    public GameObject playerPositionNode;

    public bool isRopeSpooling;
    private float totalRopeLengthLive;
    private LineRenderer lineRenderer;
    private DistanceJoint2D playerDistanceJoint;

    private List<GameObject> theRopeNodes;
    private List<Vector3> theRopeNodesPositions;


    // Start is called before the first frame update
    void Start()
    {
        ropePrefab = this.gameObject;

        player = GameObject.FindGameObjectWithTag("Player");

        Vector2 initialRopeConnectPoint = new Vector2(this.transform.position.x, this.transform.position.y - playerPositionOnRope);
        //Vector2 totalRopeDistance = new Vector2(this.transform.position.x, this.transform.position.y - ropeLength);
        playerPositionNode = Instantiate(nodePrefab, initialRopeConnectPoint, Quaternion.identity);
        playerPositionNode.layer = playerData.ropeLayerInt;
        playerPositionNode.transform.parent = this.transform;
        //playerPositionNode.GetComponent<DistanceJoint2D>().enabled = false;
        
        playerDistanceJoint = this.GetComponent<DistanceJoint2D>();
        playerDistanceJoint.distance = playerPositionOnRope;
        playerDistanceJoint.connectedBody = playerPositionNode.GetComponent<Rigidbody2D>();

        lowerNodeDistance = (ropeLength - playerPositionOnRope) / numberOfNodes;

        DistanceJoint2D nodeDistanceJoint2D = playerPositionNode.GetComponent<DistanceJoint2D>();
        Vector2 nodeConnectionPoint = new Vector2(initialRopeConnectPoint.x, initialRopeConnectPoint.y - lowerNodeDistance); ;
        GameObject previousNode;

        theRopeNodes = new List<GameObject>();
        theRopeNodes.Add(playerPositionNode);

        for (int i = 1; i <= numberOfNodes-1; i++)
        {
            GameObject aNode = Instantiate(nodePrefab, nodeConnectionPoint, Quaternion.identity);
            aNode.layer = playerData.ropeLayerInt;
            aNode.transform.parent = this.transform;

            nodeDistanceJoint2D.distance = lowerNodeDistance;
            nodeDistanceJoint2D.connectedBody = aNode.GetComponent<Rigidbody2D>();
            aNode.GetComponent<Rigidbody2D>().mass = 0.1f;

            previousNode = aNode;
            nodeDistanceJoint2D = aNode.GetComponent<DistanceJoint2D>();
            nodeConnectionPoint = new Vector2(aNode.transform.position.x, aNode.transform.position.y - lowerNodeDistance);

            theRopeNodes.Add(aNode);

            if (i >= numberOfNodes)
            {
                aNode.GetComponent<DistanceJoint2D>().enabled = false;
            } 
            
        }
       
   
        isRopeSpooling = true;

        theRopeNodesPositions = new List<Vector3>();
        theRopeNodesPositions.Add(this.transform.position);


        for (int i=1; i<theRopeNodes.Count; i++)
        {
            theRopeNodesPositions.Add(theRopeNodes[i].transform.position);
        }

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        Debug.Log(theRopeNodes.Count);
        lineRenderer.SetPositions(theRopeNodesPositions.ToArray());
    }

    // Update is called once per frame
    void Update()
    {
        

        if (isFixedRope || isRopeSpooling)
        {
            
        }

        if (playerPositionOnRope != playerPositionOnRopeOld)
        {
            CreateNode();
        }
        else
        {
            isRopeSpooling = false;
        }

      //  RenderTheLine();
    }

    private void CreateNode()
    {

    }

    private void RenderTheLine()
    {
        theRopeNodesPositions.Clear();
        theRopeNodesPositions.Add(this.transform.position);
        for (int i = 1; i < theRopeNodes.Count; i++)
        {
            theRopeNodesPositions.Add(theRopeNodes[i].transform.position);
        }

        lineRenderer.SetPositions(theRopeNodesPositions.ToArray());
    }

}


