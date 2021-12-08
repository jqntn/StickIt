using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
public class MMFeedbacksManager : MonoBehaviour
{
    public List<MMFeedbacks> feedbacksList = new List<MMFeedbacks>();
    private void Awake()
    {
        // | Listeners
		GameEvents.ShakeAppearChairEvent.AddListener(ShakeAppearChairCall);
		GameEvents.CameraShake_CEvent.AddListener(CameraShake_CCall);
		// | End Listeners
    }

	// | Calls
	public void ShakeAppearChairCall()
	{
		if (!feedbacksList[1].IsPlaying){
			feedbacksList[1].PlayFeedbacks();
		}
	}
	public void CameraShake_CCall(float duration = 1.0f, float intensity = 1.0f)
	{
		if (!feedbacksList[0].IsPlaying){
			float durationMultiplier = feedbacksList[0].TotalDuration / duration;
			feedbacksList[0].FeedbacksIntensity = intensity;
			feedbacksList[0].DurationMultiplier = durationMultiplier;
			feedbacksList[0].PlayFeedbacks();
		}
	}
	// | End Calls
}
