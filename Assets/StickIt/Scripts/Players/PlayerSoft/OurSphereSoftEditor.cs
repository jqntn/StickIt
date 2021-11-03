#if UNITY_EDITOR
    using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OurSphereSoft))]
[CanEditMultipleObjects]
public class OurSphereSoftEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var sphereSoft = target as OurSphereSoft;
        GUILayout.Label("\n \n \n");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
#endif