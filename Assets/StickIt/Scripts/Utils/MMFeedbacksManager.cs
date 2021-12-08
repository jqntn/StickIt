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
	public void CameraShake_CCall(float duration, float intensity)
	{
		if (!feedbacksList[0].IsPlaying){
			Debug.Log(duration);
			float durationMultiplier = duration / (float)feedbacksList[0].TotalDuration;
			Debug.Log(durationMultiplier);
			feedbacksList[0].FeedbacksIntensity = intensity;
			feedbacksList[0].DurationMultiplier = durationMultiplier;
			feedbacksList[0].PlayFeedbacks();
		}
	}
	// | End Calls
}
