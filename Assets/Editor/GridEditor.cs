using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(Grid))]
public class ObjectBuilderEditor : Editor {
    bool layers = true;
    bool advanced = true;

    public Grid.Connections connections;
    public Grid.Heuristics heurastic;
    public TerrainType mask;




    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();
        Grid myScript = (Grid)target;

        //grid = EditorGUILayout.Foldout(grid, "Grid");
        EditorGUILayout.Space();
        GUIStyle labelStyle = EditorStyles.label;
        labelStyle.fontStyle = FontStyle.Bold;
        int temp = labelStyle.fontSize;
        labelStyle.fontSize = 15;

        //EditorGUILayout.LabelField("Grid", labelStyle);
        //EditorGUILayout.Space();
        labelStyle.fontSize = temp;
        labelStyle.fontStyle = FontStyle.Normal;

        myScript.gridWorldSize = EditorGUILayout.Vector2Field("Grid size: ",myScript.gridWorldSize);
        myScript.nodeRadius = EditorGUILayout.FloatField("Node radius: ", myScript.nodeRadius);
        myScript.nearestNodeDistance = EditorGUILayout.FloatField("Nearest node distance: ", myScript.nearestNodeDistance);
        myScript.collisionRadius = EditorGUILayout.Slider("Collision radius: ", myScript.collisionRadius, 0, 3);
        EditorGUILayout.Space();
        GUIStyle style = EditorStyles.foldout;
        FontStyle previousStyle = style.fontStyle;
        style.fontStyle = FontStyle.Bold;

        layers = EditorGUILayout.Foldout(layers, "Layers", style);
        if (layers)
        {
            //myScript.unwalkableMask = LayerMaskField("Unwalkable layers:", myScript.unwalkableMask);
            SerializedObject serializedObject1 = new SerializedObject(target);
            SerializedProperty property2 = serializedObject1.FindProperty("unwalkableMask");
            serializedObject1.Update();
            EditorGUILayout.PropertyField(property2, true);
            serializedObject1.ApplyModifiedProperties();



            SerializedObject serializedObject = new SerializedObject(target);
            SerializedProperty property = serializedObject.FindProperty("walkableRegions");
            serializedObject.Update();
            EditorGUILayout.PropertyField(property, true);
            serializedObject.ApplyModifiedProperties();

        }

        EditorGUILayout.Space();

        advanced = EditorGUILayout.Foldout(advanced, "Advanced", style);
        //style.fontStyle = previousStyle;
        if (advanced)
        {
            //EditorGUILayout.LabelField("Inspector", EditorStyles.boldLabel);

            myScript.options = (Grid.Connections)EditorGUILayout.EnumPopup("Connections", myScript.options);
            myScript.heuristicMultiplier = EditorGUILayout.Slider("Heuristic estimation: ", myScript.heuristicMultiplier, 0,3);
            //myScript.heuristicMethod = (Grid.Heuristics)EditorGUILayout.EnumPopup("Heuristics", myScript.heuristicMethod);
            myScript.showGrid = EditorGUILayout.Toggle("Show Grid", myScript.showGrid);
            myScript.showPathSearchDebug = EditorGUILayout.Toggle("Show search debug", myScript.showPathSearchDebug);
            myScript.useThreading = EditorGUILayout.Toggle("Use threading", myScript.useThreading);
        }



        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();


        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Test grid", GUILayout.Width(300), GUILayout.Height(30))) {
            myScript.CreateGrid();

        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();


        EditorUtility.SetDirty(myScript);
    }


}


public class MyWindow : EditorWindow {
    [MenuItem("Window/Create Grid")]
    static void Init() {
        Grid instance = FindObjectOfType<Grid>();
        if (instance == null)
        {
            GameObject obj = new GameObject();
            obj.name = typeof(Grid).Name;
            instance = obj.AddComponent<Grid>();
        }
        else {
            Debug.LogError("Grid already in scene.");
        }
    }

}
