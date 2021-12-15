using UnityEngine;

public class AudioManager : Unique<AudioManager>
{
    private void Start()
    {
        AkSoundEngine.PostEvent("Play_Music_Main", gameObject);
        AkSoundEngine.PostEvent("Stop_Music_End", gameObject);
    }
    public void PlayMusicMenu(GameObject go)
    {
        AkSoundEngine.PostEvent("Event_M_Play_Menu", go);
    }
    #region SLIME SOUNDS
    public void PlayCollisionSounds(GameObject go)
    {
        AkSoundEngine.PostEvent("Play_SFX_S_Collision", go);
    }
    public void PlayJumpSounds(GameObject go)
    {
        AkSoundEngine.PostEvent("Play_SFX_S_Jump", go);
    }
    public void PlayLandSounds(GameObject go)
    {
        AkSoundEngine.PostEvent("Play_SFX_S_Land", go);
    }
    public void PlayDeathSounds(GameObject go)
    {
        AkSoundEngine.PostEvent("Play_SFX_S_Death", go);
    }
    #endregion

    #region UI Sounds
    public void PlayMoveSounds(GameObject go)
    {
        AkSoundEngine.PostEvent("Play_SFX_UI_Move", go);
    }
    public void PlayReturnSounds(GameObject go)
    {
        AkSoundEngine.PostEvent("Play_SFX_UI_Return", go);
    }
    public void PlaySubmitSounds(GameObject go)
    {
        AkSoundEngine.PostEvent("Play_SFX_UI_Submit", go);
    }
    #endregion


    #region SFX GENERAL
    public void PlayBounceShroomSounds(GameObject go)
    {
        AkSoundEngine.PostEvent("Play_SFX_S_BounceMushroom", go);
    }
    public void PlayBrambleImpactSounds(GameObject go)
    {
        AkSoundEngine.PostEvent("Play_SFX_S_BrambleImpact", go);
    }
    public void PlayGainShieldSounds(GameObject go)
    {
        AkSoundEngine.PostEvent("Play_SFX_S_GainShield", go);
    }
    public void PlayIceSlideSounds(GameObject go)
    {
       
    }
    #endregion


    #region Environment
    public void PlayAmbiantSounds(GameObject go)
    {
        Debug.Log("Play Ambiance");
        AkSoundEngine.PostEvent("Event_Play_Ambiance", go);
    }
    public void SwitchAmbianceToFall(GameObject go)
    {
        AkSoundEngine.PostEvent("Set_State_Season_Fall", go);
    }
    public void SwitchAmbianceToSummer(GameObject go)
    {
        AkSoundEngine.PostEvent("Set_State_Season_Summer", go);
    }
    #endregion
}
