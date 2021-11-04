using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraBarycenter : CameraState
{
    [Header("----- Debug -----")]
    [SerializeField] private Vector3 barycenter;        // make it local before sending

    protected override void Update()
    {
        base.Update();
        if (canMove) { UpdateBarycenter(); }

    }

    private void UpdateBarycenter()
    {
        barycenter = new Vector3(0.0f, 0.0f, cam.transform.position.z);
        foreach(Player player in playerList)
        {
            barycenter += player.transform.position;
        }
        barycenter /= playerList.Count;

        // Reset Z
        barycenter = new Vector3(barycenter.x, barycenter.y, cam.transform.position.z);

        if (bounds != null)
        {
            barycenter.x = Mathf.Clamp(barycenter.x, -bounds_X, bounds_X);
            barycenter.y = Mathf.Clamp(barycenter.y, -bounds_Y, bounds_Y);
        }

        // Freeze X axis
        if (freezeX)
        {
            barycenter = new Vector3(cam.transform.position.x, barycenter.y, cam.transform.position.z);
        }

        // Freeze Y axis
        if (freezeY)
        {
            barycenter = new Vector3(barycenter.x, cam.transform.position.y, cam.transform.position.z);
        }

        // Update Position to Go To
        float newPos_X = Mathf.Clamp(barycenter.x, min_bounds_X, max_bounds_X);
        float newPos_Y = Mathf.Clamp(barycenter.y, min_bounds_Y, max_bounds_Y);
        positionToGoTo = new Vector3(
            newPos_X,
            newPos_Y,
            cam.transform.position.z);

        //Debug
        float val = 0;
        foreach (Player player in playerList)
        {
            Debug.DrawLine(player.transform.position, barycenter, Color.red + new Color(-val, val, 0));
            val += 0.20f;
        }
    }

}
