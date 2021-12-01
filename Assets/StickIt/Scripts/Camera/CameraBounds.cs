using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    public static CameraBounds Instance { get; set; }

    private void OnEnable()
    {
        Instance = this;
    }
}
