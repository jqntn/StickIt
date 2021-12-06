using UnityEngine;
using UnityEngine.UI;

public class SettingsAudio : MonoBehaviour
{
    int volFactor = 100;
    public Slider masterVol, musicVol, sfxVol, envirVol;

    public void SetMasterVol()
    {
        AkSoundEngine.SetRTPCValue("RTPC_Volume_Global", masterVol.value * volFactor);
    }
    public void SetMusicVol()
    {
        AkSoundEngine.SetRTPCValue("RTPC_Volume_Music", musicVol.value * volFactor);
    }
    public void SetSFXVol()
    {
        AkSoundEngine.SetRTPCValue("RTPC_Volume_SFX", sfxVol.value * volFactor);
    }
    public void SetEnvironmentVol()
    {
        AkSoundEngine.SetRTPCValue("RTPC_Volume_Environment", envirVol.value * volFactor);
    }

}
