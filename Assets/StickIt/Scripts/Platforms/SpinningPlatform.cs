using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningPlatform : MonoBehaviour
{
    Rigidbody rb;
    bool playSound;
    bool security;
    // Start is called before the first frame update
    void Start()
    {
        security = false;
        if(TryGetComponent<Rigidbody>(out rb))
            rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.angularVelocity.magnitude < 0.2)
        {
            AkSoundEngine.PostEvent("Stop_SFX_Platform_Rotate", gameObject);
            playSound = false;
            security = false;
        }
        else
        {
            playSound = true;
        }
        if (playSound && !security)
        {
            AkSoundEngine.PostEvent("Play_SFX_Platform_Rotate", gameObject);
            security = true;
        }
    }
}
