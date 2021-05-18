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
    public float lowerNodeDistance = 2f;
    [SerializeField]
    private bool isFixedRope = true;
    [SerializeField]
    private PlayerData playerData;

    public float playerPositionOnRope = 5f;
    public float playerPositionOnRopeOld;
    public GameObject ropePrefab;
    public GameObject nodePrefab;
    public GameObject playerPositionNode;
    public GameObject lastNode;
    public GameObject finalNode;

    public bool isRopeSpooling;
    private float totalRopeLengthLive;
    private LineRenderer lineRenderer;
    private DistanceJoint2D playerDistanceJoint;

    private List<Vector3> theRopeNodes;


    // Start is called before the first frame update
    void Start()
    {
        ropePrefab = this.gameObject;

        player = GameObject.FindGameObjectWithTag("Player");

        Vector2 initialRopeConnectPoint = new Vector2(this.transform.position.x, this.transform.position.y - playerPositionOnRope);
        Vector2 totalRopeDistance = new Vector2(this.transform.position.x, this.transform.position.y - ropeLength);
        playerPositionNode = Instantiate(nodePrefab, initialRopeConnectPoint, Quaternion.identity);
        playerPositionNode.layer = playerData.ropeLayerInt;
        playerPositionNode.transform.parent = this.transform;
        playerPositionNode.GetComponent<DistanceJoint2D>().enabled = false;
        
        playerDistanceJoint = this.GetComponent<DistanceJoint2D>();
        playerDistanceJoint.distance = playerPositionOnRope;
        playerDistanceJoint.connectedBody = playerPositionNode.GetComponent<Rigidbody2D>();
       
   
        isRopeSpooling = true;

        theRopeNodes = new List<Vector3>();
        theRopeNodes.Add(this.transform.position);
        theRopeNodes.Add(playerPositionNode.transform.position);
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

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

        RenderTheLine();
    }

    private void CreateNode()
    {

    }

    private void RenderTheLine()
    {
        theRopeNodes.Clear();
        theRopeNodes.Add(this.transform.position);
        theRopeNodes.Add(playerPositionNode.transform.position);
        lineRenderer.SetPositions(theRopeNodes.ToArray());
    }

}


