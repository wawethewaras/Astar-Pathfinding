using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
namespace Astar2DPathFinding.Mika {

    [CustomEditor(typeof(PathfindingGrid))]
    public class ObjectBuilderEditor : Editor {
        bool layers = true;
        bool advanced = true;

        public PathfindingGrid.Connections connections;
        public PathfindingGrid.Heuristics heurastic;
        public TerrainType mask;


        //SerializedProperty nodeRadiusProp; 
        //SerializedProperty nearestNodeDistanceProp;
        //SerializedProperty collisionRadiusProp;

        //void OnEnable() {
        //    // Setup the SerializedProperties.
        //    nodeRadiusProp = serializedObject.FindProperty("nodeRadius");
        //    nearestNodeDistanceProp = serializedObject.FindProperty("nearestNodeDistance");
        //    collisionRadiusProp = serializedObject.FindProperty("collisionRadius");
        //}

        public override void OnInspectorGUI() {
            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update();

            //EditorGUILayout..PropertyField(nodeRadiusProp, new GUIContent("Node radius"));


            PathfindingGrid pathFindingGrid = (PathfindingGrid)target;

            EditorGUILayout.Space();
            GUIStyle labelStyle = EditorStyles.label;
            labelStyle.fontStyle = FontStyle.Bold;
            int temp = labelStyle.fontSize;
            labelStyle.fontSize = 15;


            labelStyle.fontSize = temp;
            labelStyle.fontStyle = FontStyle.Normal;

            pathFindingGrid.gridWorldSize = EditorGUILayout.Vector2Field("Grid size: ", pathFindingGrid.gridWorldSize);
            pathFindingGrid.nodeRadius = EditorGUILayout.FloatField("Node radius: ", pathFindingGrid.nodeRadius);
            pathFindingGrid.nearestNodeDistance = EditorGUILayout.FloatField("Nearest node distance: ", pathFindingGrid.nearestNodeDistance);
            pathFindingGrid.collisionRadius = EditorGUILayout.Slider("Collision radius: ", pathFindingGrid.collisionRadius, 0, 3);
            EditorGUILayout.Space();
            GUIStyle style = EditorStyles.foldout;
            FontStyle previousStyle = style.fontStyle;
            style.fontStyle = FontStyle.Bold;

            layers = EditorGUILayout.Foldout(layers, "Layers", style);
            if (layers) {
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
            if (advanced) {
                //EditorGUILayout.LabelField("Inspector", EditorStyles.boldLabel);

                pathFindingGrid.options = (PathfindingGrid.Connections)EditorGUILayout.EnumPopup("Connections", pathFindingGrid.options);
                pathFindingGrid.heuristicMultiplier = EditorGUILayout.Slider("Heuristic estimation: ", pathFindingGrid.heuristicMultiplier, 0, 3);
                //myScript.heuristicMethod = (Grid.Heuristics)EditorGUILayout.EnumPopup("Heuristics", myScript.heuristicMethod);
                pathFindingGrid.showGrid = EditorGUILayout.Toggle("Show Grid", pathFindingGrid.showGrid);
                pathFindingGrid.showPathSearchDebug = EditorGUILayout.Toggle("Show search debug", pathFindingGrid.showPathSearchDebug);
            }



            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();


            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Test grid", GUILayout.Width(300), GUILayout.Height(30))) {
                pathFindingGrid.CreateGrid();

            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            EditorUtility.SetDirty(pathFindingGrid);
        }


    }


    public class MyWindow : EditorWindow {
        [MenuItem("Window/Create Grid")]
        static void Init() {
            PathfindingGrid instance = FindObjectOfType<PathfindingGrid>();
            if (instance == null) {
                GameObject obj = new GameObject();
                obj.name = typeof(PathfindingGrid).Name;
                instance = obj.AddComponent<PathfindingGrid>();
            }
            else {
                Debug.LogError("Grid already in scene.");
            }
        }

    }
}