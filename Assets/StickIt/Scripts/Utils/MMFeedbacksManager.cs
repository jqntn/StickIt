using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
public class MMFeedbacksManager : MonoBehaviour
{
    public List<MMFeedbacks> feedbacksList = new List<MMFeedbacks>();
    private void Awake()
    {
        // | Listeners
		GameEvents.MMFeelOrganicEvent.AddListener(MMFeelOrganicCall);
		GameEvents.MMFeelChocEvent.AddListener(MMFeelChocCall);
		// | End Listeners
    }

    // | Calls
	public void MMFeelOrganicCall()
	{
		feedbacksList[1].PlayFeedbacks();
	}
	public void MMFeelChocCall()
	{
		feedbacksList[0].PlayFeedbacks();
	}
	// | End Calls
}
