using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStateDriven : MonoBehaviour
{
    public List<CameraState> statesList = new List<CameraState>();
    public Dictionary<int, CameraState> statesDico = new Dictionary<int, CameraState>();

    [Header("------- DEBUG ------")]
    public CameraState currentState;

    private void Awake()
    {
        foreach(CameraState state in statesList)
        {

        }
    }
    public void SwitchStates(short index)
    {
        currentState = statesList[index];
        CameraState camState = null;
        statesDico.TryGetValue(index, out camState);
        Debug.Log("camStates = " + statesDico[index]);
        Debug.Log("Cam State = " + camState);
    }
}
