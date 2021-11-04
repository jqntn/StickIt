using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class CameraState : MonoBehaviour
{
    [Header("------- Data -------")]
    public CameraType type = CameraType.BARYCENTER;
    public Collider bounds;

    [Header("------ Animation ------")]
    public float animTime = 0.5f;

    [Header("------- Move -------")]
    public bool canMove = true;
    public bool freezeX = false;
    public bool freezeY = false;

    [Header("------- Zoom -------")]
    public bool canZoom = true;
    public float maxOut_Z = -110.0f;
    public float maxIn_Z = -70.0f;
    public float zoomOutMargin = -5.0f;
    public float zoomInMargin = 10.0f;
    public float zoomOutValue = -20.0f;
    public float zoomInValue = -10.0f;

    [Header("----- End Animation -----")]
    public bool hasZoomOutAtEnd = true;
    public bool hasCenterCamera = true;
    //public bool hasFreeRoaming = false;
    //public int randomRadius = 5;
    //public float roamingTime = 3.0f;

    [Header("----- Prototype -----")]
    public List<CameraData> datas = new List<CameraData>();

    [Header("----- Debug -----")]
    [SerializeField] protected Camera cam = null;
    [SerializeField] protected MapManager mapManager = null;
    [SerializeField] protected MultiplayerManager multiplayerManager = null;
    [SerializeField] protected List<Player> playerList = new List<Player>();
    [SerializeField] protected Vector3 positionToGoTo = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField] protected Vector3 velocity = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField] protected float bounds_X = 0.0f;
    [SerializeField] protected float bounds_Y = 0.0f;
    [SerializeField] protected float min_bounds_X = 0.0f;
    [SerializeField] protected float max_bounds_X = 0.0f;
    [SerializeField] protected float min_bounds_Y = 0.0f;
    [SerializeField] protected float max_bounds_Y = 0.0f;
    [SerializeField] protected float frustumHeight = 0.0f;
    [SerializeField] protected float frustumWidth = 0.0f;
    protected virtual void Start()
    {
        cam = Camera.main;
        mapManager = MapManager.instance;
        multiplayerManager = MultiplayerManager.instance;
        if(multiplayerManager != null)
        {
            playerList = multiplayerManager.players;
        }

        if (bounds == null)
        {
            Debug.Log("Please add camera bounds to CameraBarycenter");
        }
        else
        {
            bounds_X = bounds.bounds.extents.x;
            bounds_Y = bounds.bounds.extents.y;
            min_bounds_X = bounds.transform.position.x - bounds_X / 2.0f;
            max_bounds_X = bounds.transform.position.x + bounds_X / 2.0f;
            min_bounds_Y = bounds.transform.position.y - bounds_Y / 2.0f;
            max_bounds_Y = bounds.transform.position.y + bounds_Y / 2.0f;
        }

        CalculateFrustum();
    }

    protected virtual void Update()
    {
        // Protections
        if (SceneManager.GetActiveScene().name == "0_MenuSelection" || SceneManager.GetActiveScene().buildIndex == 0) { return; }
        if (multiplayerManager == null) { return; }
        if (mapManager == null) { return; }
        if (cam == null) { return; }
        if (mapManager.isBusy) { return; }
        if (playerList.Count == 0) { return; }

        CalculateFrustum();

    }

    protected virtual void LateUpdate()
    {
        // Protections
        if (SceneManager.GetActiveScene().name == "0_MenuSelection" || SceneManager.GetActiveScene().buildIndex == 0) { return; }
        if (multiplayerManager == null) { return; }
        if (mapManager == null) { return; }
        if (cam == null) { return; }
        if (mapManager.isBusy) { return; }
        if (playerList.Count == 0) { return; }

        //if (canZoom) { UpdateZoom(); }
        UpdateCamera();
    }

    protected virtual void UpdateCamera()
    {
        transform.parent.position = Vector3.SmoothDamp(transform.parent.position, positionToGoTo, ref velocity, animTime);
    }

    private void UpdateZoom()
    {
        // Zoom Out
        // Only 1 player have to be oustide zoomOut box to zoom out
        bool insideZoomOutBox = false;
        foreach (Player player in playerList)
        {
            float player_X = player.transform.position.x;
            float player_Y = player.transform.position.y;
            float zoomOutWidth = (frustumWidth + zoomOutMargin) / 2.0f;
            float zoomOutHeight = (frustumHeight + zoomOutMargin) / 2.0f;
            float minZoomOut_X = transform.position.x - zoomOutWidth;
            float maxZoomOut_X = transform.position.x + zoomOutWidth;
            float minZoomOut_Y = transform.position.y - zoomOutHeight;
            float maxZoomOut_Y = transform.position.y + zoomOutHeight;

            insideZoomOutBox =
                player_X > minZoomOut_X && player_X < maxZoomOut_X &&
                player_Y > minZoomOut_Y && player_Y < maxZoomOut_Y;

            if (!insideZoomOutBox) { break; }
        }

        // Get New Zoom if outside of zoomOut box
        if (!insideZoomOutBox)
        {
            positionToGoTo.z = Mathf.Clamp(transform.position.z + zoomOutValue, maxOut_Z, maxIn_Z);
            return;
        }

        // Zoom In
        // All player have to be inside zoomIn Box to zoom In
        bool insideZoomInBox = true;
        foreach (Player player in playerList)
        {
            float player_X = player.transform.position.x;
            float player_Y = player.transform.position.y;
            float zoomInWidth = (frustumWidth + zoomInMargin) / 2.0f;
            float zoomInHeight = (frustumHeight + zoomInMargin) / 2.0f;
            float minZoomIn_X = transform.position.x - zoomInWidth;
            float maxZoomIn_X = transform.position.x + zoomInWidth;
            float minZoomIn_Y = transform.position.y - zoomInHeight;
            float maxZoomIn_Y = transform.position.y + zoomInHeight;

            insideZoomInBox =
                player_X > minZoomIn_X && player_X < maxZoomIn_X &&
                player_Y > minZoomIn_Y && player_Y < maxZoomIn_Y;

            if (!insideZoomInBox) { break; }
        }

        // Get New Zoom if player inside zoomIn box
        if (insideZoomInBox)
        {
            positionToGoTo.z = Mathf.Clamp(transform.position.z + zoomInValue, maxOut_Z, maxIn_Z);
        }
    }

    protected void CalculateFrustum()
    {
        float distance = -transform.parent.position.z;
        frustumHeight = 2.0f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        frustumWidth = frustumHeight * cam.aspect;
    }

    #region Public Method
    public CameraType GetCameraType()
    {
        return type;
    }
    #endregion

    #region Debug
    void OnDrawGizmosSelected()
    {
        // Draw Camera Viewport
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.parent.position, new Vector3(frustumWidth, frustumHeight, 1));

        // Draw Camera Zoom Boxes
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.parent.position, new Vector3(frustumWidth + zoomOutMargin, frustumHeight + zoomOutMargin, 1));
        Gizmos.DrawWireCube(transform.parent.position, new Vector3(frustumWidth + zoomInMargin, frustumHeight + zoomInMargin, 1));

        // Draw Camera Bounds
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(bounds.transform.position, new Vector3(bounds_X * 2.0f, bounds_Y * 2.0f, 1));

        // Draw Camera Movement Bounds
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(bounds.transform.position, new Vector3(bounds_X, bounds_Y, 1));

    }
    #endregion
}
