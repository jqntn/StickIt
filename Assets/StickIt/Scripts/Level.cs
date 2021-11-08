using System.Collections.Generic;
using UnityEngine;
public class Level : MonoBehaviour
{
    public List<Player> winners;
    public virtual void Init() { }
    public virtual void EndMap() { }
}