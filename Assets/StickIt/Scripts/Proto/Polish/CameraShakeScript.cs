using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using UnityEngine.Events;
public class CameraShakeScript : MonoBehaviour
{
    private MMFeedbacks cameraShake;

    public UnityEvent OnCameraShake;
    public static CameraShakeScript Instance;
    private void Awake()
    {
        cameraShake = GetComponent<MMFeedbacks>();

        // Listeners | TestShakeCall.cs
        GameEvents.CameraShakeEvent.AddListener(ShakeCamera);
    }
    public void ShakeCamera()
    {
        cameraShake?.PlayFeedbacks();
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
