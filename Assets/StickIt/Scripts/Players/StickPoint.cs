using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickPoint : MonoBehaviour
{

    P_Mouvement2 myPlayerMouvement;
    Vector3 velocityLastFrame;
    Rigidbody rb;
    List<ContactPointSurface> connectedPoints;
    public P_Mouvement2 playerColl;
    private void Start()
    {
        myPlayerMouvement = transform.parent.GetComponentInChildren<P_Mouvement2>();
        connectedPoints = myPlayerMouvement.connectedPoints;
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        velocityLastFrame = rb.velocity;
    }
    // ----- COLLISIONS -----
    #region Collisions
    private void OnCollisionEnter(Collision collision)
    {
        
        switch (collision.transform.tag)
        {
            case "Player":
                /*Player playerCollided = collision.transform.GetComponent<Player>();
                if (myPlayerMouvement.myPlayer.myDatas.id < playerCollided.myDatas.id)
                {
                    if (collision.contactCount > 0)
                        CollisionBetweenPlayers(playerCollided.myMouvementScript2, collision.contacts[0]);
                }*/
                //Debug.Log("Collision bone with parent");
                break;
            case "Bone":
                if (collision.gameObject.transform.parent.GetComponentInChildren<P_Mouvement2>().myPlayer.myDatas.id != myPlayerMouvement.myPlayer.myDatas.id)
                {
                    playerColl = collision.gameObject.transform.parent.GetComponentInChildren<P_Mouvement2>();
                    if(myPlayerMouvement.myPlayer.myDatas.id < playerColl.myPlayer.myDatas.id)
                    {
                        myPlayerMouvement.stickPoints.Add(this);
                        //CA MARCHE FAUT FAIRE L'IMPULSION MAINTENANT
                        //Debug.Log("COLLISION WITH ANOTHER OBJECT");
                    }
                }
                break;
           default:
               /*if (collision.transform.tag != "Untagged") return; // ----- RETURN CONDITION !!!
                #region Collision Untagged
                myPlayerMouvement.currentNumberOfJumps = myPlayerMouvement.maxNumberOfJumps;

                if (myPlayerMouvement.isChargingJump)
                {
                    myPlayerMouvement.EnableDots(true);
                }

                Vector3 contactNormal = collision.contacts[0].normal;
                float dot = Vector2.Dot(contactNormal, velocityLastFrame);
                Vector3 localContactPos = collision.transform.position - collision.contacts[0].point;
                ContactPointSurface contact = new ContactPointSurface(collision.transform, localContactPos, -dot * myPlayerMouvement.attractionMultiplier);
                contact.localPosition.z = transform.position.z;

                connectedPoints.Add(contact);

                myPlayerMouvement.state = P_Mouvement2.STATE.STICK;
                myPlayerMouvement.isAnimCurveSpeed = false;
                myPlayerMouvement.addedVector = Vector2.zero;
                myPlayerMouvement.y_speed = 0;
                myPlayerMouvement.t_speed = 0;
                myPlayerMouvement.hasJumped = false;
                #endregion*/
                break;
        }

    }


   /* private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.tag != "Untagged") return; // ----- RETURN CONDITION !!!
        foreach (ContactPointSurface point in connectedPoints.Where(point => collision.transform == point.transform))
        {
            point.localPosition = collision.transform.position - collision.contacts[0].point;
        }
    }*/
    private void OnCollisionExit(Collision collision)
    {
        //if (collision.transform.tag != "Untagged") return; // ----- RETURN CONDITION !!!
        /*for (int i = 0; i < connectedPoints.Count; i++)
        {
            if (connectedPoints[i].transform == collision.transform)
            {
                connectedPoints.RemoveAt(i);
            }
        }
        if (connectedPoints.Count == 0)
        {
            myPlayerMouvement.state = P_Mouvement2.STATE.AIR;
            if (!myPlayerMouvement.hasJumped)
            {
                myPlayerMouvement.currentNumberOfJumps--;
                myPlayerMouvement.EnableDots(false);
            }
        }*/
        switch (collision.transform.tag)
        {
            case "Bone":
                if (collision.gameObject.transform.parent.GetComponentInChildren<P_Mouvement2>().myPlayer.myDatas.id != myPlayerMouvement.myPlayer.myDatas.id)
                {
                    Player plColl = collision.gameObject.GetComponentInParent<Player>();
                    if (myPlayerMouvement.myPlayer.myDatas.id < plColl.myDatas.id)
                    {
                        myPlayerMouvement.stickPoints.Remove(this);
                    }
                    playerColl = null;
                }
                break;
        }
    }


    /*public void CollisionBetweenPlayers(P_Mouvement2 playerCollided, ContactPoint contact)
    {

        int id = GetComponent<Player>().myDatas.id;

        int ido = playerCollided.GetComponent<Player>().myDatas.id;

        float newVelMagnitudeP1 = playerCollided.velocityLastFrame.magnitude;

        float newVelMagnitudeP2 = velocityLastFrame.magnitude;



        Vector3 newDirP1 = Vector3.Reflect(velocityLastFrame.normalized, contact.normal);

        Vector3 newDirP2 = Vector3.Reflect(playerCollided.velocityLastFrame.normalized, contact.normal);



        rb.velocity = newDirP1 * newVelMagnitudeP1;

        playerCollided.rb.velocity = newDirP2 * newVelMagnitudeP2;



        Vector3 v = Quaternion.Euler(0, 0, 90) * contact.normal;
        Debug.DrawRay(contact.point, v, Color.green);
        //GameObject g = Instantiate(collisionEffect, contact.point, Quaternion.Euler(0, 0, Vector3.Angle(contact.normal, v)));
        //g.GetComponent<ParticleSystemRenderer>().material.color = new Color((MultiplayerManager.instance.materials[id].color.r + MultiplayerManager.instance.materials[ido].color.r) / 2, (MultiplayerManager.instance.materials[id].color.g + MultiplayerManager.instance.materials[ido].color.g) / 2, (MultiplayerManager.instance.materials[id].color.b + MultiplayerManager.instance.materials[ido].color.b) / 2);
        Debug.Break();
        rb.velocity = playerCollided.velocityLastFrame;
        playerCollided.rb.velocity = velocityLastFrame;

         /*#region debug
         print(playerCollided.velocityLastFrame);
         //Last velocities
         Debug.DrawRay(transform.position,  -velocityLastFrame, Color.blue, 3f);
         Debug.DrawRay(playerCollided.transform.position, -playerCollided.velocityLastFrame, Color.gray, 3f);

         New velocities
         Debug.DrawRay(playerCollided.transform.position, newDirP2 * velocityLastFrame.magnitude, Color.yellow, 3f);
         Debug.DrawRay(transform.position, newDirP1 * playerCollided.velocityLastFrame.magnitude, Color.green, 3f);

         // Normal
         Debug.DrawRay(contact.point,contact.point + contact.normal * 100f, Color.red, 3f);
        // Debug.Break();
         #endregion

    }*/
    #endregion
}

public class ContactPointSurface
{
    public Transform transform;
    public Vector3 localPosition;
    public float attractionStrength;

    public ContactPointSurface(Transform transform, Vector3 position, float attractionStrength)
    {
        this.transform = transform;
        this.localPosition = position;
        this.attractionStrength = attractionStrength;
    }

    public ContactPointSurface() { }

}
