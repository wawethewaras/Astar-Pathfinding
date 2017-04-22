using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestCode))]
public class MoveToPath : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TestCode myScript = (TestCode)target;
        if (GUILayout.Button("FindPath"))
        {
            myScript.FindPath();
        }
    }
}
