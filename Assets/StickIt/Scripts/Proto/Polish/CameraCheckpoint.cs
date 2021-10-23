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
        Debug.Log("Collision Checkpoint");
        RacePlayer otherPlayer = other.GetComponentInParent<RacePlayer>();
        if(otherPlayer != null)
        {
            Debug.Log("Player detected");
            CameraFollowFirst camera = Camera.main.GetComponent<CameraFollowFirst>();
            camera.direction = direction;

            if (!hasEnclenched)
            {
                camera.GetFirst(otherPlayer.gameObject);
                hasEnclenched = true;
                camera.currentCheckpoint = number;
            }
        }
    }
}
