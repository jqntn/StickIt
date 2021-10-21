using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraShakeScript))]
public class CameraShakeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CameraShakeScript script = (CameraShakeScript)target;
        if (GUILayout.Button("Shake Camera")){
            script.ShakeCamera();
        }
        if (GUILayout.Button("PlayOnCameraShake"))
        {
            script.PlayOnCameraShake();
        }
    }
}
