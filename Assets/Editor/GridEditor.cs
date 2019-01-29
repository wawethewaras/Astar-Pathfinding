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

        //public PathfindingGrid.Connections connections;
        //public PathfindingGrid.Heuristics heurastic;
        //public TerrainType mask;

        SerializedProperty gridWorldSizeProp;
        SerializedProperty nodeRadiusProp; 
        SerializedProperty nearestNodeDistanceProp;
        SerializedProperty collisionRadiusProp;

        //SerializedProperty heuristicMultiplierProp;
        //SerializedProperty connectionsOptionsProp;
        //SerializedProperty showGridProp;
        //SerializedProperty showPathSearchDebugProp;


        void OnEnable() {
            // Setup the SerializedProperties.
            gridWorldSizeProp = serializedObject.FindProperty("gridWorldSize");
            nodeRadiusProp = serializedObject.FindProperty("nodeRadius");
            nearestNodeDistanceProp = serializedObject.FindProperty("nearestNodeDistance");
            collisionRadiusProp = serializedObject.FindProperty("collisionRadius");

            //heuristicMultiplierProp = serializedObject.FindProperty("heuristicMultiplier");
            //connectionsOptionsProp = serializedObject.FindProperty("connectionsOptions");
            //showGridProp = serializedObject.FindProperty("showGrid");
            //showPathSearchDebugProp = serializedObject.FindProperty("showPathSearchDebug");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();


            EditorGUILayout.PropertyField(gridWorldSizeProp, new GUIContent("Grid size: "));
            EditorGUILayout.PropertyField(nodeRadiusProp, new GUIContent("Node radius: "));
            EditorGUILayout.PropertyField(nearestNodeDistanceProp, new GUIContent("Nearest Node Distance: "));
            EditorGUILayout.Slider(collisionRadiusProp, 0, 3, new GUIContent("Collision Radius: "));

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


            if (advanced) {
                //EditorGUILayout.LabelField("Inspector", EditorStyles.boldLabel);

                PathfindingGrid myScript = (PathfindingGrid)target;
                myScript.connectionsOptions = (PathfindingGrid.Connections)EditorGUILayout.EnumPopup("Connections", myScript.connectionsOptions);
                myScript.heuristicMultiplier = EditorGUILayout.Slider("Heuristic estimation: ", myScript.heuristicMultiplier, 0, 3);
                myScript.heuristicMethod = (PathfindingGrid.Heuristics)EditorGUILayout.EnumPopup("Heuristics", myScript.heuristicMethod);
                myScript.showGrid = EditorGUILayout.Toggle("Show Grid", myScript.showGrid);
                myScript.showPathSearchDebug = EditorGUILayout.Toggle("Show search debug", myScript.showPathSearchDebug);

            }

            serializedObject.ApplyModifiedProperties();

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