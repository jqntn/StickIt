using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraSpeedRunner : CameraState
{
    [Header("------ Move ------")]
    public float moveTime = 0.2f;
    public float distanceBeforeBorder = 0.0f;
    public bool freezeX = false;
    public bool freezeY = false;
    public bool hasCenterCamera = false;
    public bool hasFollowLastPlayer = false;
    public bool hasFreeRoaming = false;
    public int randomRadius = 5;
    public float roamingTime = 3.0f;

    [Header("------ Zoom ------")]
    public float maxOut_Z = -110.0f;
    public float maxIn_Z = -70.0f;
    public float zoomOutMargin = -5.0f;
    public float zoomInMargin = -10.0f;
    public float zoomOutValue = 20.0f;
    public float zoomInValue = 10.0f;
    public float zoomTime = 0.2f;
    public bool hasZoomOutAtEnd = false;

    [Header("------ Death ------")]
    public float deathMargin = 10.0f;
    public float timeBeforeDeath = 0.2f;

    [Header("------ Debug ------")]
    [SerializeField] private RaceDirection currentDirection;
    [SerializeField] private List<Player> playerList = new List<Player>();
    [SerializeField] private Player frontPlayer;
    [SerializeField] private Camera cam;
    [SerializeField] private RunnerManager runnerManager;
    [SerializeField] private MapManager mapManager;
    [SerializeField] private Vector3 centroid;
    [SerializeField] private Vector3 positionToGoTo;
    [SerializeField] private Vector3 moveVelocity;
    [SerializeField] private Vector3 zoomVelocity;
    [SerializeField] private Vector3 roamingVelocity;
    [SerializeField] private float frustumHeight;
    [SerializeField] private float frustumWidth;
    [SerializeField] private float runTimer;
    [SerializeField] private float[] deathTimer = { 0.0f, 0.0f, 0.0f, 0.0f };
    [SerializeField] private int sceneIndex = 0;
    [SerializeField] private int dataIndex = 0;
    [SerializeField] private int numberPlayers = 0;
    [SerializeField] private bool hasTouchBorder = false;
    [SerializeField] private bool onlyOnePlayerLeft = false;
    [SerializeField] private CenterCamera center;
    protected void Awake()
    {
        cam = Camera.main;
        CalculateFrustum();
        positionToGoTo.z = transform.position.z;
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
    }
    protected void Start()
    {
        playerList = MultiplayerManager.instance.players;
        numberPlayers = MultiplayerManager.instance.nbrOfPlayer;
        runnerManager = RunnerManager.Instance;
        mapManager = MapManager.instance;
        center = CenterCamera.Instance;

        if(runnerManager != null)
        {
            currentDirection = runnerManager.direction;
        }
    }
    protected void Update()
    {
        // Protections
        if (SceneManager.GetActiveScene().buildIndex == 0) { return; }
        if (mapManager.isBusy) { return; }
        bool allDead = true;
        foreach(Player player in playerList)
        {
            if (!player.isDead) {
                allDead = false;
                break;
            }
        }
        if (allDead && !hasFreeRoaming) { return; }

        // End Cam Animation
        if (allDead) {
            if (hasFreeRoaming)
            {
                positionToGoTo = Random.insideUnitCircle * randomRadius;
            }

            if (hasZoomOutAtEnd)
            {
                positionToGoTo.z = maxOut_Z;
                CalculateFrustum();
            }
            return;
        }
        if (hasCenterCamera && onlyOnePlayerLeft) { return; }
        int count = 0;
        foreach (Player player in playerList)
        {
            if (!player.isDead)
            {
                count++;
                if(count > 1)
                {
                    break;
                }
            }
        }
        if(count == 1)
        {
            onlyOnePlayerLeft = true;
        }
        if (onlyOnePlayerLeft) { return; }

        UpdatePositionToGoTo();
        UpdateZoom();
        // Update the camera viewport value in world space
        CalculateFrustum();
        PlayerOffScreenShouldDie();
    }
    protected void LateUpdate()
    {
        // Protections
        if (SceneManager.GetActiveScene().buildIndex == 0) { return; }
        if (mapManager.isBusy) { return; }
        bool allDead = true;
        foreach (Player player in playerList)
        {
            if (!player.isDead)
            {
                allDead = false;
                break;
            }
        }
        if (allDead && !hasFreeRoaming) { return; }

            if (hasFreeRoaming)
            {
                transform.position = Vector3.SmoothDamp(transform.position, positionToGoTo, ref roamingVelocity, roamingTime);
            }

            return;
        }

        if (hasCenterCamera && onlyOnePlayerLeft)
        {
            if(center == null) { return; }
            positionToGoTo = center.transform.position;
            positionToGoTo.z = maxOut_Z;
            transform.position = Vector3.SmoothDamp(transform.position, positionToGoTo, ref moveVelocity, moveTime);
            return;
        }
        // If only 1 player alive > Camera Follow Player
        if (onlyOnePlayerLeft && hasFollowLastPlayer)
        {
            foreach(Player player in playerList)
            {
                if (!player.isDead)
                {
                    frontPlayer = player;
                    break;
                }
            }
            Vector3 firstPos = new Vector3(frontPlayer.transform.position.x, frontPlayer.transform.position.y, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, firstPos, ref moveVelocity, moveTime);
            return;
        }

        if(onlyOnePlayerLeft) { return; }

        // Protection for running only
        //if (frontPlayer == null) { return; }

        // Move Camera
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
    private void UpdatePositionToGoTo()
    {
        if(frontPlayer != null) { GetFrontPlayer(); }
        GetCentroid();
        if (!hasTouchBorder)
        {
            positionToGoTo = centroid;
        }

        if(frontPlayer != null && transform.position.z >= maxOut_Z)
        {
            float first_X = frontPlayer.transform.position.x;
            float first_Y = frontPlayer.transform.position.y;
            float width = frustumWidth / 2.0f;
            float height = frustumHeight / 2.0f;
            float maxWidth = transform.position.x + width;
            float minWidth = transform.position.x - width;
            float maxHeight = transform.position.y + height;
            float minHeight = transform.position.y - height;
            // If Player touching or outside border of camera viewport
            bool isTouchingBorder =
                first_X <= minWidth + distanceBeforeBorder || first_X >= maxWidth - distanceBeforeBorder ||
                first_Y <= minHeight + distanceBeforeBorder || first_Y >= maxHeight - distanceBeforeBorder;

            // Get New PositionToGoTo
            float camToCentroid = Mathf.Abs(transform.position.x - centroid.x);
            float distanceCentroidMaxWidth = Mathf.Abs(camToCentroid - maxWidth);
            float moveValue = Mathf.Abs(first_X - distanceCentroidMaxWidth);
            if (isTouchingBorder)
            {
                switch (currentDirection)
                {
                    case RaceDirection.UP:
                        positionToGoTo.y = transform.position.y + moveValue;
                        break;
                    case RaceDirection.DOWN:
                        positionToGoTo.y = transform.position.y - moveValue;
                        break;
                    case RaceDirection.LEFT:
                        positionToGoTo.x = transform.position.x - moveValue;
                        break;
                    case RaceDirection.RIGHT:
                        positionToGoTo.x = transform.position.x + moveValue;
                        break;
                    case RaceDirection.D_UPRIGHT:
                        positionToGoTo.y = transform.position.y + moveValue;
                        positionToGoTo.x = transform.position.x + moveValue;
                        break;
                    case RaceDirection.D_UPLEFT:
                        positionToGoTo.y = transform.position.y + moveValue;
                        positionToGoTo.x = transform.position.x - moveValue;
                        break;
                    case RaceDirection.D_DOWNRIGHT:
                        positionToGoTo.y = transform.position.y - moveValue;
                        positionToGoTo.x = transform.position.x + moveValue;
                        break;
                    case RaceDirection.D_DOWNLEFT:
                        positionToGoTo.y = transform.position.y - moveValue;
                        positionToGoTo.x = transform.position.x - moveValue;
                        break;
                }

                hasTouchBorder = true;
            }
        }
    }

    private void GetFrontPlayer()
    {
        float front_Y = frontPlayer.transform.position.y;
        float front_X = frontPlayer.transform.position.x;
        switch (currentDirection)
        {
            case RaceDirection.UP:
                foreach (Player player in playerList)
                {
                    if (player.isDead) { continue; }

                    float player_Y = player.transform.position.y;
                    if (front_Y < player_Y)
                    {
                        frontPlayer = player;
                        front_Y = player_Y;
                    }
                }
                break;
            case RaceDirection.DOWN:
                foreach (Player player in playerList)
                {
                    if (player.isDead) { continue; }

                    float player_Y = player.transform.position.y;
                    if (front_Y > player_Y)
                    {
                        frontPlayer = player;
                        front_Y = player_Y;
                    }
                }
                break;
            case RaceDirection.LEFT:
                foreach (Player player in playerList)
                {
                    if (player.isDead) { continue; }

                    float player_X = player.transform.position.x;
                    if (front_X > player_X)
                    {
                        frontPlayer = player;
                        front_X = player_X;
                    }
                }
                break;
            case RaceDirection.RIGHT:
                foreach (Player player in playerList)
                {
                    if (player.isDead) { continue; }

                    float player_X = player.transform.position.x;
                    if (front_X < player_X)
                    {
                        frontPlayer = player;
                        front_X = player_X;
                    }
                }
                break;
            case RaceDirection.D_UPRIGHT:
                foreach (Player player in playerList)
                {
                    if (player.isDead) { continue; }

                    float player_X = player.transform.position.x;
                    float player_Y = player.transform.position.y;

                    if (front_X < player_X && front_Y < player_Y)
                    {
                        frontPlayer = player;
                        front_X = player_X;
                        front_Y = player_Y;
                    }
                }
                break;
            case RaceDirection.D_UPLEFT:
                foreach (Player player in playerList)
                {
                    if (player.isDead) { continue; }

                    float player_X = player.transform.position.x;
                    float player_Y = player.transform.position.y;

                    if (front_X < player_Y && front_Y < player_Y)
                    {
                        frontPlayer = player;
                        front_X = player_X;
                        front_Y = player_Y;
                    }
                }
                break;
            case RaceDirection.D_DOWNRIGHT:
                foreach (Player player in playerList)
                {
                    if (player.isDead) { continue; }

                    float player_X = player.transform.position.x;
                    float player_Y = player.transform.position.y;

                    if (front_X > player_X && front_Y < player_Y)
                    {
                        frontPlayer = player;
                        front_X = player_X;
                        front_Y = player_Y;
                    }
                }
                break;
            case RaceDirection.D_DOWNLEFT:
                foreach (Player player in playerList)
                {
                    if (player.isDead) { continue; }

                    float player_X = player.transform.position.x;
                    float player_Y = player.transform.position.y;

                    if (front_X < player_X && front_Y > player_Y)
                    {
                        frontPlayer = player;
                        front_X = player_X;
                        front_Y = player_Y;
                    }
                }
                break;
        }
    }
    private void GetCentroid()
    {
        centroid = new Vector3(0, 0, transform.position.z);
        foreach (Player player in playerList)
        {
            centroid += player.transform.position;
        }
        centroid /= playerList.Count;

        // Reset Z
        centroid = new Vector3(centroid.x, centroid.y, transform.position.z);

        // Freeze X axis
        if (freezeX)
        {
            centroid = new Vector3(transform.position.x, centroid.y, transform.position.z);
        }

        // Freeze Y axis
        if (freezeY)
        {
            centroid = new Vector3(centroid.x, transform.position.y, transform.position.z);
        }

        //Debug
        float val = 0;
        foreach (Player player in playerList)
        {
            Debug.DrawLine(player.transform.position, centroid, Color.red + new Color(-val, val, 0));
            val += 0.20f;
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

            if (!insideZoomOutBox) { break; }
        }

        // Get New Zoom if outside of zoomOut box
        if (!insideZoomOutBox)
        {
            positionToGoTo.z = Mathf.Clamp(transform.position.z - zoomOutValue, maxOut_Z, maxIn_Z);
            return;
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

            // remove this if want to have 1 player inside box for zoomin
            //if (insideZoomInBox) { break; }

            // remove this if want to have all player inside box for zoomIn
            if(!insideZoomInBox) { break; }
        }

        // Get New Zoom if player inside zoomIn box
        if (insideZoomInBox)
        {
            positionToGoTo.z = Mathf.Clamp(transform.position.z + zoomInValue, maxOut_Z, maxIn_Z);
        }
    }

    private void PlayerOffScreenShouldDie()
    {
        // Protections
        if (runnerManager == null) { return; }
        if (runnerManager.hasEndLevel)
        {
            runTimer = 0;
            return;
        }
        
        int i = 0;
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
                // Increase Timer
                if(deathTimer[i] < timeBeforeDeath)
                {
                    deathTimer[i] += Time.deltaTime;
                }else
                {
                    // Death
                    player.Death();
                    runnerManager.AddDeath(player);
                    runnerManager.AddDeadTime(runTimer);
                }
            }
            else
            {
                // Reset Death timer
                deathTimer[i] = 0f;
            }

            // If only one player left > Camera follow only player left
            if (runnerManager.hasEndLevel)
            {
                onlyOnePlayerLeft = true;
            }
            // Increment Death Timer Index
            i++;
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
        frontPlayer = player;
    }

    public void LoadCameraData()
    {
        CameraData current = datas[dataIndex];

        moveTime = current.moveTime;
        distanceBeforeBorder = current.distanceBeforeBorder;
        freezeX = current.freezeX;
        freezeY = current.freezeY;
        hasFollowLastPlayer = current.hasFollowOnlyPlayer;
        hasFreeRoaming = current.hasFreeRoaming;
        randomRadius = current.randomRadius;
        roamingTime = current.roamingTime;
        maxOut_Z = current.maxOut_Z;
        maxIn_Z = current.maxIn_Z;
        zoomOutMargin = current.zoomOutMargin;
        zoomInMargin = current.zoomInMargin;
        zoomOutValue = current.zoomOutSpeed;
        zoomInValue = current.zoomInSpeed;
        zoomTime = current.zoomTime;
        hasZoomOutAtEnd = current.hasZoomOutAtEnd;
        deathMargin = current.deathMargin;
        timeBeforeDeath = current.timeBeforeDeath;

        dataIndex++;
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
