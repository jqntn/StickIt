using UnityEngine;

public class Deathmatch : Level
{
    protected override void StartMap()
    {
        base.StartMap();
        Debug.Log("Child Start Map");
    }
}
