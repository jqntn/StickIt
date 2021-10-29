using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CameraBarycenter : MonoBehaviour
{
    [Header("----------- CAMERA MOVEMENT -----------")]
    public bool hasMovement = true;
    public float smoothTime = 0.5f;
    [Header("----------- CAMERA ZOOM    -----------")]
    public bool hasZoom = true;
    [Header("Zoom on Z Axis, take negative value ")]
    public float minZoom = -150.0f;
    public float maxZoom = -100.0f;
    [Header("high value = stronger zoom depending on distance between player")]
    public float zoomLimiter = 50.0f;
    //public float zoomTime = 0.2f;
    //public AnimationCurve zoomCurve;
    [Header("----------- CAMERA BOUNDS ------------")]
    public bool hasCameraBounds = false;
    public Collider2D cameraBounds;
    private MultiplayerManager multiplayerManager;
    [Header("----------- DEBUG --------------------")]
    [SerializeField] private Vector3 velocity = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField] private Vector3 centerPoint = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField] float bounds_X = 0.0f;
    [SerializeField] float bounds_Y = 0.0f;
    [SerializeField] MapManager mapManager;
    private void Awake()
    {
        velocity = new Vector3(0.0f, 0.0f, 0.0f);
        if(hasCameraBounds)
        {
            bounds_X = cameraBounds.bounds.extents.x / 2;
            bounds_Y = cameraBounds.bounds.extents.y / 2;
        }

    }
    private void Start()
    {
        multiplayerManager = MultiplayerManager.instance;
        mapManager = MapManager.instance;
    }

    private void LateUpdate()
    {
        //if (mapManager.isBusy) { return; }
        if (mapManager.isActiveAndEnabled) { return; }
        if (multiplayerManager.players.Count <= 0 && SceneManager.GetActiveScene().buildIndex == 0) { return; }

        if (hasMovement) { FollowPlayers(); }
        if (hasZoom) { Zoom(); }
    }

    private void FollowPlayers()
    {
        centerPoint = GetCentroid();
        if (hasCameraBounds)
        {
            centerPoint.x = Mathf.Clamp(centerPoint.x, -bounds_X, bounds_X);
            centerPoint.y = Mathf.Clamp(centerPoint.y, -bounds_Y, bounds_Y);
        }
        Vector3 newPos = new Vector3(
            centerPoint.x,
            centerPoint.y,
            transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothTime);
    }

    private void Zoom()
    {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / (maxZoom + minZoom));
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
