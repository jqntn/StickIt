using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class CameraState : MonoBehaviour
{
    [Header("------- Data -------")]
    public CameraType type = CameraType.BARYCENTER;
    public bool autoBounds = true;
    public Collider camBounds;
    [Min(0.1f)]
    public float factorOffset = 1.0f;                           
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
    public bool canClampZoom = false;

    #region Protected 
    protected Camera cam = null;
    protected MapManager mapManager = null;
    protected MultiplayerManager multiplayerManager = null;
    protected List<Player> playerList = new List<Player>();
    protected Vector3 positionToGoTo = new Vector3(0.0f, 0.0f, 0.0f);
    protected Vector3 moveVelocity = new Vector3(0.0f, 0.0f, 0.0f);
    protected Vector3 zoomVelocity = new Vector3(0.0f, 0.0f, 0.0f);
    protected Vector3 barycenter = new Vector3(0.0f, 0.0f, 0.0f);
    protected Vector2 bounds_pos = new Vector3(0.0f, 0.0f);
    protected Vector2 bounds_dimension = new Vector2(0.0f, 0.0f);
    protected Vector2 min_bounds = new Vector2(0.0f, 0.0f);
    protected Vector2 max_bounds = new Vector2(0.0f, 0.0f);
    protected Vector2 min_moveBounds = new Vector2(0.0f, 0.0f);
    protected Vector2 max_moveBounds = new Vector2(0.0f, 0.0f);
    protected Vector2 frustum_dimension = new Vector2(0.0f, 0.0f);
    protected Vector2 min_viewport = new Vector2(0.0f, 0.0f);
    protected Vector2 max_viewport = new Vector2(0.0f, 0.0f);
    protected Vector2 min_zoomOutBounds = new Vector2(0.0f, 0.0f);
    protected Vector2 max_zoomOutBounds = new Vector2(0.0f, 0.0f);
    protected Vector2 playerBounds = new Vector2(0.0f, 0.0f);
    protected Vector2 min_playerBounds = new Vector2(0.0f, 0.0f);
    protected Vector2 max_playerBounds = new Vector2(0.0f, 0.0f);
    #endregion

    protected virtual void Awake()
    {
        GameEvents.OnSwitchCamera.AddListener(ResetCamera);
    }

    protected virtual void Start()
    {
        cam = Camera.main;
        mapManager = MapManager.instance;
        multiplayerManager = MultiplayerManager.instance;
        if(multiplayerManager != null) { playerList = multiplayerManager.players; }

        if (SceneManager.GetActiveScene().name == "0_MainMenu" 
            || SceneManager.GetActiveScene().name == "1_MenuSelection" 
            || SceneManager.GetActiveScene().buildIndex == 0 
            || SceneManager.GetActiveScene().buildIndex == 1) { 
            return; 
        }
        if (multiplayerManager == null) { return; }
        if (mapManager == null) { return; }
        if (cam == null) { return; }
        if (mapManager.isBusy) { return; }
        if (playerList.Count == 0) { return; }
        ResetCamera();
    }

    protected virtual void Update()
    {
        // Protections
        if (SceneManager.GetActiveScene().name == "0_MainMenu" 
            || SceneManager.GetActiveScene().name == "1_MenuSelection" 
            || SceneManager.GetActiveScene().buildIndex == 0 
            || SceneManager.GetActiveScene().buildIndex == 1) 
        { 
            return; 
        }
        if (multiplayerManager == null) { return; }
        if (mapManager == null) { return; }
        if (cam == null) { return; }
        if (mapManager.isBusy) { return; }
        if (playerList.Count == 0) { return; }

        ResetCamera();
    }

    protected virtual void LateUpdate()
    {
        // Protections
        if (SceneManager.GetActiveScene().name == "0_MainMenu"
            || SceneManager.GetActiveScene().name == "1_MenuSelection"
            || SceneManager.GetActiveScene().buildIndex == 0
            || SceneManager.GetActiveScene().buildIndex == 1)
        {
            return;
        }
        if (multiplayerManager == null) { return; }
        if (mapManager == null) { return; }
        if (cam == null) { return; }
        if (mapManager.isBusy) { return; }
        if (playerList.Count == 0) { return; }

        UpdateCamera();
    }

    //<summary>
    //      Make The camera Move and Zoom
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
    //      Viewport dimension changing depending of the Zoom Value
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
    }

    //<summary>
    //      Camera Movement clamping depending of zoom
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
    //      Player bounds are the encapsulation of all the player to calculate how to zoom in
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

    //<summary>
    //      Calculate zoom depending of the players positions/bounds
    //<summary>
    protected virtual void UpdateZoom() {
        Vector2 maxDistance = new Vector2(bounds_dimension.x, bounds_dimension.y);
        Vector2 ratio = new Vector2(0.0f, 0.0f);
        ratio.x = Mathf.Clamp(playerBounds.x, 0, maxDistance.x) / maxDistance.x;
        ratio.y = Mathf.Clamp(playerBounds.y, 0, maxDistance.y) / maxDistance.y;

        positionToGoTo.z = Mathf.Lerp(maxIn_Z, maxOut_Z, Mathf.Max(ratio.x, ratio.y));
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
    }
    #endregion
}
