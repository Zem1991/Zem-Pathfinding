using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZemDirections;

public class Pathfinder
{
    private GridGenerator _grid;

    public Pathfinder()
    {
        _grid = GridGenerator.Singleton;
        if (!_grid) throw new MissingComponentException("Missing component: GridGenerator");
    }

    public void FindPath(Vector3 startPos, Vector3 endPos, out List<PathNode> result, out float size, out float operations)
    {
        result = null;
        size = 0;
        operations = 0;

        GridNode startGN = _grid.GridNodeFromWorldPosition(startPos);
        GridNode targetGN = _grid.GridNodeFromWorldPosition(endPos);
        Debug.Log("start: " + startGN.id + " | end: " + targetGN.id);

        PathNode startPN = new PathNode(startGN);
        PathNode targetPN = new PathNode(targetGN);

        HashSet<Vector3Int> closedList = new HashSet<Vector3Int>();
        List<PathNode> openList = new List<PathNode>();
        openList.Add(startPN);
        while (openList.Count > 0)
        {
            //New operation: process an PathNode
            operations++;

            //Get the PathNode with the lowest totalDistance/fCost
            PathNode currentPN = openList[0];
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].totalDistance < currentPN.totalDistance ||
                    (openList[i].totalDistance == currentPN.totalDistance && openList[i].distanceFromTarget < currentPN.distanceFromTarget))
                {
                    currentPN = openList[i];
                }
            }
            openList.Remove(currentPN);
            closedList.Add(currentPN.gridNode.id);

            //Make path if the target node was found
            if (currentPN.gridNode.id == targetGN.id)
            {
                MakePath(startPN, currentPN, out result, out size);
                break;
            }

            //Identify and process accessible neighbouring nodes
            foreach (OctoDirXZ dir in currentPN.gridNode.GetNeighboursConnected())
            {
                //Neighbour node must exist
                GridNode neighbourGN = currentPN.gridNode.GetNeighbour(dir);
                if (!neighbourGN) continue;

                //Neighbour node cannot be on the closed set
                PathNode neighbourPN = new PathNode(neighbourGN);
                if (closedList.Contains(neighbourGN.id)) continue;

                //Switch to existing node if possible
                PathNode existingPN = ListContainsNodeId(openList, neighbourGN.id);
                bool neighbourOnOpenList = (existingPN != null);
                if (neighbourOnOpenList) neighbourPN = existingPN;

                float moveCost = currentPN.distanceFromStart + DistanceFromHeuristic(currentPN, neighbourPN);
                if (!neighbourOnOpenList || moveCost < neighbourPN.distanceFromStart)
                {
                    //New operation: create/update an PathNode
                    operations++;

                    neighbourPN.distanceFromStart = moveCost;
                    neighbourPN.distanceFromTarget = DistanceFromHeuristic(neighbourPN, targetPN);
                    neighbourPN.previous = currentPN;
                    if (!neighbourOnOpenList) openList.Add(neighbourPN);
                }
            }
        }
    }

    private void MakePath(PathNode startNode, PathNode targetNode, out List<PathNode> result, out float size)
    {
        result = new List<PathNode>();
        size = 0;
        PathNode currentNode = targetNode;
        while (currentNode != startNode)
        {
            result.Add(currentNode);
            size += DistanceFromHeuristic(currentNode, currentNode.previous);
            currentNode = currentNode.previous;
        }
        result.Reverse();
    }

    private PathNode ListContainsNodeId(IList<PathNode> list, Vector3Int id)
    {
        foreach (PathNode item in list)
        {
            if (item.gridNode.id == id)
                return item;
        }
        return null;
    }

    private float DistanceFromHeuristic(PathNode from, PathNode to)
    {
        int distX = Mathf.Abs(from.gridNode.id.x - to.gridNode.id.x);
        int distZ = Mathf.Abs(from.gridNode.id.z - to.gridNode.id.z);
        int distY = Mathf.Abs(from.gridNode.id.y - to.gridNode.id.y);

        if (distX > distZ)
        {
            return 14 * distZ + 10 * (distX - distZ) + 10 * distY;
        }
        else
        {
            return 14 * distX + 10 * (distZ - distX) + 10 * distY;
        }
    }
}
