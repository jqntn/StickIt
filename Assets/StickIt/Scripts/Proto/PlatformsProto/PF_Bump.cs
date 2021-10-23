using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PF_Bump : InteractionPlatforms
{
    [SerializeField]
    float impulse;
    Vector3 newDirection;
    public override void PlatformAction(Collision c)
    {
        Debug.DrawRay(transform.position, newDirection * 10, Color.green, 2.0f);
        Vector3 pdir = c.gameObject.GetComponent<Rigidbody>().velocity;
        newDirection = Vector3.Reflect(pdir.normalized, c.contacts[0].normal);
        c.gameObject.GetComponent<Rigidbody>().velocity = (newDirection * impulse);

    }
}
