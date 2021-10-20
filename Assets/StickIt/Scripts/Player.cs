using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private MultiplayerManager _multiplayerManager;

    public int id;

    void Start()
    {
        _multiplayerManager = MultiplayerManager.instance;
    }
}
