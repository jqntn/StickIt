using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScoreSystem
{
    public bool doDeadGain = false;
    public ScoreType type = ScoreType.Fixed;
}
public class FixedScore : ScoreSystem
{
    
}
public enum ScoreType
{
    Fixed,
    Divide,
    Percentage,
    Dynamic,
}
