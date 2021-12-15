using System;
using UnityEngine;
using UnityEngine.UI;
public class Save : MonoBehaviour
{
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Toggle vsyncToggle;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider effectsSlider;
    [SerializeField] private Slider environmentSlider;
    private void Start() => LoadPrefs();
    private void OnDestroy() => SavePrefs();
    private void OnApplicationQuit() => SavePrefs();
    private void LoadPrefs()
    {
        fullscreenToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("fullscreen", 1));
        vsyncToggle.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("vsync", 1));
        masterSlider.value = PlayerPrefs.GetFloat("masterVol", 1);
        musicSlider.value = PlayerPrefs.GetFloat("musicVol", .5f);
        effectsSlider.value = PlayerPrefs.GetFloat("effectsVol", .5f);
        environmentSlider.value = PlayerPrefs.GetFloat("environmentVol", .5f);
    }
    private void SavePrefs()
    {
        PlayerPrefs.SetInt("fullscreen", Convert.ToInt32(fullscreenToggle.isOn));
        PlayerPrefs.SetInt("vsync", Convert.ToInt32(vsyncToggle.isOn));
        PlayerPrefs.SetFloat("masterVol", masterSlider.value);
        PlayerPrefs.SetFloat("musicVol", musicSlider.value);
        PlayerPrefs.SetFloat("effectsVol", effectsSlider.value);
        PlayerPrefs.SetFloat("environmentVol", environmentSlider.value);
    }
}