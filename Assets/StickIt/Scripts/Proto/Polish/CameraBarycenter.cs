using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBarycenter : MonoBehaviour
{
    [Header("---------- CAMERA MOVEMENT -----------")]
    public float smoothTime = 0.5f;
    [Header("----------- CAMERA ZOOM    -------------")]
    public bool hasZoom = true;
    public float minZoom = -100.0f;
    public float maxZoom = -70.0f;
    public float zoomLimiter = 50.0f;
    private MultiplayerManager multiplayerManager;
    private Camera cam;
    private Vector3 velocity;

    private void Awake()
    {
        cam = Camera.main;
    }
    private void Start()
    {
        multiplayerManager = MultiplayerManager.instance;
    }

    private void LateUpdate()
    {
        if(multiplayerManager.players.Count <= 0) { return; }

        FollowPlayers();
        if (hasZoom) { Zoom(); }
    }

    private void FollowPlayers()
    {
        Vector3 centerPoint = GetCentroid();
        Vector3 newPos = new Vector3(
            centerPoint.x,
            centerPoint.y,
            transform.position.z);

        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothTime);
    }

    private void Zoom()
    {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
        transform.position = new Vector3(
            transform.position.x, 
            transform.position.y, 
            Mathf.Lerp(transform.position.z, newZoom, Time.deltaTime));
    }

    private Vector3 GetCentroid()
    {
        Vector3 center = new Vector3(0, 0, 0);
        for (int i = 0; i < multiplayerManager.players.Count; i++)
        {
            center += multiplayerManager.players[i].transform.position;
        }
        center /= multiplayerManager.players.Count;

        //Debug
        float val = 0;
        List<Player> players = multiplayerManager.players;
        foreach(Player player in players)
        {
            Debug.DrawLine(player.transform.position, center, Color.red + new Color(-val, val, 0));
            val += 0.25f;
        }

        return center;
    }

    private float GetGreatestDistance()
    {
        List<Player> players = multiplayerManager.players;
        var bounds = new Bounds(players[0].transform.position, Vector3.zero);
        for (int i = 0; i < players.Count; i++)
        {
            bounds.Encapsulate(players[i].transform.position);
        }

        return bounds.size.x;
    }
}
