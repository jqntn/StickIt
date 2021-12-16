using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraSpeedRunner : CameraState
{
    [Header("----- Move 2 -----")]
    public float distanceBeforeBorder = 0.0f;

    [Header("------ Death ------")]
    public float deathMargin = 10.0f;
    public float timeBeforeDeath = 0.2f;

    [Header("------ Debug 2 ------")]
    [SerializeField] private RaceDirection currentDirection = RaceDirection.RIGHT;
    [SerializeField] private Player frontPlayer = null;
    [SerializeField] private RunnerManager runnerManager = null;
    [SerializeField] private float runTimer = 0.0f;
    [SerializeField] private float[] deathTimer = { 0.0f, 0.0f, 0.0f, 0.0f };
    [SerializeField] private bool hasTouchBorder = false;
    [SerializeField] private int deadPlayersCount = 0;
    [SerializeField] private int numberOfPlayers = 0;

    protected override void Start()
    {
        base.Start();
        numberOfPlayers = multiplayerManager.nbrOfPlayer;

        runnerManager = RunnerManager.Instance;
        if (runnerManager != null)
        {
            currentDirection = runnerManager.direction;
        }
    }
    protected override void Update()
    {
        base.Update();
        if(runnerManager == null) { return; }
        if (frontPlayer == null) { return; }
        deadPlayersCount = multiplayerManager.deadPlayers.Count;
        if (numberOfPlayers >= deadPlayersCount - 1) { return; }

        if (canMove) { UpdateBarycenter(); }
        if (canZoom) { UpdateZoom(); }
        PlayerOffScreenShouldDie();
    }

    private void UpdateBarycenter()
    {
        base.barycenter = new Vector2(0.0f, 0.0f);
        foreach (Player player in playerList)
        {
            base.barycenter += player.transform.position;
        }
        base.barycenter /= playerList.Count;

        // Freeze X axis
        if (freezeX)
        {
            base.barycenter = new Vector2(transform.parent.position.x, base.barycenter.y);
        }

        // Freeze Y axis
        if (freezeY)
        {
            base.barycenter = new Vector2(base.barycenter.x, transform.parent.position.y);
        }

        // Clamp Value into bounds
        float newPos_X = Mathf.Clamp(base.barycenter.x, min_moveBounds.x, max_moveBounds.x);
        float newPos_Y = Mathf.Clamp(base.barycenter.y, min_moveBounds.y, max_moveBounds.y);

        // Update Position to Go To
        positionToGoTo = new Vector3(
            newPos_X,
            newPos_Y,
            transform.parent.position.z);

        //Debug
        float val = 0;
        foreach (Player player in playerList)
        {
            Debug.DrawLine(player.transform.position, base.barycenter, Color.red + new Color(-val, val, 0));
            val += 0.20f;
        }
    }

    private void PlayerOffScreenShouldDie()
    {
        int i = 0;
        foreach (Player player in playerList)
        {
            float player_X = player.transform.position.x;
            float player_Y = player.transform.position.y;
            float player_Z = player.transform.position.z;
            float deathWidth = (frustum_dimension.x + deathMargin) / 2.0f;
            float deathHeight = (frustum_dimension.y + deathMargin) / 2.0f;
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
                if (deathTimer[i] < timeBeforeDeath)
                {
                    deathTimer[i] += Time.deltaTime;
                }
                else
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

            // Increment Death Timer Index
            i++;
        }

    }
    private void UpdatePositionToGoTo()
    {
        if (frontPlayer != null) { GetFrontPlayer(); }
        UpdateBarycenter();
        if (!hasTouchBorder)
        {
            positionToGoTo = barycenter;
        }

        if (frontPlayer != null && transform.position.z >= maxOut_Z)
        {
            float first_X = frontPlayer.transform.position.x;
            float first_Y = frontPlayer.transform.position.y;
            float width = frustum_dimension.x / 2.0f;
            float height = frustum_dimension.y / 2.0f;
            float maxWidth = transform.position.x + width;
            float minWidth = transform.position.x - width;
            float maxHeight = transform.position.y + height;
            float minHeight = transform.position.y - height;
            // If Player touching or outside border of camera viewport
            bool isTouchingBorder =
                first_X <= minWidth + distanceBeforeBorder || first_X >= maxWidth - distanceBeforeBorder ||
                first_Y <= minHeight + distanceBeforeBorder || first_Y >= maxHeight - distanceBeforeBorder;

            // Get New PositionToGoTo
            float camToCentroid = Mathf.Abs(transform.position.x - barycenter.x);
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
    #region Public Method
    public void SetCurrentFirst(Player player)
    {
        frontPlayer = player;
    }
    #endregion

    #region Debug
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        // Draw Death Margin
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(frustum_dimension.x + deathMargin, frustum_dimension.y + deathMargin, 1));
    }
    #endregion
}
