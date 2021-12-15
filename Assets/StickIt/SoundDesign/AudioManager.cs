using UnityEngine;

public class AudioManager : Unique<AudioManager>
{
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
        AkSoundEngine.PostEvent("Play_SFX_S_IceSlide", go);
    }
    #endregion


    #region Environment
    public void PlayAmbiantSounds(GameObject go)
    {
        AkSoundEngine.PostEvent("Event_Play_Ambiance", go);
    }
    public void SwitchAmbianceToFall()
    {
        AkSoundEngine.SetState("State_Season","State_Season_Fall");
    }
    public void SwitchAmbianceToSummer()
    {
        AkSoundEngine.SetState("State_Season", "State_Season_Summer");
    }
    #endregion
}
