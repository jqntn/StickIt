using UnityEngine;

public static class Utils
{
    private static Vector2 display_resolution = new Vector2(Display.main.systemWidth, Display.main.systemHeight);
    public static Vector2 DisplayResolution { get => display_resolution; }
    private static Vector2 rendering_resolution = new Vector2(Display.main.renderingWidth, Display.main.renderingHeight);
    public static Vector2 RenderingResolution { get => rendering_resolution; }
    private static Vector2 aspect_ratio = (Camera.main.aspect >= 1.7f) ? new Vector2(16, 9) : 
                                          (Camera.main.aspect >= 1.6f) ? new Vector2(16, 10) : 
                                          (Camera.main.aspect >= 1.5f) ? new Vector2(3, 2) : 
                                                                         new Vector2(4, 3);
    public static Vector2 AspectRatio { get => aspect_ratio; }

    public static CameraType GetCameraType(string curMap)
    {
        CameraType type = CameraType.BARYCENTER;
        switch (curMap)
        {
            case "MusicalChairs":   type = CameraType.BARYCENTER;   break;
            case "Deathmatch":      type = CameraType.BARYCENTER;   break;
            case "Runner":          type = CameraType.RUNNER;       break;
        }

        return type;
    }
}
