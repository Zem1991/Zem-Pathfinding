using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public static GridGenerator Singleton { get; private set; }

    [Header("Node settings")]
    public GridNode prefab_GridNode;
    public Vector3Int gridSize = new Vector3Int(40, 1, 40);
    public Vector3 gridStart = new Vector3(0, 0, 0);
    public Vector3 nodeSize = new Vector3(1, 2.5F, 1);
    public Vector3 nodeStart = new Vector3(-0.5F, 0, -0.5F);
    [Header("Layers")]
    public LayerMask layerObstacles;
    public LayerMask layerWalls;
    public LayerMask layerUnits;

    public GridNode[,,] gridNodes { get; private set; }

    private void Awake()
    {
        if (!Singleton)
        {
            Debug.Log("Singleton of GridGenerator set to instance " + this);
            Singleton = this;
        }
        if (Singleton != this)
        {
            Debug.LogError("There already is an singleton of GridGenerator.");
            DestroyImmediate(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        MakeGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RemoveGrid()
    {
        if (gridNodes != null)
        {
            foreach (var item in gridNodes)
                DestroyImmediate(item?.gameObject);
            foreach (var item in GetComponentsInChildren<GridNode>())
                DestroyImmediate(item?.gameObject);
        }
        gridNodes = null;
    }

    public void MakeGrid()
    {
        RemoveGrid();

        gridNodes = new GridNode[gridSize.x, gridSize.y, gridSize.z];
        for (int floor = 0; floor < gridSize.y; floor++)
        {
            for (int row = 0; row < gridSize.z; row++)
            {
                for (int col = 0; col < gridSize.x; col++)
                {
                    Vector3 position = new Vector3(
                        col * nodeSize.x,
                        floor * nodeSize.y,
                        row * nodeSize.z
                    );
                    position += gridStart;

                    GridNode gn = Instantiate(prefab_GridNode, position, Quaternion.identity, transform);
                    gn.transform.localScale = nodeSize;

                    gn.id = new Vector3Int(col, floor, row);
                    gn.name = "Grid Node " + gn.id.ToString();

                    if (col > 0)
                    {
                        gn.l = gridNodes[col - 1, floor, row];
                        gridNodes[col - 1, floor, row].r = gn;
                    }
                    if (row > 0)
                    {
                        gn.b = gridNodes[col, floor, row - 1];
                        gridNodes[col, floor, row - 1].f = gn;
                    }
                    if (col > 0 && row > 0)
                    {
                        gn.bl = gridNodes[col - 1, floor, row - 1];
                        gridNodes[col - 1, floor, row - 1].fr = gn;
                    }
                    if (col < gridSize.x - 1 && row > 0)
                    {
                        gn.br = gridNodes[col + 1, floor, row - 1];
                        gridNodes[col + 1, floor, row - 1].fl = gn;
                    }
                    gridNodes[col, floor, row] = gn;
                }
            }
        }

        UpdateGridConnections();
    }

    public void UpdateGridConnections()
    {
        foreach (GridNode gn in gridNodes)
        {
            gn.UpdatePassability(layerObstacles);
        }
        foreach (GridNode gn in gridNodes)
        {
            gn.UpdateConnections(layerWalls);
        }
    }

    public GridNode GridNodeFromWorldPosition(Vector3 worldPos)
    {
        Vector3Int? attempt = GridNodeIdFromWorldPosition(worldPos);
        if (attempt == null) return null;
        Vector3Int id = (Vector3Int)attempt;
        return gridNodes[id.x, id.y, id.z];
    }

    public Vector3Int? GridNodeIdFromWorldPosition(Vector3 worldPos)
    {
        float fX = (worldPos.x / nodeSize.x);
        float fY = (worldPos.y / nodeSize.y);
        float fZ = (worldPos.z / nodeSize.z);
        int x = (int)Mathf.Floor(fX);
        int y = (int)Mathf.Floor(fY);
        int z = (int)Mathf.Floor(fZ);
        if (x < 0 || x > gridSize.x - 1) return null;
        if (y < 0 || y > gridSize.y - 1) return null;
        if (z < 0 || z > gridSize.z - 1) return null;
        return new Vector3Int(x, y, z);
    }

    public Vector3 ClampWorldPosToGrid(Vector3 worldPos, Vector3 innerLimits)
    {
        float minX = gridStart.x + nodeStart.x;
        float maxX = minX + (gridSize.x * nodeSize.x);
        minX += innerLimits.x;
        maxX -= innerLimits.x;

        float minY = gridStart.y + nodeStart.y;
        float maxY = minY + (gridSize.y * nodeSize.y);

        float minZ = gridStart.z + nodeStart.z;
        float maxZ = minZ + (gridSize.z * nodeSize.z);
        minZ += innerLimits.z;
        maxZ -= innerLimits.z;

        float x = Mathf.Clamp(worldPos.x, minX, maxX);
        float y = Mathf.Clamp(worldPos.y, minY, maxY);
        float z = Mathf.Clamp(worldPos.z, minZ, maxZ);
        return new Vector3(x, y, z);
    }
}
