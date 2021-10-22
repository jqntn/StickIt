using UnityEngine;

public class CameraMultipleTarget : MonoBehaviour
{
    public Vector3 offset;
    public float smoothTime = 0.5f;
    public float minZoom = 10.0f;
    public float maxZoom = 1.0f;
    public float zoomLimiter = 20.0f;

    private Camera cam;
    private Vector3 velocity;
    private MultiplayerManager multiManager;
    private void Awake()
    {
        cam = Camera.main;
    }

    private void Start()
    {
        multiManager = MultiplayerManager.instance;
    }
    private void LateUpdate()
    {
        if (multiManager.players.Count == 0) { return; }

        Move();
        //Zoom();
    }

    private void Move()
    {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPos = centerPoint + offset;
        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothTime);
    }

    private void Zoom()
    {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);
    }

    private float GetGreatestDistance()
    {
        var bounds = new Bounds(multiManager.players[0].transform.position, Vector3.zero);
        for(int i = 0; i < multiManager.players.Count; i++) {
            bounds.Encapsulate(multiManager.players[i].transform.position);
        }

        return bounds.size.x;
    }
    private Vector3 GetCenterPoint()
    {
        if (multiManager.players.Count == 1)
        {
            return multiManager.players[0].transform.position;
        }

        var bounds = new Bounds(multiManager.players[0].transform.position, Vector3.zero);
        for (int i = 0; i < multiManager.players.Count; i++)
        {
            bounds.Encapsulate(multiManager.players[0].transform.position);
        }

        return bounds.center;
    }
}
