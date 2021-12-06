using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(this);
    }
    public void PlayMusicMenu(GameObject go)
    {
        AkSoundEngine.PostEvent("Event_M_Play_Menu", go);
    }
    #region SFXSounds
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
}
