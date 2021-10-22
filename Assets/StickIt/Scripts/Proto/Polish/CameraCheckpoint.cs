using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCheckpoint : MonoBehaviour
{
    public RaceDirection direction;
    private bool isEnclenched = false;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision Checkpoint");
        Player otherPlayer = other.GetComponentInParent<Player>();
        if(otherPlayer != null)
        {
            Debug.Log("Player detected");
            CameraFollowFirst camera = Camera.main.GetComponent<CameraFollowFirst>();
            camera.direction = direction;

            if (!isEnclenched)
            {
                camera.GetFirst(other.GetComponentInParent<Player>().gameObject);
            }
        }
    }
}
