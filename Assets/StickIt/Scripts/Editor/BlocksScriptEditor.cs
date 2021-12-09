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
        BlocksScript script = (BlocksScript)target;

        script.extendsFactor = EditorGUILayout.FloatField("Extends ", script.extendsFactor);
        script.offsets = EditorGUILayout.Vector3Field("Offsets ", script.offsets);
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.BoundsField("Bounds ", script.bounds);
        EditorGUILayout.Vector2Field("Max ", script.max);
        EditorGUILayout.Vector2Field("Dimension ", script.dimension);
        EditorGUILayout.Vector2Field("Factors ", script.factors);
        EditorGUI.BeginDisabledGroup(false);
    }
}
