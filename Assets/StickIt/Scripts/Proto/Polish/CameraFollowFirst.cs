using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowFirst : MonoBehaviour
{
    public List<Player> playerList = new List<Player>();
    private MultiplayerManager multiplayerManager;
    [Header("---------- DEBUG ------------")]
    public RaceDirection direction = RaceDirection.RIGHT;
    [SerializeField] private Vector2 positionToGoTo;
    private Vector3 velocity;
    public float smoothTime = 0.2f;
    public GameObject first;
    public GameObject second;
    public int currentCheckpoint = 0;

    public void GetFirst(GameObject _first)
    {
        first = _first;
        foreach(Player player in playerList)
        {
            if(_first != player.gameObject)
            {
                second = player.gameObject;
            }
        }
    }

    private void SwitchFirst()
    {
        if(second.GetComponent<RacePlayer>().raceCheckpoint == currentCheckpoint)
        {
            GameObject temp = first;
            first = second;
            second = temp;
        }

        positionToGoTo = first.transform.position;
    }
    private void Start()
    {
        multiplayerManager = MultiplayerManager.instance;
        playerList = multiplayerManager.players;
    }
    public void Update()
    {
        float first_Y = Mathf.Round(first.transform.position.y);
        float first_X = Mathf.Round(first.transform.position.x);
        float second_Y = Mathf.Round(second.transform.position.y);
        float second_X = Mathf.Round(second.transform.position.x);
        switch (direction)
        {
            case RaceDirection.UP:
                if (first_Y >= second_Y && first.GetComponent<RacePlayer>().raceCheckpoint == currentCheckpoint)
                {
                    positionToGoTo = first.transform.position;
                }
                else
                {
                    SwitchFirst();
                }
                break;
            case RaceDirection.DOWN: 
                if (first_Y <= second_Y && first.GetComponent<RacePlayer>().raceCheckpoint == currentCheckpoint)
                {
                    positionToGoTo = first.transform.position;
                }
                else
                {
                    SwitchFirst();
                }
                break;
            case RaceDirection.LEFT: 
                if (first_X <= second_X && first.GetComponent<RacePlayer>().raceCheckpoint == currentCheckpoint)
                {
                    positionToGoTo = first.transform.position;
                }
                else
                {
                    SwitchFirst();
                }
                break;
            case RaceDirection.RIGHT: 
                if (first_X >= second_X && first.GetComponent<RacePlayer>().raceCheckpoint == currentCheckpoint)
                {
                    positionToGoTo = first.transform.position;
                }
                else
                {
                    SwitchFirst();
                }
                break;
        }
    }
    void LateUpdate()
    {
        Vector3 newPos = new Vector3(
            positionToGoTo.x,
            positionToGoTo.y,
            transform.position.z);

        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothTime);
    }
}

public enum RaceDirection{
    UP,
    DOWN,
    LEFT,
    RIGHT
}
