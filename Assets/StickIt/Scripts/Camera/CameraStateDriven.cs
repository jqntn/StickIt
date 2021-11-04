using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStateDriven : MonoBehaviour
{
    public List<CameraState> statesList = new List<CameraState>();

    [Header("------- DEBUG ------")]
    public CameraState currentState;

    public void DeactivateAllCameraState()
    {
        foreach (CameraState state in statesList)
        {
            state.gameObject.SetActive(false);
        }
    }
    public void SwitchStates(CameraType type)
    {

        DeactivateAllCameraState();
        foreach(CameraState state in statesList)
        {
            if(state.GetCameraType() == type)
            {
                currentState = state;
                state.gameObject.SetActive(true);
                break;
            }
        }

        Debug.Log("Switch State : " + currentState.name);
    }

    public void SwitchToRunner()
    {
        SwitchStates(CameraType.RUNNER);
    }
    public void SwitchToFollow()
    {
        SwitchStates(CameraType.FOLLOW);
    }
    public void SwitchToBarycenter()
    {
        SwitchStates(CameraType.BARYCENTER);
    }
}

public enum CameraType
{
    BARYCENTER,
    FOLLOW,
    RUNNER,
}
