using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : MonoBehaviour
{
    private Pathfinder pathfinder;
    private Vector3Int currentNodeId;
    private Vector3Int? targetNodeId;
    private List<PathNode> pathNodes;
    private Vector3 movementTargetPos;

    [Header("Unit Config")]
    public float movementSpd = 5F;

    [Header("Pathfinding Data")]
    public string currentNode;
    public string targetNode;
    public float pathSize;
    public float operations;
    public bool inMovement = false;

    // Start is called before the first frame update
    void Awake()
    {
        pathfinder = new Pathfinder();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        currentNodeId = (Vector3Int)GridGenerator.Singleton.GridNodeIdFromWorldPosition(transform.position);
        currentNode = currentNodeId.ToString();

        if (targetNodeId == null) targetNode = "No target";
        else targetNode = targetNodeId.ToString();
    }

    public bool CommandMove(Vector3 targetPos)
    {
        inMovement = false;
        targetNodeId = GridGenerator.Singleton.GridNodeIdFromWorldPosition(targetPos);
        if (targetNodeId == null)
        {
            pathNodes = null;
        }
        else
        {
            pathNodes = new List<PathNode>();
            pathfinder.FindPath(transform.position, targetPos, out pathNodes, out pathSize, out operations);
            inMovement = true;
        }
        return inMovement;
    }

    private void Movement()
    {
        if (!inMovement) return;
        if (pathNodes.Count == 0)
        {
            inMovement = false;
            return;
        }

        Vector3 currentPos = transform.position;
        movementTargetPos = pathNodes[0].gridNode.transform.position + GridGenerator.Singleton.nodeStart;
        if (currentPos != movementTargetPos)
        {
            Vector3 angles = Quaternion.FromToRotation(currentPos, movementTargetPos).eulerAngles;
            Vector3 moveDir = (movementTargetPos - currentPos).normalized;
            Debug.Log(angles + " | " + moveDir);
            float distance = Vector3.Distance(currentPos, movementTargetPos);

            Vector3 rotatedMovementTargetPos = Quaternion.Euler(angles.x, angles.y, 0) * moveDir;
            rotatedMovementTargetPos = rotatedMovementTargetPos.normalized * movementSpd;
            Vector3 deltaTimeMovementTargetPos = rotatedMovementTargetPos * Time.deltaTime;
            deltaTimeMovementTargetPos = Vector3.ClampMagnitude(deltaTimeMovementTargetPos, distance);
            transform.Translate(deltaTimeMovementTargetPos, Space.World);
        }
        else
        {
            pathNodes.RemoveAt(0);
        }
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
