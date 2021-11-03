using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterCamera : MonoBehaviour
{
    public static CenterCamera Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
}
