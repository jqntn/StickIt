using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpeedRunner : MonoBehaviour
{
    [Header("------ Move ------")]
    public float moveTime = 0.5f;
    public float distanceBeforeBorder = 0.0f;

    [Header("------ Zoom ------")]
    public float maxOut_Z = -110.0f;
    public float maxIn_Z = -70.0f;
    public float zoomOutMargin = -5.0f;
    public float zoomInMargin = -15.0f;
    public float zoomValue = 10.0f;
    public float zoomTime = 0.5f;

    [Header("------ Death ------")]
    public float deathMargin = 10.0f;

    private RaceDirection currentDirection;
    private List<Player> playerList = new List<Player>();
    private Player playerFirst;
    private Vector3 centroid;
    private Vector3 positionToGoTo;
    private Vector3 moveVelocity;
    private Vector3 zoomVelocity;
    private Camera cam;
    private float frustumHeight;
    private float frustumWidth;
    private float timer;
    private RunnerManager runnerManager;
    private bool isFollowingFirst = false;

    private void Awake()
    {
        cam = Camera.main;
        CalculateFrustum();
        positionToGoTo.z = transform.position.z;
    }
    private void Start()
    {
        playerList = MultiplayerManager.instance.players;
        runnerManager = RunnerManager.Instance;
    }
    private void Update()
    {
        if (playerFirst == null) { return; }
        if (isFollowingFirst) { return; }

        UpdatePositionToGoTo();
        UpdateZoom();
        PlayerOffScreenShouldDie();
    }
    private void LateUpdate()
    {
        // Move Camera
        if (playerFirst == null) { return; }
        if (isFollowingFirst)
        {
            Vector3 firstPos = new Vector3(playerFirst.transform.position.x, playerFirst.transform.position.y, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, firstPos, ref moveVelocity, moveTime);
            return;
        }
        Vector3 newPos = new Vector3(
            positionToGoTo.x,
            positionToGoTo.y,
            transform.position.z);

        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref moveVelocity, moveTime);

        // Zoom Camera
        Vector3 newZoom = new Vector3(
            transform.position.x,
            transform.position.y,
            positionToGoTo.z);
        transform.position = Vector3.SmoothDamp(transform.position, newZoom, ref zoomVelocity, zoomTime);
    }
    private void GetCentroid()
    {
        centroid = new Vector3(0, 0, transform.position.z);
        foreach (Player player in playerList)
        {
            centroid += player.transform.position;
        }
        centroid /= playerList.Count;
        centroid = new Vector3(centroid.x, centroid.y, transform.position.z);
        //Debug
        float val = 0;
        foreach (Player player in playerList)
        {
            Debug.DrawLine(player.transform.position, centroid, Color.red + new Color(-val, val, 0));
            val += 0.25f;
        }
    }
    private void UpdatePositionToGoTo()
    {
        GetCentroid();
        positionToGoTo = centroid;
        if(playerFirst != null)
        {
            float first_X = playerFirst.transform.position.x;
            float first_Y = playerFirst.transform.position.y;
            float maxWidth = transform.position.x + frustumWidth / 2.0f;
            float minWidth = transform.position.x - maxWidth;
            float maxHeight = transform.position.y + frustumHeight / 2.0f;
            float minHeight = transform.position.y - maxHeight;

            bool isTouchingBorder =
                first_X <= minWidth + distanceBeforeBorder || first_X >= maxWidth - distanceBeforeBorder ||
                first_Y <= minHeight + distanceBeforeBorder || first_Y >= maxHeight - distanceBeforeBorder;

            float distanceCentroidMaxWidth = Mathf.Abs(centroid.x - maxWidth);
            float moveValue = Mathf.Abs(first_X - distanceCentroidMaxWidth) * 2f;
            Debug.Log("Move Value = " + moveValue);
            if (isTouchingBorder)
            {
                switch (currentDirection)
                {
                    case RaceDirection.UP:
                        positionToGoTo.y += moveValue;
                        break;
                    case RaceDirection.DOWN:
                        positionToGoTo.y -= moveValue;
                        break;
                    case RaceDirection.LEFT:
                        positionToGoTo.x -= moveValue;
                        break;
                    case RaceDirection.RIGHT:
                        positionToGoTo.x = transform.position.x + moveValue;
                        break;
                    case RaceDirection.D_UPRIGHT:
                        positionToGoTo.y += moveValue;
                        positionToGoTo.x += moveValue;
                        break;
                    case RaceDirection.D_UPLEFT:
                        positionToGoTo.y += moveValue;
                        positionToGoTo.x -= moveValue;
                        break;
                    case RaceDirection.D_DOWNRIGHT:
                        positionToGoTo.y -= moveValue;
                        positionToGoTo.x += moveValue;
                        break;
                    case RaceDirection.D_DOWNLEFT:
                        positionToGoTo.y -= moveValue;
                        positionToGoTo.x -= moveValue;
                        break;
                }
            }
        }
    }

    private void UpdateZoom()
    {
        // Zoom Out
        // if one player is outside zoomOut box
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
        }

        // if outside zoomOut box
        if (!insideZoomOutBox)
        {
            positionToGoTo.z = Mathf.Clamp(transform.position.z - zoomValue, maxOut_Z, maxIn_Z);
        }

        // Zoom In
        //if one player is inside zoomIn box
        bool insideZoomInBox = true;
        foreach(Player player in playerList)
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

            if (insideZoomInBox) { break; }
            // remove this if want to have all player inside box for zoomIn
            //if(!insideZoomInBox) { break; }
        }

        // if one player inside zoomInBox
        if (insideZoomInBox)
        {
            positionToGoTo.z = Mathf.Clamp(transform.position.z + zoomValue, maxOut_Z, maxIn_Z);
        }

        CalculateFrustum();
    }

    private void PlayerOffScreenShouldDie()
    {
        if (runnerManager.hasEndLevel)
        {
            timer = 0;
            return;
        }

        foreach (Player player in playerList)
        {
            float player_X = player.transform.position.x;
            float player_Y = player.transform.position.y;
            float player_Z = player.transform.position.z;
            float deathWidth = (frustumWidth + deathMargin) / 2.0f;
            float deathHeight = (frustumHeight + deathMargin) / 2.0f;
            float minDeahtZone_X = transform.position.x - deathWidth;
            float maxDeathZone_X = transform.position.x + deathWidth;
            float minDeathZone_Y = transform.position.y - deathHeight;
            float maxDeathZone_Y = transform.position.y + deathHeight;

            bool onScreen = player_Z > transform.position.z &&
                player_X > minDeahtZone_X && player_X < maxDeathZone_X &&
                player_Y > minDeathZone_Y && player_Y < maxDeathZone_Y;

            if (!onScreen && !runnerManager.GetDead().Contains(player))
            {
                player.Death();
                runnerManager.AddDeath(player);
                runnerManager.AddDeadTime(timer);
                if (runnerManager.hasEndLevel)
                {
                    isFollowingFirst = true;
                }
            }
        }
    }

    private void CalculateFrustum()
    {
        float distance = -transform.position.z;
        frustumHeight = 2.0f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        frustumWidth = frustumHeight * cam.aspect;
    }

    #region Public Method
    public void SetCurrentFirst(Player player)
    {
        playerFirst = player;
    }
    #endregion

    #region Debug
    void OnDrawGizmosSelected()
    {
        // Draw Camera Viewport
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(frustumWidth, frustumHeight, 1));

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(frustumWidth + deathMargin, frustumHeight + deathMargin, 1));

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(frustumWidth + zoomOutMargin, frustumHeight + zoomOutMargin, 1));
        Gizmos.DrawWireCube(transform.position, new Vector3(frustumWidth + zoomInMargin, frustumHeight + zoomInMargin, 1));
    }
    #endregion
}
