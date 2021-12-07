using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class CameraState : MonoBehaviour
{
    [Header("------- Data -------")]
    public CameraType type = CameraType.BARYCENTER;
    public bool autoBounds = true;
    [Min(0.1f)]
    public float factorOffset = 1.0f;
    public Collider camBounds;
    public bool autoOffset = true;
    public Vector2 posOffset = new Vector2(0.0f, 0.0f);

    [Header("------- Move -------")]
    public bool canMove = true;
    public float moveTime = 0.5f;
    public bool freezeX = false;
    public bool freezeY = false;

    [Header("------- Zoom -------")]
    public bool canZoom = true;
    public float zoomTime = 1.0f;
    public bool autoMaxOut_Z = false;
    public float maxOut_Z = -110.0f;
    public float maxIn_Z = -70.0f;
    public float zoomOutSpeed = 10.0f;
    public float zoomInSpeed = 10.0f;
    public float zoomOutMargin = 2.0f;
    public float zoomOutMargin2 = 5.0f;
    public float zoomInMargin = 5.0f;
    public bool canClampZoom = false;

    [Header("----- Debug -----")]
    [SerializeField] protected Camera cam = null;
    [SerializeField] protected MapManager mapManager = null;
    [SerializeField] protected MultiplayerManager multiplayerManager = null;
    [SerializeField] protected List<Player> playerList = new List<Player>();
    [SerializeField] protected Vector3 positionToGoTo = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField] protected Vector3 moveVelocity = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField] protected Vector3 zoomVelocity = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField] protected Vector3 barycenter = new Vector3(0.0f, 0.0f, 0.0f);
    public bool hasBounds = false;
    [SerializeField] protected Vector2 bounds_pos = new Vector3(0.0f, 0.0f);
    [SerializeField] protected Vector2 bounds_dimension = new Vector2(0.0f, 0.0f);
    [SerializeField] protected Vector2 min_bounds = new Vector2(0.0f, 0.0f);
    [SerializeField] protected Vector2 max_bounds = new Vector2(0.0f, 0.0f);
    [SerializeField] protected Vector2 min_moveBounds = new Vector2(0.0f, 0.0f);
    [SerializeField] protected Vector2 max_moveBounds = new Vector2(0.0f, 0.0f);
    [SerializeField] protected Vector2 frustum_dimension = new Vector2(0.0f, 0.0f);
    [SerializeField] protected Vector2 min_viewport = new Vector2(0.0f, 0.0f);
    [SerializeField] protected Vector2 max_viewport = new Vector2(0.0f, 0.0f);
    [SerializeField] protected Vector2 min_zoomOutBounds = new Vector2(0.0f, 0.0f);
    [SerializeField] protected Vector2 max_zoomOutBounds = new Vector2(0.0f, 0.0f);
    [SerializeField] protected Vector2 playerBounds = new Vector2(0.0f, 0.0f);
    [SerializeField] protected Vector2 min_playerBounds = new Vector2(0.0f, 0.0f);
    [SerializeField] protected Vector2 max_playerBounds = new Vector2(0.0f, 0.0f);

    protected virtual void Awake()
    {
        GameEvents.OnSwitchCamera.AddListener(ResetCamera);
        if (SceneManager.GetActiveScene().name == "1_MenuSelection" || SceneManager.GetActiveScene().buildIndex == 0) { return; }
    }

    protected virtual void Start()
    {
        cam = Camera.main;
        mapManager = MapManager.instance;
        multiplayerManager = MultiplayerManager.instance;
        if(multiplayerManager != null) { playerList = multiplayerManager.players; }

        if (SceneManager.GetActiveScene().name == "1_MenuSelection" || SceneManager.GetActiveScene().buildIndex == 0) { return; }
        ResetCamera();
    }

    protected virtual void Update()
    {
        // Protections
        if (SceneManager.GetActiveScene().name == "1_MenuSelection" || SceneManager.GetActiveScene().buildIndex == 0) { return; }
        if (multiplayerManager == null) { return; }
        if (mapManager == null) { return; }
        if (cam == null) { return; }
        if (mapManager.isBusy) { return; }
        if (playerList.Count == 0) { return; }

        SearchMaxOut_Z();
        SearchPosOffset();
        UpdateFrustum();
        UpdateMoveBounds();
        UpdatePlayersBounds();
    }

    protected virtual void LateUpdate()
    {
        // Protections
        if (SceneManager.GetActiveScene().name == "1_MenuSelection" || SceneManager.GetActiveScene().buildIndex == 0) { return; }
        if (multiplayerManager == null) { return; }
        if (mapManager == null) { return; }
        if (cam == null) { return; }
        if (mapManager.isBusy) { return; }
        if (playerList.Count == 0) { return; }

        UpdateCamera();
    }

    //<summary>
    // Make The camera Move and Zoom
    //<summary>
    protected virtual void UpdateCamera()
    {
        // Move Camera
        Vector3 newPos = new Vector3(
            positionToGoTo.x + posOffset.x,
            positionToGoTo.y + posOffset.y,
            transform.parent.position.z);
        transform.parent.position = Vector3.SmoothDamp(transform.parent.position, newPos, ref moveVelocity, moveTime);

        // if zooming out and is touching border > return and wait to move first
        bool isTouchingBorder =
            min_viewport.x <= min_bounds.x || max_viewport.x >= max_bounds.x;
        if (canClampZoom && positionToGoTo.z < transform.parent.position.z && isTouchingBorder) { return; }

        // Zoom Camera
        Vector3 newZoom = new Vector3(
            transform.parent.position.x,
            transform.parent.position.y,
            positionToGoTo.z);
        transform.parent.position = Vector3.SmoothDamp(transform.parent.position, newZoom, ref zoomVelocity, zoomTime);
      
    }

    //<summary>
    // Viewport dimension changing depending of the Zoom Value
    //<summary>
    protected void UpdateFrustum()
    {
        float distance = -transform.parent.position.z;
        frustum_dimension.y = 2.0f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        frustum_dimension.x = frustum_dimension.y * cam.aspect;

        // frustum update
        float offsetX = frustum_dimension.x / 2.0f;
        float offsetY = frustum_dimension.y / 2.0f;
        min_viewport.x = transform.parent.position.x - posOffset.x - offsetX;
        max_viewport.x = transform.parent.position.x - posOffset.x + offsetX;
        min_viewport.y = transform.parent.position.y - posOffset.y - offsetY;
        max_viewport.y = transform.parent.position.y - posOffset.y + offsetY;

        // camera zoom out margin update
        min_zoomOutBounds.x = min_viewport.x + zoomOutMargin2;
        min_zoomOutBounds.y = min_viewport.y + zoomOutMargin2;
        max_zoomOutBounds.x = max_viewport.x - zoomOutMargin2;
        max_zoomOutBounds.y = max_viewport.y - zoomOutMargin2;
    }

    //<summary>
    // Camera Movement clamping depending of zoom
    //<summary>
    protected void UpdateMoveBounds()
    {
        float offsetX = (bounds_dimension.x - frustum_dimension.x /*- posOffset.x*/) / 2.0f;
        float offsetY = (bounds_dimension.y - frustum_dimension.y /*- posOffset.y*/) / 2.0f;
        max_moveBounds.x = bounds_pos.x + offsetX;
        min_moveBounds.x = bounds_pos.x - offsetX;
        min_moveBounds.y = bounds_pos.y - offsetY;
        max_moveBounds.y = bounds_pos.y + offsetY;
    }

    //<summary>
    // Player bounds are the encapsulation of all the player to calculate how to zoom in
    //<summary>
    protected void UpdatePlayersBounds()
    {
        Bounds playersBounds = new Bounds(playerList[0].transform.position, Vector3.zero);

        foreach (Player player in playerList)
        {
            playersBounds.Encapsulate(player.transform.position);
        }

        playerBounds.x = playersBounds.size.x;
        playerBounds.y = playersBounds.size.y;

        float offsetX = playerBounds.x;
        min_playerBounds.x = barycenter.x - offsetX;
        max_playerBounds.x = barycenter.x + offsetX;
    }

    protected virtual void UpdateZoom() {
        // Zoom Out
        float min_zoomOut_X = min_playerBounds.x - zoomOutMargin;
        float max_zoomOut_X = max_playerBounds.x + zoomOutMargin;
        float min_zoomOut_Y = min_playerBounds.y - zoomOutMargin;
        float max_zoomOut_Y = max_playerBounds.y + zoomOutMargin;
        // If Viewport is too small compare to external Bounds players > Zoom out 
        bool canZoomOut = (( (min_viewport.x >= min_zoomOut_X || max_viewport.x <= max_zoomOut_X)
                           //|| min_viewport.y >= min_zoomOut_Y || max_viewport.y <= max_zoomOut_Y)
                        && -Mathf.Floor(-transform.parent.position.z) > maxOut_Z)
                        );

        int count = 0;
        // If Player is touching zoomOutMargin > Zoom Out
        foreach (Player player in playerList)
        {
            count++;
            Vector2 playerPos = player.transform.position;
            if ((playerPos.x <= min_zoomOutBounds.x || playerPos.x >= max_zoomOutBounds.x
                || playerPos.y <= min_zoomOutBounds.y || playerPos.y >= max_zoomOutBounds.y)
                && -Mathf.Floor(-transform.parent.position.z) > maxOut_Z)
            {
                canZoomOut = true;
                break;
            }
        }

        // Zoom Out
        if (canZoomOut)
        {
            positionToGoTo.z = Mathf.Clamp(transform.parent.position.z - zoomOutSpeed, maxOut_Z, maxIn_Z);
            return;
        }

        // Zoom In
        float min_zoomIn_X = min_playerBounds.x - zoomInMargin;
        float max_zoomIn_X = max_playerBounds.x + zoomInMargin;

        // If Viewport is too big compare to bounds players > Zoom In
        bool canZoomIn = (min_viewport.x <= min_zoomIn_X || max_viewport.x >= max_zoomIn_X)
                         && -Mathf.Floor(-transform.parent.position.z) < maxIn_Z;
        if (canZoomIn)
        {
            positionToGoTo.z = Mathf.Clamp(transform.parent.position.z + zoomInSpeed, maxOut_Z, maxIn_Z);
        }
    }

    private void SearchMaxOut_Z()
    {
        float maxFrustumHeight = 0.0f;
        if (bounds_dimension.x < bounds_dimension.y)
        {
            float maxFrustumWidth = bounds_dimension.x;
            maxFrustumHeight = maxFrustumWidth / cam.aspect;
        }
        else
        {
            maxFrustumHeight = bounds_dimension.y;
        }
        float distance = -maxFrustumHeight * 0.5f / Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        maxOut_Z = distance;
    }

  
    private void SearchPosOffset()
    {
        // Tan(alpha) * adjacent  = opposite
        Vector3 angles = transform.parent.rotation.eulerAngles;
        float rad = angles.x * Mathf.Deg2Rad;
        posOffset.y = Mathf.Tan(rad) * -transform.parent.position.z;
    }

    #region Public Method
    public CameraType GetCameraType()
    {
        return type;
    }

    public void SubscribeToCamera(Vector2 _bounds_pos, Vector2 _dimension)
    {
        bounds_pos = _bounds_pos;
        bounds_dimension = _dimension;
        float offsetX = bounds_dimension.x / 2.0f;
        float offsetY = bounds_dimension.y / 2.0f;
        min_bounds.x = bounds_pos.x - offsetX;
        max_bounds.x = bounds_pos.x + offsetX;
        min_bounds.y = bounds_pos.y - offsetY;
        max_bounds.y = bounds_pos.y + offsetY;

        UpdateMoveBounds();
    }

    public void ResetCamera()
    {
        if (autoMaxOut_Z) SearchMaxOut_Z();
        if (autoOffset) SearchPosOffset();
        UpdateFrustum();
        UpdateMoveBounds();
        UpdatePlayersBounds();

    }
    #endregion

    #region Debug
    protected virtual void OnDrawGizmosSelected()
    {
        // Draw Camera Viewport
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.parent.position - (Vector3)posOffset, new Vector3(frustum_dimension.x, frustum_dimension.y, 1));

        // Draw Camera Bounds
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(bounds_pos, new Vector3(bounds_dimension.x * factorOffset, bounds_dimension.y * factorOffset, 1));

        // Draw Camera Movement Clamp
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(bounds_pos + posOffset, new Vector3(bounds_dimension.x - frustum_dimension.x, bounds_dimension.y - frustum_dimension.y, 1));

        // Draw Players Bounds
        Gizmos.color = Color.grey;
        Gizmos.DrawWireCube(barycenter, new Vector3(playerBounds.x, playerBounds.y, 1));

        // Draw Zoom Out Margin from PlayerBounds
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(barycenter, new Vector3(playerBounds.x + zoomOutMargin, playerBounds.y + zoomOutMargin, 1));

        // Draw Zoom In Margin from PlayerBounds
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(barycenter, new Vector3(playerBounds.x + zoomInMargin, playerBounds.y + zoomInMargin, 1));

        // Draw Zoom Out Margin
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.parent.position - (Vector3)posOffset, new Vector3(frustum_dimension.x - zoomOutMargin2, frustum_dimension.y - zoomOutMargin2, 1));
    }
    #endregion
}
