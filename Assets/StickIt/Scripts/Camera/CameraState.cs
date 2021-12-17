using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class CameraState : MonoBehaviour
{
    [Header("------- Data -------")]
    public CameraType type = CameraType.BARYCENTER;
    public CameraData data;
    public bool autoBounds = true;
    public Collider camBounds;
    [Min(0.1f)]
    public float factorOffset = 1.0f;                           
    public bool autoOffset = true;
    public Vector2 posOffset = new Vector2(0.0f, 0.0f);
    public bool canDezoomOnTransition = true;
    [Range(0f, 100f)]
    public float dezoomFactor = 100.0f;

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
    protected Bounds playerBounds = new Bounds();
    protected Vector2 min_playerBounds = new Vector2(0.0f, 0.0f);
    protected Vector2 max_playerBounds = new Vector2(0.0f, 0.0f);
    #endregion

    protected virtual void Awake()
    {
        GameEvents.OnSwitchCamera.AddListener(UpdateCameraDatas);
        cam = Camera.main;
        maxIn_Z = data.maxZoomIn;
    }

    protected virtual void Start()
    {
        mapManager = MapManager.instance;
        multiplayerManager = MultiplayerManager.instance;
        if (multiplayerManager != null) { playerList = multiplayerManager.players; }

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

        UpdateCameraDatas();
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
        UpdateCameraDatas();
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
        if (positionToGoTo.z == float.NaN) { return; }
        MoveAndZoom();
    }

    //<summary>
    //      Make The camera Move and Zoom
    //<summary>
    protected virtual void MoveAndZoom()
    {
        // Move Camera
        Vector3 newPos = new Vector3(
            positionToGoTo.x + posOffset.x,
            positionToGoTo.y + posOffset.y,
            transform.parent.position.z);
        transform.parent.position = Vector3.SmoothDamp(transform.parent.position, newPos, ref moveVelocity, moveTime, Mathf.Infinity, Time.unscaledDeltaTime);

        // if zooming out and is touching border > return and wait to move first
        //bool isTouchingBorder =
        //    min_viewport.x <= min_bounds.x || max_viewport.x >= max_bounds.x;
        //if (canClampZoom && positionToGoTo.z < transform.parent.position.z && isTouchingBorder) { return; }

        // Zoom Camera
        Vector3 newZoom = new Vector3(
            transform.parent.position.x,
            transform.parent.position.y,
            positionToGoTo.z);
        transform.parent.position = Vector3.SmoothDamp(transform.parent.position, newZoom, ref zoomVelocity, zoomTime, Mathf.Infinity, Time.unscaledDeltaTime);
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
        if (playerList.Count == 0) return;

        playerBounds = new Bounds();
        foreach (Player player in playerList)
        {
            Collider collider = player.GetComponent<SphereCollider>();
            if(collider != null)
            {
                Bounds bounds = collider.bounds;
                playerBounds.Encapsulate(bounds);
            }
        }

        Vector2 offset = new Vector2(playerBounds.size.x, playerBounds.size.y);

        min_playerBounds.x = playerBounds.center.x - offset.x;
        min_playerBounds.y = playerBounds.center.y - offset.y;
        max_playerBounds.x = playerBounds.center.x + offset.x;
        max_playerBounds.y = playerBounds.center.y + offset.y;
    }

    //<summary>
    //      Calculate zoom depending of the players positions/bounds
    //<summary>
    protected virtual void UpdateZoom() {

        Vector2 maxDistance = new Vector2(bounds_dimension.x, bounds_dimension.y);
        Vector2 ratio = new Vector2(0.0f, 0.0f);
        if ((maxDistance.x == 0 || maxDistance.y == 0) && SceneManager.GetActiveScene().name == "100_EndScene")
        {
            bounds_dimension = BlocksScript.Instance.Dimension;
        }
        // Divide by zero protection
        if (maxDistance.x != 0)
        {
            ratio.x = Mathf.Clamp(playerBounds.size.x, 0, maxDistance.x) / maxDistance.x;
        }

        // Divide by zero protection
        if(maxDistance.y != 0) { 
            ratio.y = Mathf.Clamp(playerBounds.size.y, 0, maxDistance.y) / maxDistance.y; 
        }

        positionToGoTo.z = Mathf.Lerp(maxIn_Z, maxOut_Z, Mathf.Max(ratio.x, ratio.y));
    }

    //<summary>
    //      Search the maximum zoom out value the camera can take
    //<summary>
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

    //<summary>
    //      If rotation of camera > search the posOffset to take viewport angle in account
    //<summary>
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

    //<summary>
    //      Get all data from camera bounds
    //<summary>
    public void SubscribeToCamera(Vector2 _bounds_pos, Vector2 _dimension, CameraData _data)
    {
        bounds_pos = _bounds_pos;
        bounds_dimension = _dimension;
        float offsetX = bounds_dimension.x / 2.0f;
        float offsetY = bounds_dimension.y / 2.0f;
        min_bounds.x = bounds_pos.x - offsetX;
        max_bounds.x = bounds_pos.x + offsetX;
        min_bounds.y = bounds_pos.y - offsetY;
        max_bounds.y = bounds_pos.y + offsetY;
        if (_data != null)
        {
            data = _data;
            maxIn_Z = data.maxZoomIn;
        }
        else
        {
            maxIn_Z = data.maxZoomIn;
        }

        UpdateCameraDatas();
        // Dezoom to new map
        StartCoroutine(OnSubscribeCamera());    
    }

    public void UpdateCameraDatas()
    {
        if (autoMaxOut_Z) SearchMaxOut_Z();
        if (autoOffset) SearchPosOffset();
        UpdateFrustum();
        UpdateMoveBounds();
        UpdatePlayersBounds();
    }
    #endregion

    private IEnumerator OnSubscribeCamera()
    {
        if (canDezoomOnTransition) {
            while (Mathf.Abs(positionToGoTo.z - maxOut_Z) > 0.1f)
            {
                UpdateCameraDatas();
                positionToGoTo.x = bounds_pos.x;
                positionToGoTo.y = bounds_pos.y;
                positionToGoTo.z = maxOut_Z * (dezoomFactor / 100.0f);
                MoveAndZoom();
                yield return null;
            }
        }
    }
    #region Debug
    protected virtual void OnDrawGizmos()
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
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(playerBounds.center, new Vector3(playerBounds.size.x, playerBounds.size.y, 1));
    }
    #endregion
}
