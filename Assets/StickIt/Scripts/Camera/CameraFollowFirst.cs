using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowFirst : MonoBehaviour
{
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
    public float deathOffset = 0.1f;

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
    [SerializeField] private float centroid_X;
    [SerializeField] private float centroid_Y;
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

        // Update position to go to
        float first_Y = playerFirst.transform.position.y;
        float first_X = playerFirst.transform.position.x;
        
        switch (currentDirection)
        {
            case RaceDirection.UP:
                foreach (Player player in playerList)
                {
                    float player_Y = player.transform.position.y;
                    if(first_Y < player_Y)
                    {
                        playerFirst = player;
                        first_Y = playerFirst.transform.position.y;
                    }
                }

                // Adding Offset Camera
                positionToGoTo = new Vector2(
                    startPos.x + centroidOffset_X, 
                    playerFirst.transform.position.y - offset_Y);
                break;
            case RaceDirection.DOWN:
                foreach (Player player in playerList)
                {
                    float player_Y = player.transform.position.y;
                    if (first_Y > player_Y)
                    {
                        playerFirst = player;
                        first_Y = playerFirst.transform.position.y;
                    }
                }

                // Adding Offset Camera
                positionToGoTo = new Vector2(
                    startPos.x + centroidOffset_X,
                    playerFirst.transform.position.y + offset_Y);
                break;
            case RaceDirection.LEFT:
                foreach (Player player in playerList)
                {
                    float player_X = player.transform.position.x;
                    if (first_X > player_X)
                    {
                        playerFirst = player;
                        first_X = playerFirst.transform.position.x;
                    }
                }

                // Adding Offset Camera
                positionToGoTo = new Vector2(
                    playerFirst.transform.position.x - offset_X,
                    startPos.y + centroidOffset_Y);
                break;
            case RaceDirection.RIGHT:
                foreach (Player player in playerList)
                {
                    float player_X = player.transform.position.x;
                    if (first_X < player_X)
                    {
                        playerFirst = player;
                        first_X = playerFirst.transform.position.x;
                    }
                }

                // Adding Offset Camera X
                positionToGoTo = new Vector2(
                    playerFirst.transform.position.x - offset_X,
                    startPos.y + centroidOffset_Y);
                break;
        }

        UpdatePlayerOnCamera();
    }
    #endregion
    private void GetCentroid()
    {
        centroidOffset_X = 0;
        centroidOffset_Y = 0;
        // Get centroid
        foreach (Player player in playerList)
        {
            centroidOffset_X += player.transform.position.x;
            centroidOffset_Y += player.transform.position.y;
        }
        centroidOffset_X /= playerList.Count;
        centroidOffset_Y /= playerList.Count;
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

            if (!onScreen && !runnerManager.GetDead().Contains(player) && !runnerManager.GetOrder().Contains(player))
            {
                runnerManager.AddDeath(player);
                runnerManager.AddDeadTime(timer);
                player.Death();
            }
        }
    }
}


public enum RaceDirection{
    UP,
    DOWN,
    LEFT,
    RIGHT,
    D_UPRIGHT,
    D_UPLEFT,
    D_DOWNRIGHT,
    D_DOWNLEFT
}
