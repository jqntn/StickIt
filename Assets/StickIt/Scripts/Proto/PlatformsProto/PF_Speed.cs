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
    // Update is called once per frame
    void Update()
    {
        float angle = transform.rotation.z * Mathf.Deg2Rad;
        _directionPlatform = transform.up.normalized;
    }
    public override void PlatformAction(GameObject p)
    {
        p.GetComponent<Rigidbody>().velocity = newDirection * impulse;
    }
    private void OnCollisionEnter(Collision c)
    {
        Vector3 vel = c.gameObject.GetComponent<Rigidbody>().velocity;
        Vector3 proj = Vector3.Project(vel, transform.right);
        newDirection = proj.normalized;
        Debug.Log(c.gameObject.name);
    }

}
