using UnityEngine;
using UnityEngine.UI;
public class SettingsVideo : MonoBehaviour
{
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Toggle vsyncToggle;
    public void Fullscreen() => Screen.fullScreen = fullscreenToggle.isOn;
    public void Vsync() => QualitySettings.vSyncCount = vsyncToggle.isOn ? 1 : 0;
}