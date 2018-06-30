using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Astar2DPathFinding.Mika {

    [CustomPropertyDrawer(typeof(TerrainType))]
    class IngredientDrawer : PropertyDrawer {
        private object serializedObject;

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            // Calculate rects
            GUILayout.BeginHorizontal();
            {

                Rect amountRect = new Rect(position.x, position.y, 50, position.height);
                EditorGUI.LabelField(amountRect, "Mask: ");
                amountRect = new Rect(position.x + 50, position.y, 150, position.height);
                EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("terrainMask"), GUIContent.none);
                amountRect = new Rect(position.x + 200, position.y, 100, position.height);
                EditorGUI.LabelField(amountRect, "Penalty: ");
                amountRect = new Rect(position.x + 300, position.y, 150, position.height);
                EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("terrainPenalty"), GUIContent.none);

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                //// Set indent back to what it was
                //EditorGUI.indentLevel = indent;

                //EditorGUI.EndProperty();
            }
            GUILayout.EndHorizontal();

        }

    }
}
