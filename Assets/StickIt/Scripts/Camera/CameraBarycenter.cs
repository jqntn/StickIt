using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraBarycenter : CameraState
{

    protected override void Update()
    {
        base.Update();
        if (canMove) { UpdateBarycenter(); }

    }

    private void UpdateBarycenter()
    {
        base.barycenter = new Vector2(0.0f, 0.0f);
        foreach(Player player in playerList)
        {
            base.barycenter += player.transform.position;
        }
        base.barycenter /= playerList.Count;

        // Freeze X axis
        if (freezeX)
        {
            base.barycenter = new Vector2(transform.parent.position.x, base.barycenter.y);
        }

        // Freeze Y axis
        if (freezeY)
        {
            base.barycenter = new Vector2(base.barycenter.x, transform.parent.position.y);
        }

        // Clamp Value into bounds
        float newPos_X = Mathf.Clamp(base.barycenter.x, min_bounds_X, max_bounds_X);
        float newPos_Y = Mathf.Clamp(base.barycenter.y, min_bounds_Y, max_bounds_Y);

        // Update Position to Go To
        positionToGoTo = new Vector3(
            newPos_X,
            newPos_Y,
            transform.parent.position.z);

        //Debug
        float val = 0;
        foreach (Player player in playerList)
        {
            Debug.DrawLine(player.transform.position, base.barycenter, Color.red + new Color(-val, val, 0));
            val += 0.20f;
        }
    }
}
