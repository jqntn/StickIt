using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PF_Bump : InteractionPlatforms
{
    [SerializeField]
    float impulse;
    Vector3 newDirection;

    private void Start()
    {
        _directionPlatform = transform.up.normalized;
    }
    private void Update()
    {

    }
    public override void PlatformAction(GameObject p)
    {
        Debug.DrawRay(transform.position, newDirection * 10, Color.green, 2.0f);
    }


    private void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.TryGetComponent<SpringJoint>(out SpringJoint sj))
        {
            GameObject go = sj.gameObject.transform.parent.GetComponentInChildren<Player>().gameObject;
            Debug.Log(go.name);
            Vector3 pdir = go.GetComponent<Rigidbody>().velocity;
            newDirection = Vector3.Reflect(pdir.normalized, c.contacts[0].normal);
            go.GetComponent<Rigidbody>().velocity = (newDirection * impulse);
        }
        /*if (c.gameObject.CompareTag("Player"))
        {
            Vector3 pdir = c.gameObject.GetComponent<Rigidbody>().velocity;
            newDirection = Vector3.Reflect(pdir.normalized, c.contacts[0].normal);
            c.gameObject.GetComponent<Rigidbody>().velocity = (newDirection * impulse);
        }*/
    }
}
