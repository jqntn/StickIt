using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using UnityEngine.Events;
public class CameraShakeScript : MonoBehaviour
{
    private MMFeedback cameraShake;

    public UnityEvent OnCameraShake;
    public static CameraShakeScript Instance;
    private void Awake()
    {
        cameraShake = GetComponent<MMFeedback>();

        // Listeners | TestShakeCall.cs
        GameEvents.CameraShakeEvent.AddListener(ShakeCamera);
    }
    public void ShakeCamera()
    {
        cameraShake?.Play(new Vector3(0,0,0));
        PlayOnCameraShake();
    }

    public void PlayOnCameraShake()
    {
        if(OnCameraShake != null)
        {
            OnCameraShake.Invoke();
        }
    }
}
