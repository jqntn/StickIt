using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowFirst : MonoBehaviour
{
    public List<Player> playerList = new List<Player>();
    MultiplayerManager multiplayerManager;
    [Header("---------- DEBUG ------------")]
    public RaceDirection direction = RaceDirection.RIGHT;
    [SerializeField] private Vector2 positionToGoTo;
    private Vector3 velocity;
    public float smoothTime = 0.2f;
    public GameObject first;
    public GameObject second;

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
    private void Start()
    {
        multiplayerManager = MultiplayerManager.instance;
        playerList = multiplayerManager.players;
    }
    public void Update()
    {
        switch (direction)
        {
            case RaceDirection.UP:
                if (first.transform.position.y >= second.transform.position.y)
                {
                    positionToGoTo = first.transform.position;
                }
                else
                {
                    positionToGoTo = second.transform.position;
                }
                break;
            case RaceDirection.DOWN: 
                if (first.transform.position.y <= second.transform.position.y)
                {
                    positionToGoTo = first.transform.position;
                }
                else
                {
                    positionToGoTo = second.transform.position;
                }
                break;
            case RaceDirection.LEFT: 
                if (first.transform.position.x <= second.transform.position.x)
                {
                    positionToGoTo = first.transform.position;
                }
                else
                {
                    positionToGoTo = second.transform.position;
                }
                break;
            case RaceDirection.RIGHT: 
                if (first.transform.position.x >= second.transform.position.x)
                {
                    positionToGoTo = first.transform.position;
                }
                else
                {
                    positionToGoTo = second.transform.position;
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
