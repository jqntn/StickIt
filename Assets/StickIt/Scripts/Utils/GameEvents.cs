using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class GameEvents
{
    // | Events
	public static FeelEvent ShakeAppearChairEvent = new FeelEvent();
	public static FeelEvent ShakeManetteEvent = new FeelEvent();
	public static FeelEvent CameraShake_CEvent = new FeelEvent();
	// | End Events

	public static UnityEvent OnSceneUnloaded = new UnityEvent();
	public static UnityEvent OnSwitchCamera = new UnityEvent();
}

public class FeelEvent : UnityEvent<float, float> { }
public class CameraEvent : UnityEvent<CameraType> { }
// SHIFT + F12 : Search all occurence of an event
