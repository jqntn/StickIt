using UnityEngine;
public class PF_Speed : Platform
{
    public bool imposeDir;
    public Vector2 dir;
    public float impulseForce;
    public override void Action(Collision c)
    {
        if (imposeDir) c.transform.GetComponent<Rigidbody>().velocity = dir.normalized * impulseForce;
        else
        {
            Vector2 vel = c.gameObject.GetComponent<Rigidbody>().velocity;
            Vector2 proj = Vector3.Project(vel, transform.right);
            c.transform.GetComponent<Rigidbody>().velocity = proj.normalized * impulseForce;
        }
    }
}