using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickPoint : MonoBehaviour
{

    P_Mouvement2 myPlayerMouvement;
    public List<ContactPointSurface> connectedPoints = new List<ContactPointSurface>();

    //// ----- COLLISIONS -----
    //#region Collisions
    //private void OnCollisionEnter(Collision collision)
    //{

    //    switch (collision.transform.tag)
    //    {
    //        case "Player":
    //            Player playerCollided = collision.transform.GetComponent<Player>();
    //            if (myPlayerMouvement.myPlayer.myDatas.id < playerCollided.myDatas.id)
    //            {
    //                if (collision.contactCount > 0)
    //                    CollisionBetweenPlayers(playerCollided.myMouvementScript2, collision.contacts[0]);
    //            }
    //            break;

    //        default:
    //            if (collision.transform.tag != "Untagged") return; // ----- RETURN CONDITION !!!
    //            #region Collision Untagged
    //            currentNumberOfJumps = maxNumberOfJumps;

    //            if (isChargingJump)
    //            {
    //                EnableDots(true);
    //            }

    //            Vector3 contactNormal = collision.contacts[0].normal;
    //            float dot = Vector2.Dot(contactNormal, velocityLastFrame);
    //            Vector3 localContactPos = collision.transform.position - collision.contacts[0].point;
    //            ContactPointSurface contact = new ContactPointSurface(collision.transform, localContactPos, -dot * attractionMultiplier);
    //            contact.localPosition.z = transform.position.z;

    //            connectedPoints.Add(contact);

    //            state = STATE.STICK;
    //            isAnimCurveSpeed = false;
    //            addedVector = Vector2.zero;
    //            y_speed = 0;
    //            t_speed = 0;
    //            hasJumped = false;
    //            #endregion
    //            break;
    //    }

    //}


    //private void OnCollisionStay(Collision collision)
    //{
    //    if (collision.transform.tag != "Untagged") return; // ----- RETURN CONDITION !!!
    //    foreach (ContactPointSurface point in connectedPoints.Where(point => collision.transform == point.transform))
    //    {
    //        point.localPosition = collision.transform.position - collision.contacts[0].point;
    //    }
    //}
    //private void OnCollisionExit(Collision collision)
    //{
    //    if (collision.transform.tag != "Untagged") return; // ----- RETURN CONDITION !!!
    //    for (int i = 0; i < connectedPoints.Count; i++)
    //    {
    //        if (connectedPoints[i].transform == collision.transform)
    //        {
    //            connectedPoints.RemoveAt(i);
    //        }
    //    }
    //    if (connectedPoints.Count == 0)
    //    {
    //        state = STATE.AIR;
    //        if (!hasJumped)
    //        {
    //            currentNumberOfJumps--;
    //            EnableDots(false);
    //        }
    //    }
    //}


    //public void CollisionBetweenPlayers(P_Mouvement2 playerCollided, ContactPoint contact)
    //{

    //    int id = GetComponent<Player>().myDatas.id;

    //    int ido = playerCollided.GetComponent<Player>().myDatas.id;

    //    //float newVelMagnitudeP1 = playerCollided.velocityLastFrame.magnitude;

    //    //float newVelMagnitudeP2 = velocityLastFrame.magnitude;



    //    //Vector3 newDirP1 = Vector3.Reflect(velocityLastFrame.normalized, contact.normal);

    //    //Vector3 newDirP2 = Vector3.Reflect(playerCollided.velocityLastFrame.normalized, contact.normal);



    //    //rb.velocity = newDirP1 * newVelMagnitudeP1;

    //    //playerCollided.rb.velocity = newDirP2 * newVelMagnitudeP2;



    //    Vector3 v = Quaternion.Euler(0, 0, 90) * contact.normal;
    //    Debug.DrawRay(contact.point, v, Color.green);
    //    GameObject g = Instantiate(collisionEffect, contact.point, Quaternion.Euler(0, 0, Vector3.Angle(contact.normal, v)));
    //    g.GetComponent<ParticleSystemRenderer>().material.color = new Color((MultiplayerManager.instance.materials[id].color.r + MultiplayerManager.instance.materials[ido].color.r) / 2, (MultiplayerManager.instance.materials[id].color.g + MultiplayerManager.instance.materials[ido].color.g) / 2, (MultiplayerManager.instance.materials[id].color.b + MultiplayerManager.instance.materials[ido].color.b) / 2);
    //    //Debug.Break();
    //    rb.velocity = playerCollided.velocityLastFrame;
    //    playerCollided.rb.velocity = velocityLastFrame;

    //    /* #region debug
    //     print(playerCollided.velocityLastFrame);
    //     //Last velocities
    //     Debug.DrawRay(transform.position,  -velocityLastFrame, Color.blue, 3f);
    //     Debug.DrawRay(playerCollided.transform.position, -playerCollided.velocityLastFrame, Color.gray, 3f);

    //     //New velocities
    //     //Debug.DrawRay(playerCollided.transform.position, newDirP2 * velocityLastFrame.magnitude, Color.yellow, 3f);
    //     //Debug.DrawRay(transform.position, newDirP1 * playerCollided.velocityLastFrame.magnitude, Color.green, 3f);

    //     // Normal
    //     Debug.DrawRay(contact.point,contact.point + contact.normal * 100f, Color.red, 3f);
    //    // Debug.Break();
    //     #endregion*/

    //}



    //#endregion



    private void OnCollisionEnter(Collision collision)
    {
        

    }

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
