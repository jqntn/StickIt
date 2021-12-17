using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ModsData", menuName = "ScriptableObjects/ModsData")]
public class ModsData : ScriptableObject
{
    public List<Mod> mods;
}

[Serializable]
public class Mod
{
    public string name;
    public List<string> maps;
    public bool isTutoDone;
}