using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New CameraData", menuName = "ScriptableObjects/CameraData")]
public class CameraData : ScriptableObject
{
    [Tooltip("Camera Max Zoom In Value\nShould Always be negative and higher than max Zoom Out")]
    public float maxZoomIn = -100.0f;
}
