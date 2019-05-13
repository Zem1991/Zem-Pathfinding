using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZemDirections;

public class GridNode : MonoBehaviour
{
    [Header("Neighbours")]
    public GridNode bl;
    public GridNode b;
    public GridNode br;
    public GridNode l;
    public GridNode r;
    public GridNode fl;
    public GridNode f;
    public GridNode fr;
    [Header("Parameters")]
    public Vector3Int id;
    public bool blocked;
    public bool bl_Blocked;
    public bool b__Blocked;
    public bool br_Blocked;
    public bool l__Blocked;
    public bool r__Blocked;
    public bool fl_Blocked;
    public bool f__Blocked;
    public bool fr_Blocked;

    public void Start()
    {
        
    }

    public void Update()
    {
        
    }

    //Commented because not in use
    //public List<OctoDirXZ> GetNeighbours()
    //{
    //    List<OctoDirXZ> result = new List<OctoDirXZ>();
    //    OctoDirXZ dir = OctoDirXZ.BACK_LEFT;
    //    if (GetNeighbour(dir)) result.Add(dir);
    //    dir = OctoDirXZ.BACK;
    //    if (GetNeighbour(dir)) result.Add(dir);
    //    dir = OctoDirXZ.BACK_RIGHT;
    //    if (GetNeighbour(dir)) result.Add(dir);
    //    dir = OctoDirXZ.LEFT;
    //    if (GetNeighbour(dir)) result.Add(dir);
    //    dir = OctoDirXZ.RIGHT;
    //    if (GetNeighbour(dir)) result.Add(dir);
    //    dir = OctoDirXZ.FRONT_LEFT;
    //    if (GetNeighbour(dir)) result.Add(dir);
    //    dir = OctoDirXZ.FRONT;
    //    if (GetNeighbour(dir)) result.Add(dir);
    //    dir = OctoDirXZ.FRONT_RIGHT;
    //    if (GetNeighbour(dir)) result.Add(dir);
    //    return result;
    //}

    public List<OctoDirXZ> GetNeighboursConnected()
    {
        List<OctoDirXZ> result = new List<OctoDirXZ>();
        bool connectionCheck = bl_Blocked;
        OctoDirXZ dir = OctoDirXZ.BACK_LEFT;
        if (connectionCheck) result.Add(dir);
        connectionCheck = b__Blocked;
        dir = OctoDirXZ.BACK;
        if (connectionCheck) result.Add(dir);
        connectionCheck = br_Blocked;
        dir = OctoDirXZ.BACK_RIGHT;
        if (connectionCheck) result.Add(dir);
        connectionCheck = l__Blocked;
        dir = OctoDirXZ.LEFT;
        if (connectionCheck) result.Add(dir);
        connectionCheck = r__Blocked;
        dir = OctoDirXZ.RIGHT;
        if (connectionCheck) result.Add(dir);
        connectionCheck = fl_Blocked;
        dir = OctoDirXZ.FRONT_LEFT;
        if (connectionCheck) result.Add(dir);
        connectionCheck = f__Blocked;
        dir = OctoDirXZ.FRONT;
        if (connectionCheck) result.Add(dir);
        connectionCheck = fr_Blocked;
        dir = OctoDirXZ.FRONT_RIGHT;
        if (connectionCheck) result.Add(dir);
        return result;
    }

    public GridNode GetNeighbour(OctoDirXZ odxz)
    {
        GridNode result = null;
        switch (odxz)
        {
            case OctoDirXZ.BACK_LEFT:
                result = bl;
                break;
            case OctoDirXZ.BACK:
                result = b;
                break;
            case OctoDirXZ.BACK_RIGHT:
                result = br;
                break;
            case OctoDirXZ.LEFT:
                result = l;
                break;
            case OctoDirXZ.RIGHT:
                result = r;
                break;
            case OctoDirXZ.FRONT_LEFT:
                result = fl;
                break;
            case OctoDirXZ.FRONT:
                result = f;
                break;
            case OctoDirXZ.FRONT_RIGHT:
                result = fr;
                break;
        }
        return result;
    }

    public void UpdatePassability(LayerMask layerObstacles)
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2.001F, Quaternion.identity, layerObstacles);
        blocked = colliders.Length > 0;
    }

    public void UpdateConnections(LayerMask layerWalls)
    {
        OctoDirXZ dir = OctoDirXZ.BACK_LEFT;
        bl_Blocked = CheckConnection(dir, layerWalls);
        dir = OctoDirXZ.BACK;
        b__Blocked = CheckConnection(dir, layerWalls);
        dir = OctoDirXZ.BACK_RIGHT;
        br_Blocked = CheckConnection(dir, layerWalls);
        dir = OctoDirXZ.LEFT;
        l__Blocked = CheckConnection(dir, layerWalls);
        dir = OctoDirXZ.RIGHT;
        r__Blocked = CheckConnection(dir, layerWalls);
        dir = OctoDirXZ.FRONT_LEFT;
        fl_Blocked = CheckConnection(dir, layerWalls);
        dir = OctoDirXZ.FRONT;
        f__Blocked = CheckConnection(dir, layerWalls);
        dir = OctoDirXZ.FRONT_RIGHT;
        fr_Blocked = CheckConnection(dir, layerWalls);
    }

    public bool CheckConnection(OctoDirXZ odxz, LayerMask layerWalls)
    {
        GridNode target = GetNeighbour(odxz);
        if (!target) return false;

        Vector3 midPos = Vector3.Lerp(transform.position, target.transform.position, 0.5F);
        Collider[] colliders = Physics.OverlapBox(midPos, transform.localScale / 3.001F, Quaternion.identity, layerWalls);
        if (colliders.Length > 0) return false;

        if (odxz.IsDiagonal())
            return CheckConnectionDiagonal(target);
        else
            return CheckConnection(target);
    }

    private bool CheckConnection(GridNode target)
    {
        if (!target) return false;
        return !blocked && !target.blocked;
    }

    private bool CheckConnectionDiagonal(GridNode target)
    {
        GridNode sideA = null;
        GridNode sideB = null;
        if (target == bl)
        {
            sideA = b;
            sideB = l;
        }
        if (target == br)
        {
            sideA = b;
            sideB = r;
        }
        if (target == fl)
        {
            sideA = f;
            sideB = l;
        }
        if (target == fr)
        {
            sideA = f;
            sideB = r;
        }
        if (!sideA || !sideB) return false;
        return CheckConnection(target) && !sideA.blocked && !sideB.blocked;
    }
}
