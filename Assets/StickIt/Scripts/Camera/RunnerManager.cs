using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerManager : Level
{
    public RaceDirection direction;

    public static RunnerManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
}
