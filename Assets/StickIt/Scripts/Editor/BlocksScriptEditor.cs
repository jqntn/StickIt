using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BlocksScript))]
[CanEditMultipleObjects]
public class BlocksScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {

        BlocksScript script = target as BlocksScript;

        script.extend = EditorGUILayout.Vector2Field("Extend", script.extend);
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Debug_________________________________________");
        EditorGUILayout.BoundsField("Bounds", script.bounds);
        EditorGUILayout.Vector2Field("Bounds Pos", script.boundsPos);
        EditorGUILayout.Vector2Field("Max", script.max);
        EditorGUILayout.Vector2Field("Dimension", script.dimension);
        EditorGUI.BeginDisabledGroup(false);
    }
}
