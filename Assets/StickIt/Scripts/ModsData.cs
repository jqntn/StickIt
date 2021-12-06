using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ModsData", menuName = "ScriptableObjects/ModsData")]
public class ModsData : ScriptableObject
{
    [Serializable]
    public class Mod
    {
        public string name;
        public List<string> maps;
        public bool isTutoDone;
    }
    public List<Mod> mods;
}