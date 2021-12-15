using UnityEngine;

public class Deathmatch : Level
{
    protected override void Awake()
    {
        AudioManager.instance.SwitchAmbianceToSummer(gameObject);
    }
    protected override void StartMap()
    {
        base.StartMap();
    }
}
