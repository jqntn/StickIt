using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TestShakeCall))]
public class TestShakeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TestShakeCall script = (TestShakeCall)target;
        if (GUILayout.Button("Somthing Is Happening"))
        {
            script.SomethingIsHappening();
        }
    }
}
