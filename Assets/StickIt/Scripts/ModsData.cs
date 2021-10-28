using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[CreateAssetMenu(fileName = "ModsData", menuName = "ScriptableObjects/ModsData")]
class ModsData : ScriptableObject
{
    [Serializable]
    public class Mod
    {
        public string name;
        public List<Scene> maps;
    }
    public List<Mod> mods;
}