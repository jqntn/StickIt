using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class GameEvents
{
    // | Events
	public static CameraShakeEvent CameraShake_CEvent = new CameraShakeEvent();
	// | End Events

	public static UnityEvent OnSceneUnloaded = new UnityEvent();
	public static UnityEvent OnSwitchCamera = new UnityEvent();
}

public class CameraShakeEvent : UnityEvent<float> { }
public class CameraEvent : UnityEvent<CameraType> { }
// SHIFT + F12 : Search all occurence of an event
