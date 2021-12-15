using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
public class MMFeedbacksManager : MonoBehaviour
{
    public List<MMFeedbacks> feedbacksList = new List<MMFeedbacks>();

	[Header("DEBUG______________________________________________")]
	[SerializeField] private List<float> startDurationList = new List<float>(); 
    private void Awake()
    {
        // | Listeners
		GameEvents.ShakeAppearChairEvent.AddListener(ShakeAppearChairCall);
		GameEvents.CameraShake_CEvent.AddListener(CameraShake_CCall);
		// | End Listeners

		foreach(MMFeedbacks feedback in feedbacksList)
        {
			startDurationList.Add(feedback.TotalDuration);
        }
    }

	// | Calls
	public void ShakeAppearChairCall(float duration, float intensity)
	{
		if (!feedbacksList[1].IsPlaying){
			float durationMultiplier = Mathf.Sqrt(duration / startDurationList[1]);
			feedbacksList[1].FeedbacksIntensity = intensity;
			feedbacksList[1].DurationMultiplier = durationMultiplier;
			feedbacksList[1].PlayFeedbacks();
		}
	}
	public void CameraShake_CCall(float duration, float intensity)
	{
		if (!feedbacksList[0].IsPlaying){
			float durationMultiplier = Mathf.Sqrt(duration / startDurationList[0]);
			feedbacksList[0].FeedbacksIntensity = intensity;
			feedbacksList[0].DurationMultiplier = durationMultiplier;
			feedbacksList[0].PlayFeedbacks();
		}
	}
	// | End Calls
}
