using UnityEngine;
public class SettingsVideo : MonoBehaviour
{
    private bool vsync;
    public void Fullscreen() => Screen.fullScreen = !Screen.fullScreen;
    public void Vsync()
    {
        vsync ^= true;
        QualitySettings.vSyncCount = vsync ? 1 : 0;
    }
}