using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStateDriven : MonoBehaviour
{
    public List<CameraState> statesList = new List<CameraState>();

    [Header("---- DEBUG ----- ")]
    public CameraState currentState;

    private void Awake()
    {
        // Debug
        foreach(CameraState state in statesList)
        {
            if (state.isActiveAndEnabled)
            {
                currentState = state;
                break;
            }
        }
        // -------
    }

    private void Update()
    {
        if(currentState == null)
        {
            foreach (CameraState state in statesList)
            {
                if (state.isActiveAndEnabled)
                {
                    currentState = state;
                    break;
                }
            }
        }
    }
    public void DeactivateAllCameraState()
    {
        foreach (CameraState state in statesList)
        {
            state.gameObject.SetActive(false);
        }
    }
    public void SwitchStates(CameraType type)
    {
        currentState.gameObject.SetActive(false);
        currentState = null;
        foreach(CameraState state in statesList)
        {
            if(state.GetCameraType() == type)
            {
                currentState = state;
                state.gameObject.SetActive(true);
                break;
            }
        }

        GameEvents.OnSwitchCamera.Invoke();
    }
}