using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class TestShakeCall : MonoBehaviour
{
    public UnityEvent OnSomethingHappening;

    public void SomethingIsHappening()
    {
        if(OnSomethingHappening != null)
        {
            OnSomethingHappening?.Invoke();
        }
    }
}
