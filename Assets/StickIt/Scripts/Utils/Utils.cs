using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static CameraType GetCameraType(string curMap)
    {
        CameraType type = CameraType.STATIC;
        switch (curMap)
        {
            case "MusicalChairs":   type = CameraType.BARYCENTER;   break;
            case "Deathmatch":      type = CameraType.BARYCENTER;   break;
            case "Runner":          type = CameraType.RUNNER;       break;
        }
        return type;
    }
}
