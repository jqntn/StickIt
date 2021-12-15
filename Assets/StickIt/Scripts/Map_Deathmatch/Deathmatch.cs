using UnityEngine;

public class Deathmatch : Level
{
    protected override void Awake()
    {
        if(AudioManager.instance == null) { return; }
        AudioManager.instance.SwitchAmbianceToSummer(gameObject);
    }
    protected override void StartMap()
    {
        base.StartMap();
    }
}
