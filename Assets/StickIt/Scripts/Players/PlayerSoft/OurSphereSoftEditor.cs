#if UNITY_EDITOR
    using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(OurSphereSoft))]
[CanEditMultipleObjects]
public class OurSphereSoftEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Save Changes (Do Ctrl+S after click)"))
        {
            EditorUtility.SetDirty(target);
        }
        var sphereSoft = target as OurSphereSoft;
        var serializedObject = new SerializedObject(target);
        var property = serializedObject.FindProperty("bones");
       // GUILayout.Label("\nBones");
       // sphereSoft.root = (GameObject)EditorGUILayout.ObjectField("Root  ", sphereSoft.root, typeof(GameObject), allowSceneObjects: true);
        //EditorGUILayout.PropertyField(property, true);
        //sphereSoft.matBones = (PhysicMaterial)EditorGUILayout.ObjectField("Material Bones  ", sphereSoft.matBones, typeof(PhysicMaterial), allowSceneObjects: true);


        GUILayout.Label("\nColliders settings");
        sphereSoft.ColliderSizeRoot = EditorGUILayout.IntSlider("Size collider root", sphereSoft.ColliderSizeRoot, 8 , 15);
        sphereSoft.ColliderSize = EditorGUILayout.IntSlider("Size collider bones", sphereSoft.ColliderSize, 1, 7);
        sphereSoft.collSizeRoot = sphereSoft.ColliderSizeRoot / 10000f;
        sphereSoft.collSizeBones = sphereSoft.ColliderSize / 10000f;
        sphereSoft.RigidbodyMass = EditorGUILayout.FloatField("Mass of the each bone's rigibody", sphereSoft.RigidbodyMass);


        GUILayout.Label("\nJoints Settings");
        sphereSoft.ConfigurableJoint = EditorGUILayout.Toggle("Configurable Joint ?", sphereSoft.ConfigurableJoint);
        if (sphereSoft.ConfigurableJoint)
        {
            serializedObject.ApplyModifiedProperties();
            sphereSoft.Spring = EditorGUILayout.FloatField("Strength of spring", sphereSoft.Spring);
            sphereSoft.Damper = EditorGUILayout.FloatField("Oscillation speed (Damper)", sphereSoft.Damper);
        }
        else
        {
            serializedObject.ApplyModifiedProperties();
            sphereSoft.Spring = EditorGUILayout.FloatField("Strength of spring", sphereSoft.Spring);
            sphereSoft.Damper = EditorGUILayout.FloatField("Oscillation speed (Damper)", sphereSoft.Damper);
        }

    }
}
#endif