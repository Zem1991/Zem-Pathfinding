using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : MonoBehaviour
{
    private Pathfinder pathfinder;
    private Vector3Int? currentNodeId;
    private Vector3Int? targetNodeId;

    [Header("Unit Config")]
    public float movementSpd = 5F;

    [Header("Pathfinding Data")]
    public string currentNode;
    public string targetNode;
    public List<PathNode> pathNodes;
    public float pathSize;
    public float operations;
    public bool inMovement = false;
    //public bool isNextMoveDone = false;//
    public Vector3 movementTargetPos;
    public Vector3 movementTargetDir;
    public Vector3 movementVelocity;

    // Start is called before the first frame update
    void Awake()
    {
        pathfinder = new Pathfinder();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        InspectorData();
    }

    public bool CommandMove(Vector3 targetPos)
    {
        inMovement = false;
        targetNodeId = GridGenerator.Singleton.GridNodeIdFromWorldPosition(targetPos);
        if (targetNodeId != null)
        {
            pathfinder.FindPath(transform.position, targetPos, out pathNodes, out pathSize, out operations);
            inMovement = true;
            NextMove();
        }
        return inMovement;
    }

    private void Movement()
    {
        if (!inMovement) return;

        Vector3 frameVelocity = movementVelocity * Time.deltaTime;
        float distance = Vector3.Distance(transform.position, movementTargetPos);
        if (frameVelocity.magnitude > distance) frameVelocity = Vector3.ClampMagnitude(frameVelocity, distance);
        transform.Translate(frameVelocity, Space.World);

        if (transform.position == movementTargetPos)
        {
            //Doing this may seem redundant, but it actually fixes some floating point issues that can cause movement overshooting
            //Moving to the bottom or left edge of the grid without this fix may cause the Unit to be read as over a tile with coordinate equal to -1
            transform.position = movementTargetPos;

            NextMove();
        }
    }

    private void NextMove()
    {
        if (pathNodes.Count == 0)
        {
            inMovement = false;
            return;
        }

        Vector3 currentPos = transform.position;
        movementTargetPos = pathNodes[0].gridNode.transform.position + GridGenerator.Singleton.nodeStart;
        movementTargetDir = (movementTargetPos - currentPos).normalized;
        movementVelocity = movementTargetDir * movementSpd;
        pathNodes.RemoveAt(0);
    }

    private void InspectorData()
    {
        currentNodeId = GridGenerator.Singleton.GridNodeIdFromWorldPosition(transform.position);
        if (currentNodeId == null) currentNode = "No grid node";
        else currentNode = currentNodeId.ToString();

        if (targetNodeId == null) targetNode = "No grid node";
        else targetNode = targetNodeId.ToString();
    }

    private void OnDrawGizmos()
    {
        if (pathNodes != null)
        {
            Gizmos.color = Color.magenta;
            foreach (PathNode item in pathNodes)
            {
                Gizmos.DrawSphere(item.gridNode.transform.position, .5F);
            }
        }
    }
}
