using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class GridGeneratorGizmos : MonoBehaviour
{
    private GridGenerator gridGenerator;
    private bool[,,] blockedNodes;

    public LayerMask obstaclesLayer;
    public Vector3 screenToGridPos;

    public bool drawIfSelected = true;
    public bool drawAroundMouseOnly = true;
    public int currentFloor = 0;
    public Vector3Int nodesAroundMouse = new Vector3Int(4, 0, 4);
    public bool drawGridGeneratorGizmos = true;
    public Color gridSize = Color.white;
    public bool drawGridNodeGizmos = true;
    public Color passableNode = Color.cyan;
    public Color blockedNode = Color.red;
    public bool drawConnectionGizmos = true;
    public Color connection = Color.blue;

    void Awake()
    {
        UpdateData();
    }

    void Update()
    {
        UpdateData();
    }

    void OnDrawGizmos()
    {
        if (!drawIfSelected) DrawGizmos();
    }

    void OnDrawGizmosSelected()
    {
        if (drawIfSelected) DrawGizmos();
    }
    public Vector3 ScreenToGridPoint(Camera cam, int floor)
    {
        // create a plane whose normal points to +Y:
        Plane hPlane = new Plane(Vector3.up, new Vector3(0, floor * gridGenerator.nodeSize.y, 0));

        Vector3 position = cam.ScreenToWorldPoint(Input.mousePosition);
        position = Event.current.mousePosition;

        // Plane.Raycast stores the distance from ray.origin to the hit point in this variable:
        Ray ray = HandleUtility.GUIPointToWorldRay(position);

        // if the ray hits the plane...
        if (hPlane.Raycast(ray, out float distance))
        {
            // get the hit point:
            screenToGridPos = ray.GetPoint(distance);
        }
        return screenToGridPos;
    }

    private void UpdateData()
    {
        gridGenerator = GetComponent<GridGenerator>();
        if (!gridGenerator) throw new MissingComponentException("Missing component: GridGenerator");

        Vector3Int gridSize = gridGenerator.gridSize;
        blockedNodes = new bool[gridSize.x, gridSize.y, gridSize.z];

        obstaclesLayer = LayerMask.GetMask("Obstacles");
    }

    private void DrawGizmos()
    {
        if (drawGridGeneratorGizmos)
        {
            DrawGridGeneratorGizmos();
        }

        if (drawGridNodeGizmos || drawConnectionGizmos)
        {
            if (Camera.current == null) return;

            for (int floor = 0; floor < gridGenerator.gridSize.y; floor++)
            {
                for (int row = 0; row < gridGenerator.gridSize.z; row++)
                {
                    for (int col = 0; col < gridGenerator.gridSize.x; col++)
                    {
                        Vector3 pos = Vector3.Scale(new Vector3(col, floor, row), gridGenerator.nodeSize) + gridGenerator.gridStart;
                        Vector3 size = gridGenerator.nodeSize;
                        Collider[] colliders = Physics.OverlapBox(pos, size / 2.001F, Quaternion.identity, obstaclesLayer);
                        blockedNodes[col, floor, row] = (colliders.Length > 0);

                        bool canDrawNode = true;
                        if (drawAroundMouseOnly)
                        {
                            Camera cam = FindObjectOfType<Camera>();
                            Vector3? worldPos = gridGenerator.ScreenToGridPoint(cam, currentFloor);
                            if (worldPos == null) return;

                            Vector3 gridPos = gridGenerator.ClampWorldPosToGrid((Vector3)worldPos, Vector3.zero);
                            Vector3Int centralNodeId = (Vector3Int)gridGenerator.GridNodeIdFromWorldPosition(gridPos);

                            if (Mathf.Abs(centralNodeId.x - col) > nodesAroundMouse.x) canDrawNode = false;
                            if (Mathf.Abs(centralNodeId.y - floor) > nodesAroundMouse.y) canDrawNode = false;
                            if (Mathf.Abs(centralNodeId.z - row) > nodesAroundMouse.z) canDrawNode = false;
                        }
                        if (canDrawNode)
                        {
                            if (drawGridNodeGizmos) DrawGridNodeGizmos(col, floor, row, pos, size);
                            if (drawConnectionGizmos) DrawConnectionGizmos(col, floor, row);
                        }
                    }
                }
            }
        }
    }

    private void DrawGridGeneratorGizmos()
    {
        Vector3 size = Vector3.Scale(gridGenerator.gridSize, gridGenerator.nodeSize);
        Vector3 center = gridGenerator.gridStart + (size / 2F) + gridGenerator.nodeStart;
        Gizmos.color = gridSize;
        Gizmos.DrawWireCube(center, size);
    }

    private void DrawGridNodeGizmos(int col, int floor, int row, Vector3 pos, Vector3 size)
    {
        Color color = blockedNodes[col, floor, row] ? blockedNode : passableNode;
        size.y = .001F;

        Gizmos.color = color;
        Gizmos.DrawWireCube(pos, size);

        color.a /= 4F;
        Gizmos.color = color;
        Gizmos.DrawCube(pos, size);
    }

    private void DrawConnectionGizmos(int col, int floor, int row)
    {
        if (blockedNodes[col, floor, row]) return;

        Gizmos.color = connection;
        Vector3 startPos = Vector3.Scale(new Vector3(col, floor, row), gridGenerator.nodeSize);
        int x, y, z;
        if (col > 0)
        {
            x = col - 1;
            y = floor;
            z = row;
            DrawConnection(startPos, x, y, z);
        }
        if (row > 0)
        {
            x = col;
            y = floor;
            z = row - 1;
            DrawConnection(startPos, x, y, z);
        }
        if (col > 0 && row > 0)
        {
            x = col - 1;
            y = floor;
            z = row - 1;
            if (!blockedNodes[col, y, z] && !blockedNodes[x, y, row])
                DrawConnection(startPos, x, y, z);
        }
        if (col < gridGenerator.gridSize.x - 1 && row > 0)
        {
            x = col + 1;
            y = floor;
            z = row - 1;
            if (!blockedNodes[col, y, z] && !blockedNodes[x, y, row])
                DrawConnection(startPos, x, y, z);
        }
    }

    private void DrawConnection(Vector3 startPos, int x, int y, int z)
    {
        if (!blockedNodes[x, y, z])
        {
            Vector3 targetPos = Vector3.Scale(new Vector3(x, y, z), gridGenerator.nodeSize);
            Gizmos.DrawLine(startPos + gridGenerator.gridStart, targetPos + gridGenerator.gridStart);
        }
    }
}
