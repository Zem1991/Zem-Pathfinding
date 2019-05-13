using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public GridNode gridNode;           //this node on the "visible" grid
    public PathNode previous;           //from where it came from during the pathfinding process
    public float distanceFromStart;     //the G cost
    public float distanceFromTarget;    //the H cost
    public float totalDistance { get { return distanceFromStart + distanceFromTarget; } }  //the F cost

    public PathNode(GridNode gridNode)
    {
        this.gridNode = gridNode;
    }
}
