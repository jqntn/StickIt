using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
public class MMFeedbacksManager : MonoBehaviour
{
    public List<MMFeedbacks> feedbacksList = new List<MMFeedbacks>();
    private void Awake()
    {
        // | Listeners
		GameEvents.CameraShake_CEvent.AddListener(CameraShake_CCall);
		// | End Listeners
    }

	// | Calls
	public void CameraShake_CCall(float durationMultiplier = 1.0f, float intensity = 1.0f)
	{
		if (!feedbacksList[0].IsPlaying){
			feedbacksList[0].FeedbacksIntensity = intensity;
			feedbacksList[0].DurationMultiplier = durationMultiplier;
			feedbacksList[0].PlayFeedbacks();
		}
	}
	// | End Calls
}
