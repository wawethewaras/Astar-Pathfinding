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
    public Grid.Heurastics heurastic;
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

        EditorGUILayout.LabelField("Grid", labelStyle);
        EditorGUILayout.Space();
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
            myScript.heurasticMultiplier = EditorGUILayout.Slider("Heurastic estimation: ", myScript.heurasticMultiplier, 0,3);
            myScript.heurasticMethod = (Grid.Heurastics)EditorGUILayout.EnumPopup("Heurastics", myScript.heurasticMethod);
            myScript.showGrid = EditorGUILayout.Toggle("Show Grid", myScript.showGrid);
            myScript.showPathSearchDebug = EditorGUILayout.Toggle("Show search debug", myScript.showPathSearchDebug);
            myScript.useThreading = EditorGUILayout.Toggle("Use threading", myScript.useThreading);
        }


        EditorUtility.SetDirty(myScript);

        if (GUILayout.Button("Test grid"))
        {
            myScript.CreateGrid();
        }
    }


    //public static LayerMask LayerMaskField(string label, LayerMask selected)
    //{
    //    return LayerMaskField(label, selected, true);
    //}

    //public static LayerMask LayerMaskField(string label, LayerMask selected, bool showSpecial)
    //{

    //    List<string> layers = new List<string>();
    //    List<int> layerNumbers = new List<int>();

    //    string selectedLayers = "";

    //    for (int i = 0; i < 32; i++)
    //    {

    //        string layerName = LayerMask.LayerToName(i);

    //        if (layerName != "")
    //        {
    //            if (selected == (selected | (1 << i)))
    //            {

    //                if (selectedLayers == "")
    //                {
    //                    selectedLayers = layerName;
    //                }
    //                else {
    //                    selectedLayers = "Mixed";
    //                }
    //            }
    //        }
    //    }

    //    EventType lastEvent = Event.current.type;

    //    if (Event.current.type != EventType.MouseDown && Event.current.type != EventType.ExecuteCommand)
    //    {
    //        if (selected.value == 0)
    //        {
    //            layers.Add("Nothing");
    //        }
    //        else if (selected.value == -1)
    //        {
    //            layers.Add("Everything");
    //        }
    //        else {
    //            layers.Add(selectedLayers);
    //        }
    //        layerNumbers.Add(-1);
    //    }

    //    if (showSpecial)
    //    {
    //        layers.Add((selected.value == 0 ? "[X] " : "     ") + "Nothing");
    //        layerNumbers.Add(-2);

    //        layers.Add((selected.value == -1 ? "[X] " : "     ") + "Everything");
    //        layerNumbers.Add(-3);
    //    }

    //    for (int i = 0; i < 32; i++)
    //    {

    //        string layerName = LayerMask.LayerToName(i);

    //        if (layerName != "")
    //        {
    //            if (selected == (selected | (1 << i)))
    //            {
    //                layers.Add("[X] " + layerName);
    //            }
    //            else {
    //                layers.Add("     " + layerName);
    //            }
    //            layerNumbers.Add(i);
    //        }
    //    }

    //    bool preChange = GUI.changed;

    //    GUI.changed = false;

    //    int newSelected = 0;

    //    if (Event.current.type == EventType.MouseDown)
    //    {
    //        newSelected = -1;
    //    }

    //    newSelected = EditorGUILayout.Popup(label, newSelected, layers.ToArray(), EditorStyles.layerMaskField);

    //    if (GUI.changed && newSelected >= 0)
    //    {
    //        //newSelected -= 1;

    //        //Debug.Log(lastEvent + " " + newSelected + " " + layerNumbers[newSelected]);

    //        if (showSpecial && newSelected == 0)
    //        {
    //            selected = 0;
    //        }
    //        else if (showSpecial && newSelected == 1)
    //        {
    //            selected = -1;
    //        }
    //        else {

    //            if (selected == (selected | (1 << layerNumbers[newSelected])))
    //            {
    //                selected &= ~(1 << layerNumbers[newSelected]);
    //                //Debug.Log ("Set Layer "+LayerMask.LayerToName (LayerNumbers[newSelected]) + " To False "+selected.value);
    //            }
    //            else {
    //                //Debug.Log ("Set Layer "+LayerMask.LayerToName (LayerNumbers[newSelected]) + " To True "+selected.value);
    //                selected = selected | (1 << layerNumbers[newSelected]);
    //            }
    //        }
    //    }
    //    else {
    //        GUI.changed = preChange;
    //    }

    //    return selected;
    //}
}



