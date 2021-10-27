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
    public float centroidOffet_X = 0.5f;
    public float centroidOffset_Y = 0.5f;

    [Header("----------- DEBUG ------------")]
    [SerializeField] private List<Player> playerList = new List<Player>();
    [SerializeField] private MultiplayerManager multiplayerManager;
    [SerializeField] private Player playerFirst;
    [SerializeField] private RaceDirection currentDirection;
    [SerializeField] private Vector2 positionToGoTo;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private Vector2 startPos;
    [SerializeField] private float centroid_X;
    [SerializeField] private float centroid_Y;
    //private void OnEnable()
    //{
    //    // Getting the direction of the run
    //    currentDirection = RunnerManager.Instance.direction;
    //}

    private void Start()
    {
        multiplayerManager = MultiplayerManager.instance;
        playerList = multiplayerManager.players;
        currentDirection = RunnerManager.Instance.direction;
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
        if(playerFirst == null) { return; }

        // Update who is first
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

                // Adding Offset Camera Y
                positionToGoTo = new Vector2(
                    startPos.x + centroidOffet_X, 
                    playerFirst.transform.position.y - offset_Y);
                break;
            case RaceDirection.DOWN:
                foreach (Player player in playerList)
                {
                    float player_Y = player.transform.position.y;
                    if (first_Y < player_Y)
                    {
                        playerFirst = player;
                        first_Y = playerFirst.transform.position.y;
                    }
                }

                // Adding Offset Camera Y
                positionToGoTo = new Vector2(
                    startPos.x + centroidOffet_X,
                    playerFirst.transform.position.y + offset_Y);
                break;
            case RaceDirection.LEFT:
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
                    playerFirst.transform.position.x + offset_X,
                    startPos.y + centroidOffset_Y);
                // if player first is down offset should go up
                // if player first is up offset should go down
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
    }
    #endregion
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
