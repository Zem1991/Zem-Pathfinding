using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class GridGeneratorGizmos : MonoBehaviour
{
    private GridGenerator gridGenerator;
    private bool[,,] blockedNodes;

    public bool drawIfSelected = true;
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
        BakePreviewGrid();
    }

    void Update()
    {
        UpdateData();
    }

    void OnDrawGizmos()
    {
        if (!drawIfSelected && enabled) DrawGizmos();
    }

    void OnDrawGizmosSelected()
    {
        if (drawIfSelected && enabled) DrawGizmos();
    }

    public void BakePreviewGrid()
    {
        Vector3Int gridSize = gridGenerator.gridSize;
        blockedNodes = new bool[gridSize.x, gridSize.y, gridSize.z];
    }

    private void UpdateData()
    {
        gridGenerator = GetComponent<GridGenerator>();
        if (!gridGenerator) throw new MissingComponentException("Missing component: GridGenerator");
    }

    private void DrawGizmos()
    {
        if (drawGridGeneratorGizmos)
        {
            DrawGridGeneratorGizmos();
        }

        if (drawGridNodeGizmos || drawConnectionGizmos)
        {
            if (blockedNodes == null) return;

            Camera cam = FindObjectOfType<Camera>();
            Vector3? worldPos = ScreenToWorldPoint(cam);
            if (worldPos == null) return;

            Vector3Int? nodeIdAttempt = gridGenerator.GridNodeIdFromWorldPosition((Vector3)worldPos);
            if (nodeIdAttempt == null) return;
            Vector3Int nodeId = (Vector3Int)nodeIdAttempt;

            int minX = Mathf.Max(0, nodeId.x - nodesAroundMouse.x);
            int maxX = Mathf.Min(gridGenerator.gridSize.x, nodeId.x + nodesAroundMouse.x);
            int minY = Mathf.Max(0, nodeId.y - nodesAroundMouse.y);
            int maxY = Mathf.Min(gridGenerator.gridSize.y, nodeId.y + nodesAroundMouse.y);
            int minZ = Mathf.Max(0, nodeId.z - nodesAroundMouse.z);
            int maxZ = Mathf.Min(gridGenerator.gridSize.z, nodeId.z + nodesAroundMouse.z);

            for (int floor = minY; floor <= maxY; floor++)
            {
                for (int row = minZ; row <= maxZ; row++)
                {
                    for (int col = minX; col <= maxX; col++)
                    {
                        Vector3 pos = Vector3.Scale(new Vector3(col, floor, row), gridGenerator.nodeSize) + gridGenerator.gridStart;
                        Vector3 size = gridGenerator.nodeSize;
                        Collider[] colliders = Physics.OverlapBox(pos, size / 2.001F, Quaternion.identity, gridGenerator.layerObstacles);
                        blockedNodes[col, floor, row] = (colliders.Length > 0);

                        if (drawGridNodeGizmos) DrawGridNodeGizmos(col, floor, row, pos, size);
                        if (drawConnectionGizmos) DrawConnectionGizmos(col, floor, row);
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
        int x, z;

        if (col > 0)
        {
            x = col - 1;
            z = row;
            DrawConnectionGizmosAux(startPos, x, floor, z);

            if (row > 0)
            {
                x = col - 1;
                z = row - 1;
                if (!blockedNodes[col, floor, z] && !blockedNodes[x, floor, row])
                    DrawConnectionGizmosAux(startPos, x, floor, z);
            }

            if (row < gridGenerator.gridSize.z - 1)
            {
                x = col - 1;
                z = row + 1;
                if (!blockedNodes[col, floor, z] && !blockedNodes[x, floor, row])
                    DrawConnectionGizmosAux(startPos, x, floor, z);
            }
        }

        if (col < gridGenerator.gridSize.x - 1)
        {
            x = col + 1;
            z = row;
            DrawConnectionGizmosAux(startPos, x, floor, z);

            if (row > 0)
            {
                x = col + 1;
                z = row - 1;
                if (!blockedNodes[col, floor, z] && !blockedNodes[x, floor, row])
                    DrawConnectionGizmosAux(startPos, x, floor, z);
            }

            if (row < gridGenerator.gridSize.z - 1)
            {
                x = col + 1;
                z = row + 1;
                if (!blockedNodes[col, floor, z] && !blockedNodes[x, floor, row])
                    DrawConnectionGizmosAux(startPos, x, floor, z);
            }
        }

        if (row > 0)
        {
            x = col;
            z = row - 1;
            DrawConnectionGizmosAux(startPos, x, floor, z);
        }

        if (row < gridGenerator.gridSize.z - 1)
        {
            x = col;
            z = row + 1;
            DrawConnectionGizmosAux(startPos, x, floor, z);
        }
    }

    private void DrawConnectionGizmosAux(Vector3 startPos, int x, int y, int z)
    {
        if (!blockedNodes[x, y, z])
        {
            Vector3 targetPos = Vector3.Scale(new Vector3(x, y, z), gridGenerator.nodeSize);
            targetPos = Vector3.Lerp(startPos, targetPos, 0.4F);
            startPos = Vector3.Lerp(startPos, targetPos, 0.15F);
            Gizmos.DrawLine(startPos + gridGenerator.gridStart, targetPos + gridGenerator.gridStart);
        }
    }

    private Vector3? ScreenToWorldPoint(Camera cam)
    {
        //Create a plane whose normal points to +Y. We will raycast against this at the current floor height.
        Plane hPlane = new Plane(Vector3.up, new Vector3(0, currentFloor * gridGenerator.nodeSize.y, 0));

        Ray ray;
        if (cam != null && Application.isPlaying && !EditorApplication.isPaused)
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
        }
        else
        {
            ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        }

        if (hPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        return null;
    }
}
