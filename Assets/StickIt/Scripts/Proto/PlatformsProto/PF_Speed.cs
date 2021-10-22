using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PF_Speed : InteractionPlatforms
{
    [SerializeField]
    float impulse;
    Vector3 newDirection;

    public override void PlatformAction(Collision c)
    {
        Vector3 vel = c.gameObject.GetComponent<Rigidbody>().velocity;
        Vector3 proj = Vector3.Project(vel, transform.right);
        newDirection = proj.normalized;
        c.transform.GetComponent<Rigidbody>().velocity = newDirection * impulse;
    }

}
