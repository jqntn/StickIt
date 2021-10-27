using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCheckpoint : MonoBehaviour
{
    private bool hasEnclenched = false;

    private CameraFollowFirst _camera;
    private void Awake()
    {
        _camera = Camera.main.GetComponent<CameraFollowFirst>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (hasEnclenched) { return; }

        Player otherPlayer = other.GetComponentInParent<Player>();
        if(otherPlayer != null)
        {
            Debug.Log("Start Follow First Player");
            _camera.SetCurrentFirst(otherPlayer);
            hasEnclenched = true;
        }
    }
}
