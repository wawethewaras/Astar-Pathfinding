using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Grid))]
public class ReCalculateObstacles : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Grid myScript = (Grid)target;
        if (GUILayout.Button("Calculate grid"))
        {
            myScript.CreateGrid();
        }
    }
}
