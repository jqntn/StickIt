using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCheckpoint : MonoBehaviour
{
    public RaceDirection direction;
    public int number = 0;
    private bool hasEnclenched = false;
    private void OnTriggerEnter(Collider other)
    {
        if (hasEnclenched) { return; }

        Debug.Log("Checkpoint First Trigger");

        Player otherPlayer = other.GetComponentInParent<Player>();
        if(otherPlayer != null)
        {
            Debug.Log("Player First");
            
            hasEnclenched = true;
            //Debug.Log("Player detected");
            //CameraFollowFirst camera = Camera.main.GetComponent<CameraFollowFirst>();
            //camera.direction = direction;

            //if (!hasEnclenched)
            //{
            //    camera.GetFirst(otherPlayer.gameObject);
            //    hasEnclenched = true;
            //    camera.currentCheckpoint = number;
            //}
        }
    }
}
