using UnityEngine;

public class OurSphereSoft : MonoBehaviour
{

    [Header("Bones")]
    public GameObject root = null;
    public GameObject[] bones;
    public PhysicMaterial matBones;
    [Header("Spring Joint Settings")]
    [Tooltip("Strength of spring")]
    public float Spring = 100f;
    [Tooltip("Higher the value the faster the spring oscillation stops")]
    public float Damper = 0.2f;
    [Header("Other Settings")]
    public Softbody.ColliderShape Shape = Softbody.ColliderShape.Sphere;
    public float ColliderSizeRoot = 0.0003f;
    public float ColliderSize = 0.0003f;
    public float RigidbodyMass = 1f;
    public LineRenderer PrefabLine = null;
    public bool ViewLines = true;
    [Header("Player Movements")]
    public P_Mouvement2 playerMovements;

    private void Awake()
    {
        Softbody.Init(Shape, ColliderSize, RigidbodyMass, Spring, Damper, RigidbodyConstraints.FreezeRotation , PrefabLine, ViewLines, matBones);
        for (int i = 0; i < bones.Length; i++)
        {
            Softbody.AddCollider(ref bones[i]);
            Softbody.AddSpring(ref bones[i], ref root);
        }
    }
    private void Start()
    {
        //Softbody.AddCollider(ref root, Softbody.ColliderShape.Sphere, ColliderSizeRoot, 0.5f);
        
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
}