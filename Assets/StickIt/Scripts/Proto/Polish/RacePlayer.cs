using UnityEngine;

public class RacePlayer : MonoBehaviour
{
    [Header("------------ DEBUG -----------")]
    public int raceCheckpoint = 0;

    public void ResetCheckpoint()
    {
        raceCheckpoint = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        CameraCheckpoint checkpoint = other.GetComponent<CameraCheckpoint>();
        if(other != null)
        {
            raceCheckpoint = checkpoint.number;
        }
    }
}
