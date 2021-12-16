using UnityEngine;
public class SpeedPlatfom : Platform
{
    public bool imposeDir;
    public Vector2 dir;
    public float impulseForce;


    public override void Action(Collision c)
    {
        AkSoundEngine.PostEvent("Play_SFX_S_IceSlide", gameObject);
        if (imposeDir) c.transform.GetComponent<Rigidbody>().velocity = dir.normalized * impulseForce;
        else
        {
            Vector2 vel = c.gameObject.GetComponent<Rigidbody>().velocity;
            Vector2 proj = Vector3.Project(vel, -transform.up);
            c.transform.GetComponent<Rigidbody>().velocity = proj.normalized * impulseForce;
        }
    }
    private void OnCollisionExit(Collision c)
    {
        Player player = c.gameObject.GetComponent<Player>();
        if (player != null)
            AkSoundEngine.PostEvent("Stop_SFX_S_IceSlide", gameObject);
    }
}