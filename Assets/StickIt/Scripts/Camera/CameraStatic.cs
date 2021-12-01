using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStatic : CameraState
{
    private bool isPlaying = false;
    
    [SerializeField] private float timer = 0.0f;
    private void OnEnable()
    {
        base.Awake();
        GameEvents.OnSwitchCamera.AddListener(SaveBounds);
    }

    private void SaveBounds(CameraType type)
    {
        Vector2 boundsSavePos = bounds.transform.position;
        if (canMove) { positionToGoTo = boundsSavePos; }
        if (canZoom) { positionToGoTo.z = maxOut_Z; }
    }
    protected override void Update()
    {
        base.Update();
        
        if(transform.parent.position != positionToGoTo)
        {
            transform.parent.position += new Vector3(0, 0, 0);
        }
    }
}
