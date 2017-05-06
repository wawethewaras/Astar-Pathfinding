using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridManager))]
public class ReCalculateObstacles : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridManager myScript = (GridManager)target;
        if (GUILayout.Button("Calculate grid"))
        {
            //myScript.CalculateObstacles();
        }
    }
}
