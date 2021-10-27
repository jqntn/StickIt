using UnityEngine;

public class OurSphereSoft : MonoBehaviour
{

    [Header("Bones")]
    public GameObject[] bones;
    public GameObject root = null;
    public PhysicMaterial matBones;
    /*
    public GameObject x = null;
    public GameObject x2 = null;
    public GameObject y = null;
    public GameObject y2 = null;
    public GameObject z = null;
    public GameObject z2 = null;
    public GameObject xyz = null;
    public GameObject x2yz = null;
    public GameObject x2y2z = null;
    public GameObject x2y2z2 = null;
    public GameObject xy2z = null;
    public GameObject xy2z2 = null;
    public GameObject xyz2 = null;
    public GameObject x2yz2 = null;*/
    [Header("Spring Joint Settings")]
    [Tooltip("Strength of spring")]
    public float Spring = 100f;
    [Tooltip("Higher the value the faster the spring oscillation stops")]
    public float Damper = 0.2f;
    [Header("Other Settings")]
    public Softbody.ColliderShape Shape = Softbody.ColliderShape.Sphere;
    public float ColliderSize = 0.0003f;
    public float ColliderSizeRoot = 0.0003f;
    public float RigidbodyMass = 1f;
    public LineRenderer PrefabLine = null;
    public bool ViewLines = true;

    private void Awake()
    {
        Softbody.Init(Shape, ColliderSize, RigidbodyMass, Spring, Damper, RigidbodyConstraints.FreezeRotation , PrefabLine, ViewLines, matBones);
    }
    private void Start()
    {
        Softbody.AddCollider(ref root, Softbody.ColliderShape.Sphere, ColliderSizeRoot, 0.5f);
        GameObject boneAfter;
        boneAfter = bones[1];
        for (int i = 0; i < bones.Length;i++)
        {
            Softbody.AddCollider(ref bones[i]);
            Softbody.AddSpring(ref bones[i], ref root);
        }

        /*Softbody.AddCollider(ref x);
        Softbody.AddCollider(ref x2);
        Softbody.AddCollider(ref y);
        Softbody.AddCollider(ref y2);
        Softbody.AddCollider(ref z);
        Softbody.AddCollider(ref z2);

        Softbody.AddCollider(ref xyz);
        Softbody.AddCollider(ref x2yz);
        Softbody.AddCollider(ref x2y2z);
        Softbody.AddCollider(ref x2y2z2);
        Softbody.AddCollider(ref xy2z);
        Softbody.AddCollider(ref xy2z2);
        Softbody.AddCollider(ref xyz2);
        Softbody.AddCollider(ref x2yz2);

        Softbody.AddSpring(ref x, ref root);
        Softbody.AddSpring(ref x2, ref root);
        Softbody.AddSpring(ref y, ref root);
        Softbody.AddSpring(ref y2, ref root);
        Softbody.AddSpring(ref z, ref root);
        Softbody.AddSpring(ref z2, ref root);

        Softbody.AddSpring(ref xyz, ref root);
        Softbody.AddSpring(ref x2yz, ref root);
        Softbody.AddSpring(ref x2y2z, ref root);
        Softbody.AddSpring(ref x2y2z2, ref root);
        Softbody.AddSpring(ref xy2z, ref root);
        Softbody.AddSpring(ref xy2z2, ref root);
        Softbody.AddSpring(ref xyz2, ref root);
        Softbody.AddSpring(ref x2yz2, ref root);*/
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
}