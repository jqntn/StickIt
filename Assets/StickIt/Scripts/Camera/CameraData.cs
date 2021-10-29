using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New CameraData", menuName = "ScriptableObjects/CameraData")]
public class CameraData : ScriptableObject
{
    [Header("------ Move ------")]
    public float moveTime = 0.2f;
    public float distanceBeforeBorder = 0.0f;
    public bool freezeX = false;
    public bool freezeY = false;
    public bool hasFollowOnlyPlayer = false;
    public bool hasFreeRoaming = false;
    public int randomRadius = 5;
    public float roamingTime = 3.0f;

    [Header("------ Zoom ------")]
    public float maxOut_Z = -110.0f;
    public float maxIn_Z = -70.0f;
    public float zoomOutMargin = -5.0f;
    public float zoomInMargin = -10.0f;
    public float zoomOutSpeed = 20.0f;
    public float zoomInSpeed = 10.0f;
    public float zoomTime = 0.2f;
    public bool hasZoomOutAtEnd = true;

    [Header("------ Death ------")]
    public float deathMargin = 10.0f;
    public float timeBeforeDeath = 0.2f;
}
