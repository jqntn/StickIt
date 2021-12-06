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
        EditorGUI.BeginDisabledGroup(true);
        base.OnInspectorGUI();
        EditorGUI.BeginDisabledGroup(false);
    }
}
