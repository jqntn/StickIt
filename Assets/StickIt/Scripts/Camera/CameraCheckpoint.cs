using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCheckpoint : MonoBehaviour
{
    private bool hasEnclenched = false;

    //private CameraFollowFirst _camera;
    private CameraSpeedRunner _camera;
    private void Awake()
    {
        //_camera = Camera.main.GetComponent<CameraFollowFirst>();
        _camera = Camera.main.GetComponent<CameraSpeedRunner>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (hasEnclenched) { return; }

        Player otherPlayer = other.GetComponentInParent<Player>();
        if(otherPlayer != null)
        {
            //_camera.SetCurrentFirst(otherPlayer);
            hasEnclenched = true;
        }
    }
}
