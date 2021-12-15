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
	public void ShakeAppearChairCall(float duration, float intensity)
	{
		if (!feedbacksList[1].IsPlaying){
			float durationMultiplier = duration / feedbacksList[1].TotalDuration;
			feedbacksList[1].FeedbacksIntensity = intensity;
			feedbacksList[1].DurationMultiplier = durationMultiplier;
			feedbacksList[1].PlayFeedbacks();
		}
	}

	public void CameraShake_CCall(float duration = 1.0f, float intensity = 1.0f)
	{
		if (!feedbacksList[0].IsPlaying){
			feedbacksList[0].DurationMultiplier = 1.0f;
			float durationMultiplier = feedbacksList[0].TotalDuration / duration;
			feedbacksList[0].FeedbacksIntensity = intensity;
			feedbacksList[0].DurationMultiplier = durationMultiplier;
			feedbacksList[0].PlayFeedbacks();
		}
	}
	// | End Calls

	public void CameraShake_C2(MMFeedbacksData data)
    {
		feedbacksList[0].PlayFeedbacks();
    }
}
