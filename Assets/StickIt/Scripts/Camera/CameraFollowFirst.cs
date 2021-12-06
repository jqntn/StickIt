using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowFirst : CameraState
{
    /*
    [Header("---------- ANIMATION ---------")]
    public float smoothTime = 0.5f;
    [Header("Offset between camera & first player")]
    public float offset_X = 10.0f;
    public float offset_Y = 5.0f;
    [Header("Centroid of player")]
    [Header("+ Offset in the direction first Player")]
    public float centroidOffset_X = 0.5f;
    public float centroidOffset_Y = 0.5f;

    [Header("---------- CAMERA BOUNDS ---------")]
    public float deathOffset = 0.2f;

    [Header("----------- DEBUG ------------")]
    [SerializeField] private List<Player> playerList = new List<Player>();
    [SerializeField] private MultiplayerManager multiplayerManager;
    [SerializeField] private Player playerFirst;
    [SerializeField] private Camera cam;
    [SerializeField] private RunnerManager runnerManager;
    [SerializeField] private RaceDirection currentDirection;
    [SerializeField] private Vector2 positionToGoTo;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private Vector2 startPos;
    [SerializeField] private Vector3 centroid;
    [SerializeField] private float timer = 0.0f;

    private void Awake()
    {
        cam = Camera.main;
    }

    //private void OnEnable()
    //{
    //    // Getting the direction of the run
    //    currentDirection = RunnerManager.Instance.direction;
    //}

    private void Start()
    {
        multiplayerManager = MultiplayerManager.instance;
        playerList = multiplayerManager.players;
        runnerManager = RunnerManager.Instance;
        currentDirection = runnerManager.direction;
        startPos = transform.position;
    }

    public void SetCurrentFirst(Player first)
    {
        playerFirst = first;
    }

    #region Camera Movement
    public void LateUpdate()
    {
        // Move camera
        Vector3 newPos = new Vector3(
            positionToGoTo.x,
            positionToGoTo.y,
            transform.position.z);

        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothTime);
    }

    public void Update()
    {
        timer += Time.deltaTime;
        if(playerFirst == null) { return; }

        GetCentroid();
        UpdatePositionToGoTo();
        UpdatePlayerOnCamera();
    }
    #endregion
    private void GetCentroid()
    {
        centroid = new Vector3(0, 0, 0);
        foreach (Player player in playerList)
        {
            centroid += player.transform.position;
        }
        centroid /= playerList.Count;

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
        positionToGoTo = centroid;
        float first_Y = playerFirst.transform.position.y;
        float first_X = playerFirst.transform.position.x;

        //Front = direction + the first who is in that direction
        // calculate camera bounds to first
        //switch (currentDirection)
        //{
        //    case RaceDirection.UP:
        //        foreach (Player player in playerList)
        //        {
        //            float player_Y = player.transform.position.y;
        //            if (first_Y < player_Y)
        //            {
        //                playerFirst = player;
        //                first_Y = player_Y;
        //            }
        //        }

        //        // Adding Offset Camera
        //        positionToGoTo = new Vector2(
        //            startPos.x + centroidOffset_X,
        //            playerFirst.transform.position.y - offset_Y);
        //        break;

        //    case RaceDirection.DOWN:
        //        foreach (Player player in playerList)
        //        {
        //            float player_Y = player.transform.position.y;
        //            if (first_Y > player_Y)
        //            {
        //                playerFirst = player;
        //                first_Y = player_Y;
        //            }
        //        }

        //        // Adding Offset Camera
        //        positionToGoTo = new Vector2(
        //            startPos.x + centroidOffset_X,
        //            playerFirst.transform.position.y + offset_Y);
        //        break;

        //    case RaceDirection.LEFT:
        //        foreach (Player player in playerList)
        //        {
        //            float player_X = player.transform.position.x;
        //            if (first_X > player_X)
        //            {
        //                playerFirst = player;
        //                first_X = player_X;
        //            }
        //        }

        //        // Adding Offset Camera
        //        positionToGoTo = new Vector2(
        //            playerFirst.transform.position.x - offset_X,
        //            startPos.y + centroidOffset_Y);
        //        break;

        //    case RaceDirection.RIGHT:
        //        foreach (Player player in playerList)
        //        {
        //            float player_X = player.transform.position.x;
        //            if (first_X < player_X)
        //            {
        //                playerFirst = player;
        //                first_X = player_X;
        //            }
        //        }

        //        // Adding Offset Camera
        //        positionToGoTo = new Vector2(
        //            playerFirst.transform.position.x - offset_X,
        //            startPos.y + centroidOffset_Y);
        //        break;

        //    case RaceDirection.D_UPRIGHT:
        //        foreach (Player player in playerList)
        //        {
        //            float player_X = player.transform.position.x;
        //            float player_Y = player.transform.position.y;

        //            if (first_X < player_X && first_Y < player_Y)
        //            {
        //                playerFirst = player;
        //                first_X = player_X;
        //                first_Y = player_Y;
        //            }
        //        }

        //        // Adding Offset Camera
        //        positionToGoTo = new Vector2(
        //            playerFirst.transform.position.x - offset_X,
        //            startPos.y + centroidOffset_Y - offset_Y);
        //        break;

        //    case RaceDirection.D_UPLEFT:
        //        foreach (Player player in playerList)
        //        {
        //            float player_X = player.transform.position.x;
        //            float player_Y = player.transform.position.y;

        //            if (first_X < player_Y && first_Y < player_Y)
        //            {
        //                playerFirst = player;
        //                first_X = player_X;
        //                first_Y = player_Y;
        //            }
        //        }

        //        // Adding Offset Camera
        //        positionToGoTo = new Vector2(
        //            playerFirst.transform.position.x - offset_X,
        //            startPos.y + centroidOffset_Y - offset_Y);
        //        break;

        //    case RaceDirection.D_DOWNRIGHT:
        //        foreach (Player player in playerList)
        //        {
        //            float player_X = player.transform.position.x;
        //            float player_Y = player.transform.position.y;

        //            if (first_X > player_X && first_Y < player_Y)
        //            {
        //                playerFirst = player;
        //                first_X = player_X;
        //                first_Y = player_Y;
        //            }
        //        }

        //        // Adding Offset Camera
        //        positionToGoTo = new Vector2(
        //            playerFirst.transform.position.x + offset_X,
        //            startPos.y + centroidOffset_Y + offset_Y);
        //        break;

        //    case RaceDirection.D_DOWNLEFT:
        //        foreach (Player player in playerList)
        //        {
        //            float player_X = player.transform.position.x;
        //            float player_Y = player.transform.position.y;

        //            if (first_X < player_X && first_Y > player_Y)
        //            {
        //                playerFirst = player;
        //                first_X = player_X;
        //                first_Y = player_Y;
        //            }
        //        }

        //        // Adding Offset Camera
        //        positionToGoTo = new Vector2(
        //            playerFirst.transform.position.x - offset_X,
        //            startPos.y + centroidOffset_Y - offset_Y);
        //        break;
        //}
    }

    private void UpdatePlayerOnCamera()
    {
        if (runnerManager.hasEndLevel) {
            timer = 0;
            return;
        }
        foreach (Player player in playerList)
        {
            Vector3 screenPoint = cam.WorldToViewportPoint(player.transform.position);
            bool onScreen = screenPoint.z > 0 &&
                screenPoint.x > 0 - deathOffset && screenPoint.x < 1 + deathOffset &&
                screenPoint.y > 0 - deathOffset && screenPoint.y < 1 + deathOffset;

            if (!onScreen && !runnerManager.GetDead().Contains(player))
            {
                player.Death();
                runnerManager.AddDeath(player);
                runnerManager.AddDeadTime(timer);
            }
        }
    }
    */
}
