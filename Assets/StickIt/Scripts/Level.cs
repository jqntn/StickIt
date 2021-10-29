using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Level : MonoBehaviour
{
    protected MultiplayerManager _multiplayerManager;
    protected GameManager _gameManager;
    public List<Player> winners;
    public Transform startingPos;

    void Start()
    {
        StartMap();
    }

    protected virtual void StartMap()
    {
        _multiplayerManager = MultiplayerManager.instance;
        _gameManager = GameManager.instance;
    }

    protected virtual void EndMap()
    {
        Debug.Log("End Map");
    }
}
