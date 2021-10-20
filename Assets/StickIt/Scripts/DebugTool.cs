using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
class DebugTool : MonoBehaviour
{
    int BW = 600, BH = 60, TH = 40, SW = 400, SH;
    int LD = 600, RD = 2;
    int GSGW = 1000, GSGH = 600;
    int GSBW = 100, GSBH = 20;
    int GL = -2, GLL;
    int GSSM, GSSR;
    bool debug = true;
    int GSMS;
    bool GM, GP;
    public KeyCode k;
    void Start()
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        string[] scenes = new string[sceneCount];
        for (int i = 0; i < sceneCount; i++)
        {
            scenes[i] = SceneUtility.GetScenePathByBuildIndex(i);
        }
    }
    void Update()
    {
        // debug ^= Input.GetKeyDown(KeyCode.F1);
    }
    // void OnGUI()
    // {
    //     if (debug) GUI.Label(new Rect(0, -5, LD, LD), "FPS: " + Mathf.RoundToInt(1 / Time.unscaledDeltaTime));
    //     if (GL != -3)
    //     {
    //         if (GL == -2)
    //         {
    //             if (GUI.Button(new Rect((Screen.width - BW) / 2, (Screen.height - BH) / 2 - 120, BW, BH), "RESUME")) GL = -3;
    //             if (GUI.Button(new Rect((Screen.width - BW) / 2, (Screen.height - BH) / 2 - 40, BW, BH), "SETTINGS"))
    //             {
    //                 GL = 0;
    //                 GLL = -2;
    //             }
    //             if (GUI.Button(new Rect((Screen.width - BW) / 2, (Screen.height - BH) / 2 + 40, BW, BH), "MENU")) GL = -1;
    //             if (GUI.Button(new Rect((Screen.width - BW) / 2, (Screen.height - BH) / 2 + 120, BW, BH), "QUIT")) Application.Quit();
    //         }
    //         else if (GL == -1)
    //         {
    //             if (GUI.Button(new Rect((Screen.width - BW) / 2, (Screen.height - BH) / 2, BW, BH), "SETTINGS"))
    //             {
    //                 GL = 0;
    //                 GLL = -1;
    //             }
    //             GUI.Button(new Rect((Screen.width - BW) / 2, (Screen.height - BH) / 2 - 80, BW, BH), "PLAY");
    //             if (GUI.Button(new Rect((Screen.width - BW) / 2, (Screen.height - BH) / 2 + 80, BW, BH), "QUIT")) Application.Quit();
    //         }
    //         else
    //         {
    //             GUI.BeginGroup(new Rect((Screen.width - GSGW) / 2, (Screen.height - GSGH) / 2, GSGW, GSGH));
    //             GUI.Box(new Rect(0, 0, GSGW, GSGH), GUIContent.none);
    //             GL = GUI.Toolbar(new Rect(0, 0, GSGW, TH), GL, new string[] { "VIDEO", "AUDIO", "MOUSE", "KEYBOARD" });
    //             if (GUI.Button(new Rect(40, GSGH - 40, GSBW, GSBH), "BACK")) GL = GLL;
    //             if (GUI.Button(new Rect(160, GSGH - 40, GSBW, GSBH), "APPLY")) Screen.SetResolution(Screen.resolutions[GSSR].width, Screen.resolutions[GSSR].height, (FullScreenMode) GSSM, Screen.resolutions[GSSR].refreshRate);
    //             if (GUI.Button(new Rect(GSGW - GSBW - 40, GSGH - 40, GSBW, GSBH), "RESET"))
    //             {
    //                 PlayerPrefs.DeleteAll();
    //                 Start();
    //             }
    //             if (GL == 0)
    //             {
    //                 GUI.BeginGroup(new Rect(40, 0, GSGW, GSGH));
    //                 GUI.Label(new Rect(0, 100, LD, LD), ((FullScreenMode) GSSM).ToString());
    //                 GUI.Label(new Rect(0, 140, LD, LD), Screen.resolutions[GSSR].ToString().Substring(0, Screen.resolutions[GSSR].ToString().Length - 2));
    //                 GUI.Label(new Rect(0, 180, LD, LD), "FOV: " + C.fieldOfView);
    //                 GUI.Label(new Rect(0, 220, LD, LD), "VSYNC: " + QualitySettings.vSyncCount);
    //                 GUI.Label(new Rect(0, 260, LD, LD), "MSAA: " + QualitySettings.antiAliasing);
    //                 GUI.Label(new Rect(0, 300, LD, LD), "AF: " + QualitySettings.anisotropicFiltering);
    //                 GUI.Label(new Rect(0, 340, LD, LD), "MasterTextureLimit: " + QualitySettings.masterTextureLimit);
    //                 GUI.Label(new Rect(0, 380, LD, LD), "ShadowResolution: " + QualitySettings.shadowResolution);
    //                 GUI.EndGroup();
    //                 GUI.BeginGroup(new Rect(240, 0, GSGW, GSGH));
    //                 GSSM = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(0, 105, SW, SH), GSSM, 0, 3));
    //                 GSSR = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(0, 145, SW, SH), GSSR, 0, Screen.resolutions.Length - 1));
    //                 C.fieldOfView = Mathf.Round(GUI.HorizontalSlider(new Rect(0, 185, SW, SH), C.fieldOfView / 5, 10, 20)) * 5;
    //                 QualitySettings.vSyncCount = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(0, 225, SW, SH), QualitySettings.vSyncCount, 0, 2));
    //                 QualitySettings.antiAliasing = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(0, 265, SW, SH), QualitySettings.antiAliasing / 2, 0, 4)) * 2;
    //                 QualitySettings.anisotropicFiltering = (AnisotropicFiltering) (Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(0, 305, SW, SH), (int) QualitySettings.anisotropicFiltering / 2, 0, 1)) * 2);
    //                 QualitySettings.masterTextureLimit = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(0, 345, SW, SH), QualitySettings.masterTextureLimit, 3, 0));
    //                 QualitySettings.shadowResolution = (ShadowResolution) Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(0, 385, SW, SH), (int) QualitySettings.shadowResolution, 0, 3));
    //                 GUI.EndGroup();
    //             }
    //             if (GL == 1)
    //             {
    //                 GUI.BeginGroup(new Rect(40, 0, GSGW, GSGH));
    //                 GUI.Label(new Rect(0, 100, LD, LD), "MAIN: " + AudioListener.volume * 100);
    //                 GUI.Label(new Rect(0, 140, LD, LD), "MUSIC: " + GSAM.volume * 100);
    //                 GUI.Label(new Rect(0, 180, LD, LD), "EFFECTS: " + GSAE.volume * 100);
    //                 GUI.EndGroup();
    //                 GUI.BeginGroup(new Rect(240, 0, GSGW, GSGH));
    //                 AudioListener.volume = Mathf.Round(GUI.HorizontalSlider(new Rect(0, 105, SW, SH), AudioListener.volume / 5 * 100, 0, 20)) * 5 / 100;
    //                 GSAM.volume = Mathf.Round(GUI.HorizontalSlider(new Rect(0, 145, SW, SH), GSAM.volume / 5 * 100, 0, 20)) * 5 / 100;
    //                 GSAE.volume = Mathf.Round(GUI.HorizontalSlider(new Rect(0, 185, SW, SH), GSAE.volume / 5 * 100, 0, 20)) * 5 / 100;
    //                 GUI.EndGroup();
    //             }
    //             if (GL == 2)
    //             {
    //                 GUI.Label(new Rect(40, 100, LD, LD), "SENSIVITY: " + GSMS);
    //                 GSMS = (int) GUI.HorizontalSlider(new Rect(240, 105, SW, SH), GSMS / 5, 2, 20) * 5;
    //             }
    //             GUI.EndGroup();
    //         }
    //     }
    // }
}