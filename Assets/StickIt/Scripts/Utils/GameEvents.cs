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
}

public class CameraShakeEvent : UnityEvent<float> { }
// SHIFT + F12 : Search all occurence of an event
