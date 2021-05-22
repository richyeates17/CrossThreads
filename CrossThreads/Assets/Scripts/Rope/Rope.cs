using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    public int numberOfLinks = 10;
    //[SerializeField]
    //private bool isFixedRope = true;
    [SerializeField]
    private PlayerData playerData;

    public int playerPositionOnRope;
    public int playerPositionOnRopeOld;

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

        player = GameObject.FindGameObjectWithTag("Player");

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

    public void AddLink()
    {
        GameObject aLink = Instantiate(linkPrefab, transform);
        aLink.layer = playerData.ropeLayerInt;
        aLink.transform.parent = this.transform;

        theRopeNodes.Insert(1, aLink);

        HingeJoint2D joint = aLink.GetComponent<HingeJoint2D>();
        joint.connectedBody = hook.GetComponent<Rigidbody2D>();

        HingeJoint2D movedLinkHJ = theRopeNodes[2].GetComponent<HingeJoint2D>();
        movedLinkHJ.connectedBody = aLink.GetComponent<Rigidbody2D>();
        numberOfLinks++;
    }

    public void RemoveLink()
    {
        GameObject removeLink = theRopeNodes[1];
        GameObject newTopLink = theRopeNodes[2];
         
        theRopeNodes.RemoveAt(1);

        HingeJoint2D joint = newTopLink.GetComponent<HingeJoint2D>();
        joint.connectedBody = hook.GetComponent<Rigidbody2D>();

        Destroy(removeLink);

        numberOfLinks--;
    }

}


