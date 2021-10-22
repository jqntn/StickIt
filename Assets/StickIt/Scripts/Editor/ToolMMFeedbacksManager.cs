using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ToolMMFeedbacksManager))]
public class ToolMMFeedbacksManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ToolMMFeedbacksManager script = (ToolMMFeedbacksManager)target;
        if (GUILayout.Button("Generate Files"))
        {
            script.GenerateFiles();
        }
    }
}