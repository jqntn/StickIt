using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private MultiplayerManager _multiplayerManager;
    public PlayerMouvement myMouvementScript;

    public MultiplayerManager.PlayerData myDatas;

    bool isDead;
   

    void Start()
    {
        _multiplayerManager = MultiplayerManager.instance;
        myMouvementScript = GetComponent<PlayerMouvement>();
        myMouvementScript.myPlayer = this;
    }

    public void Death()
    {
        isDead = true;
        myMouvementScript.enabled = false;       
        _multiplayerManager.alivePlayers.Remove(this);
        _multiplayerManager.deadPlayers.Add(this);
    }

    public void Respawn()
    {
        _multiplayerManager.alivePlayers.Add(this);
        _multiplayerManager.deadPlayers.Add(this);
        myMouvementScript.enabled = true;
    }
}
