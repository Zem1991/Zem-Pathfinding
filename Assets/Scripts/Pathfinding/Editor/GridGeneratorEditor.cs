using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridGenerator))]
public class GridGeneratorEditor : Editor
{
    private GridGenerator gridGen;

    private void Awake()
    {
        gridGen = (GridGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //if (GUILayout.Button("Remove Grid"))  //Currently makes a grid on each scene change
        //{
        //    gridGen.RemoveGrid();
        //}

        if (GUILayout.Button("Make Grid"))
        {
            gridGen.MakeGrid();
        }
    }

    //public void OnSceneGUI()
    //{
    //    Rect toolbarRect = new Rect(10, 10, 200, 30);
    //    toolbarRect = GUILayout.Window(0, toolbarRect, ToolbarWindow, "Toolbar");

    //    RefreshGridPos();

    //    HandleUtility.Repaint();
    //}

    //private void ToolbarWindow(int windowID)
    //{
    //    GUILayout.BeginHorizontal();
    //    if (GUILayout.Button("Pencil"))
    //        gridGen.setCurrentTool(TilemapEditorTool.PENCIL);
    //    if (GUILayout.Button("Line"))
    //        gridGen.setCurrentTool(TilemapEditorTool.LINE);
    //    if (GUILayout.Button("Square"))
    //        gridGen.setCurrentTool(TilemapEditorTool.SQUARE);
    //    if (GUILayout.Button("Circle"))
    //        gridGen.setCurrentTool(TilemapEditorTool.CIRCLE);
    //    if (GUILayout.Button("Pick"))
    //        gridGen.setCurrentTool(TilemapEditorTool.PICK);
    //    if (GUILayout.Button("Fill"))
    //        gridGen.setCurrentTool(TilemapEditorTool.FILL);
    //    GUILayout.EndHorizontal();
    //}

    //private void RefreshGridPos()
    //{
    //    Plane hPlane = new Plane(Vector3.up, Vector3.zero);
    //    Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
    //    if (hPlane.Raycast(ray, out float distance))
    //    {
    //        Vector3 worldPos = ray.GetPoint(distance);
    //        worldPos.x /= gridGen.tileSize.x;
    //        worldPos.z /= gridGen.tileSize.z;
    //        gridGen.setCurrentGridPos(Vector3Int.RoundToInt(worldPos));
    //    }
    //}
}
