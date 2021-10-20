using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PF_Speed : InteractionPlatforms
{
    [SerializeField]
    bool instant;
    [SerializeField]
    float impulse;
    Vector3 newDirection;
    private void Start()
    {
        _directionPlatform = transform.up.normalized;
    }

    public override void PlatformAction(GameObject p)
    {
        p.GetComponent<Rigidbody>().velocity = newDirection * impulse;
    }
    private void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.TryGetComponent<SpringJoint>(out SpringJoint sj))
        {
            GameObject go = sj.gameObject.transform.parent.GetComponentInChildren<Player>().gameObject;
            Vector3 vel = go.GetComponent<Rigidbody>().velocity;
            Vector3 proj = Vector3.Project(vel, transform.right);
            newDirection = proj.normalized;
            Debug.Log(go.name);
        }
        if (c.gameObject.CompareTag("Player"))
        {
            Vector3 vel = c.gameObject.GetComponent<Rigidbody>().velocity;
            Vector3 proj = Vector3.Project(vel, transform.right);
            newDirection = proj.normalized;
            Debug.Log(c.gameObject.name);
        }
    }

}
