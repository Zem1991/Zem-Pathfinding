using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public static GridGenerator Singleton { get; private set; }

    public GridNode prefab_GridNode;
    public Vector3 gridStart = new Vector3(0, 0, 0);
    public Vector3Int gridSize = new Vector3Int(20, 1, 20);
    public Vector3 nodeSize = new Vector3(1, 2, 1);
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

    //Commented because not used
    //public GridNode GridNodeFromId(Vector3Int id)
    //{
    //    return gridNodes[id.x, id.y, id.z];
    //}

    public GridNode GridNodeFromWorldPosition(Vector3 worldPos)
    {
        Vector3Int? attempt = GridNodeIdFromWorldPosition(worldPos);
        if (attempt == null) return null;
        Vector3Int id = (Vector3Int)attempt;
        return gridNodes[id.x, id.y, id.z];
    }

    public Vector3Int? GridNodeIdFromWorldPosition(Vector3 worldPos)
    {
        float auxX = (worldPos.x / nodeSize.x) - gridStart.x;
        float auxY = (worldPos.y / nodeSize.y) - gridStart.y;
        float auxZ = (worldPos.z / nodeSize.z) - gridStart.z;
        int x = Mathf.RoundToInt(auxX);
        int y = Mathf.RoundToInt(auxY);
        int z = Mathf.RoundToInt(auxZ);
        if (x < 0 || x > gridSize.x - 1) return null;
        if (y < 0 || y > gridSize.y - 1) return null;
        if (z < 0 || z > gridSize.z - 1) return null;
        return new Vector3Int(x, y, z);
    }

    public Vector3? ScreenToGridPoint(Camera cam, int floor)
    {
        // create a plane whose normal points to +Y:
        Plane hPlane = new Plane(Vector3.up, new Vector3(0, floor * nodeSize.y, 0));

        // Plane.Raycast stores the distance from ray.origin to the hit point in this variable:
        //position = Event.current.mousePosition;   //TODO MOVE THIS ELSEWHERE
        //Vector3 position = cam.ScreenToWorldPoint(Input.mousePosition);
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        // if the ray hits the plane...
        if (hPlane.Raycast(ray, out float distance))
        {
            // get the hit point:
            return ray.GetPoint(distance);
        }
        return null;
    }

    public Vector3 ClampWorldPosToGrid(Vector3 worldPos, Vector3 innerLimits)
    {
        float halfX = nodeSize.x / 2F;
        float halfY = nodeSize.y / 2F;
        float halfZ = nodeSize.z / 2F;

        float minX = gridStart.x - halfX;
        float maxX = minX + (gridSize.x * nodeSize.x);
        minX += innerLimits.x;
        maxX -= innerLimits.x;

        float minY = gridStart.y - halfY;
        float maxY = minY + (gridSize.y * nodeSize.y);

        float minZ = gridStart.z - halfZ;
        float maxZ = minZ + (gridSize.z * nodeSize.z);
        minZ += innerLimits.z;
        maxZ -= innerLimits.z;

        float x = Mathf.Clamp(worldPos.x, minX, maxX);
        float y = Mathf.Clamp(worldPos.y, minY, maxY);
        float z = Mathf.Clamp(worldPos.z, minZ, maxZ);
        return new Vector3(x, y, z);
    }
}
