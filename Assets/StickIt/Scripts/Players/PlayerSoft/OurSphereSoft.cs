using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class OurSphereSoft : MonoBehaviour
{

    [Header("Bones")]
    GameObject root = null;
    GameObject[] bones;
    public PhysicMaterial matBones;
    [Header("Spring Joint Settings")]
    public bool ConfigurableJoint;
    [Tooltip("Strength of spring")]
    public float Spring = 100f;
    [Tooltip("Higher the value the faster the spring oscillation stops")]
    public float Damper = 0.2f;
    [Header("Other Settings")]
    public int ColliderSizeRoot;
    [HideInInspector]
    public float collSizeRoot;
    public int ColliderSize;
    [HideInInspector]
    public float collSizeBones;
    public float RigidbodyMass = 1f;

    //[Header("Configurable joints settings")]
    private List<Vector3> initBonesPos = new List<Vector3>();

    float maxDist;
    float _ratioMass;
    private void Awake()
    {
        root = this.gameObject;
        root.GetComponent<SphereCollider>().radius = collSizeRoot;
        Softbody.Init(collSizeBones, RigidbodyMass, Spring, Damper, RigidbodyConstraints.FreezeRotation |RigidbodyConstraints.FreezePositionZ, matBones);
        bones = this.GetComponentsInChildren<Transform>().Where(x => x.CompareTag("Bone")).Select(x => x.gameObject).ToArray();

        for (int i = 0; i < bones.Length; i++)
        {
            Softbody.AddCollider(ref bones[i]);
            Vector3 localPosition = bones[i].transform.position - transform.position;
            initBonesPos.Add(localPosition);
            if(ConfigurableJoint)
                Softbody.AddConfJoint(ref bones[i], ref root);
            else
                Softbody.AddSpring(ref bones[i], ref root);
        }
    }

    private void FixedUpdate()
    {
        CheckDistanceBetweenBones();
    }


    public void ReplaceBones(float ratioMass)
    {
        _ratioMass = ratioMass;
        for (int i = 0; i < bones.Length; i++)
        {
            bones[i].transform.position = initBonesPos[i] * (_ratioMass) + transform.position;
        }

        maxDist = (initBonesPos[0] * (ratioMass)).magnitude * 2;
    }


    private void CheckDistanceBetweenBones()
    {
        float dist;
        for (int i = 0; i < bones.Length; i++)
        {
            dist = (bones[i].transform.position - transform.position).magnitude;
            if(dist > maxDist )
            {
                if(Time.timeScale == 1)
                ReplaceBones(_ratioMass);
                break;
            }
        }


    }






}