using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ToolChangeFontText))]
public class ToolChangeFontTextEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ToolChangeFontText script = (ToolChangeFontText)target;
        if (GUILayout.Button("ChangeFont"))
        {
            script.ChangeFontText();
        }
    }
}
