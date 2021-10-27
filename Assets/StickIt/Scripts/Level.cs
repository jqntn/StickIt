using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform startingPos;

    void Start()
    {
        MultiplayerManager.instance.playersStartingPos = startingPos;
        MultiplayerManager.instance.InstantiatePlayers();
    }


}
