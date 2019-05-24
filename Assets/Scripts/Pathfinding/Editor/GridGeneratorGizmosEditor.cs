using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridGeneratorGizmos))]
public class GridGeneratorGizmosEditor : Editor
{
    private GridGeneratorGizmos gridGizmos;

    private void Awake()
    {
        gridGizmos = (GridGeneratorGizmos)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Bake Preview Grid"))
        {
            gridGizmos.BakePreviewGrid();
        }
    }
}
