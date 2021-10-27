using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    // Start is called before the first frame update

    protected GameManager _gameManager;
    protected MultiplayerManager _multiplayerManager;
    public List<Player> winners;
    public Transform startingPos;

    void Start()
    {
        StartMap();
    }
    protected virtual void StartMap()
    {
        _multiplayerManager = MultiplayerManager.instance;
        _multiplayerManager.playersStartingPos = startingPos;
        _multiplayerManager.InstantiatePlayers();
        _gameManager = GameManager.instance;
    }
}
