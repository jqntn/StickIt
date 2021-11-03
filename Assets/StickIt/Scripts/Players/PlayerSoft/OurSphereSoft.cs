using UnityEngine;

public class OurSphereSoft : MonoBehaviour
{

    [Header("Bones")]
    public GameObject root = null;
    public GameObject[] bones;
    public PhysicMaterial matBones;
    [Header("Spring Joint Settings")]
    public bool ConfigurableJoint;
    [Tooltip("Strength of spring")]
    public float Spring = 100f;
    [Tooltip("Higher the value the faster the spring oscillation stops")]
    public float Damper = 0.2f;
    [Header("Other Settings")]
    public float ColliderSizeRoot = 0.0003f;
    public float ColliderSize = 0.0003f;
    public float RigidbodyMass = 1f;
    public LineRenderer PrefabLine = null;
    public bool ViewLines = false;

    //[Header("Configurable joints settings")]

    private void Awake()
    {
        Softbody.Init(ColliderSize, RigidbodyMass, Spring, Damper, RigidbodyConstraints.FreezeRotation |RigidbodyConstraints.FreezePositionZ , PrefabLine, ViewLines, matBones);
        for (int i = 0; i < bones.Length; i++)
        {
            Softbody.AddCollider(ref bones[i]);
            if(ConfigurableJoint)
                Softbody.AddConfJoint(ref bones[i], ref root);
            else
                Softbody.AddSpring(ref bones[i], ref root);
        }
    }

}