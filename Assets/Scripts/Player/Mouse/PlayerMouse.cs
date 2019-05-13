using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMouse : MonoBehaviour
{
    private GameObject _telepointer;

    [Header("Screen Position Data")]
    public bool performingSelectionBox;
    public Vector2 mouseCurrentScreenPos;
    public Vector2 mouseLastScreenPos;

    [Header("Grid Position Data")]
    public bool outsideGrid;
    public Vector3 mouseCurrentGridPos;
    public Vector3 mouseLastGridPos;

    [Header("Mouse Pointer Data")]
    public GridNode gridNode;
    public GameObject obstacle;
    public GameObject wall;
    public GameObject unit;

    // Start is called before the first frame update
    void Start()
    {
        _telepointer = GetComponentInChildren<SphereCollider>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        _telepointer.transform.position = mouseCurrentGridPos;
    }

    public void RefreshData(Camera cam, int floor, bool btnPress, bool btnUp)
    {
        mouseCurrentScreenPos = Input.mousePosition;

        float height = GridGenerator.Singleton.nodeSize.y * floor;
        Plane xzPlane = new Plane(Vector3.up, new Vector3(0, height, 0));
        Ray ray = cam.ScreenPointToRay(mouseCurrentScreenPos);

        Vector3? planePos;
        if (xzPlane.Raycast(ray, out float distance))
        {
            planePos = ray.GetPoint(distance);

            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, distance, GridGenerator.Singleton.layerObstacles))
                obstacle = rayHit.transform.gameObject;
            else
                obstacle = null;
            if (Physics.Raycast(ray, out rayHit, distance, GridGenerator.Singleton.layerWalls))
                wall = rayHit.transform.gameObject;
            else
                wall = null;
            if (Physics.Raycast(ray, out rayHit, distance, GridGenerator.Singleton.layerUnits))
                unit = rayHit.transform.gameObject;
            else
                unit = null;
        }
        else
        {
            planePos = null;
        }

        if (planePos != null)
        {
            gridNode = GridGenerator.Singleton.GridNodeFromWorldPosition((Vector3)planePos);

            Vector3 gridPos = GridGenerator.Singleton.ClampWorldPosToGrid((Vector3)planePos, Vector3.zero);
            outsideGrid = (planePos != gridPos);
            mouseCurrentGridPos = gridPos;
        }
        else
        {
            outsideGrid = true;
            gridNode = null;
            obstacle = null;
            wall = null;
            unit = null;
        }

        if (!btnPress || btnUp)
        {
            performingSelectionBox = false;
            mouseLastScreenPos = mouseCurrentScreenPos;
            mouseLastGridPos = mouseCurrentGridPos;
        }
        else
        {
            performingSelectionBox = true;
        }
    }
}
