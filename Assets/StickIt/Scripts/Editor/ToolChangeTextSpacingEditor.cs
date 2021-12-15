using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ToolChangeTextSpacing))]
public class ToolChangeTextSpacingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ToolChangeTextSpacing script = (ToolChangeTextSpacing)target;
        if (GUILayout.Button("ChangeCharacterSpacing"))
        {
            script.ChangeCharacterSpacing();
        }
    }
}

