using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deathmatch : Level
{
    public override void StartMap()
    {
        base.StartMap();
        Debug.Log("Child Start Map");
    }
}
