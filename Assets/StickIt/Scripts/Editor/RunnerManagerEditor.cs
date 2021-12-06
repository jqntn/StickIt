using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(RunnerManager))]
[CanEditMultipleObjects]
public class RunnerManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        RunnerManager script = (RunnerManager)target;

        //if (EditorGUILayout.BeginFadeGroup(m_ShowExtraFields.faded))
        //{
        //    EditorGUILayout.PrefixLabel("Color");
        //    m_Color = EditorGUILayout.ColorField(m_Color);
        //    EditorGUILayout.PrefixLabel("Text");
        //    m_String = EditorGUILayout.TextField(m_String);
        //    EditorGUILayout.PrefixLabel("Number");
        //    m_Number = EditorGUILayout.IntSlider(m_Number, 0, 10);
        //}

    }
}
