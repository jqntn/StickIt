#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ParallaxSettings))]
public class ParallaxSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Save changes(Do Ctrl+S after click)"))
        {
            EditorUtility.SetDirty(target);
        }
        var parallaxSettings = target as ParallaxSettings;
        var serializedObject = new SerializedObject(target);
        var property = serializedObject.FindProperty("ParallaxElements");


        GUILayout.Label("\nParallax settings");

        EditorGUILayout.PropertyField(property, true);
        //parallaxSettings.ParallaxElements = EditorGUILayout.ObjectField("Size collider root", GameObject[],parallaxSettings.ParallaxElements);
        parallaxSettings.posZBegin = EditorGUILayout.IntField("Position of the first element on Z", parallaxSettings.posZBegin);
        parallaxSettings.parallaxMultiplier = EditorGUILayout.IntField("Multiplier for Z", parallaxSettings.parallaxMultiplier);
        if (GUILayout.Button("Do changes"))
        {
            parallaxSettings.ChangePosZ();
            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif