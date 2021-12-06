using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public void PlayMusicMenu()
    {
        AkSoundEngine.PostEvent("Event_M_Play_Menu", gameObject);
    }
}
