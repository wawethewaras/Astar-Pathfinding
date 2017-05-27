using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Grid))]
public class ObjectBuilderEditor : Editor
{
    bool showPosition = true;
    //string status = "Advanced";
    //bool posGroupEnabled = true;
    //public void OnGUI()
    //{
    //    Grid g = 
    //    showPosition = EditorGUILayout.Foldout(showPosition, status);
    //    if (showPosition)


    //    if (!Selection.activeTransform)
    //    {
    //        status = "Select a GameObject";
    //        showPosition = false;
    //    }
    //}

    public Grid.Connections connections;
    public Grid.Heurastics heurastic;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Grid myScript = (Grid)target;
        GUIStyle style = EditorStyles.foldout;
        FontStyle previousStyle = style.fontStyle;
        style.fontStyle = FontStyle.Bold;
        showPosition = EditorGUILayout.Foldout(showPosition, "Advanced", style);
        style.fontStyle = previousStyle;
        if (showPosition)
        {
            //EditorGUILayout.LabelField("Inspector", EditorStyles.boldLabel);

            myScript.options = (Grid.Connections)EditorGUILayout.EnumPopup("Connections", connections);
            myScript.heurasticMethod = (Grid.Heurastics)EditorGUILayout.EnumPopup("Heurastics", heurastic);
            myScript.showGrid = EditorGUILayout.Toggle("Show Grid.", myScript.showGrid);
            myScript.showPathSearchDebug = EditorGUILayout.Toggle("Show search debug", myScript.showPathSearchDebug);
            myScript.useThreading = EditorGUILayout.Toggle("Use threading.", myScript.useThreading);
        }

        if (GUILayout.Button("Test grid"))
        {
            myScript.CreateGrid();
        }
    }

}
