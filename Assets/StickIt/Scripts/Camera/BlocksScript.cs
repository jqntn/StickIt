using System.Collections;
using UnityEngine;

public class BlocksScript : MonoBehaviour
{
    public float extendsFactor = 1.0f;
    #region Grayed out
    [SerializeField] private Bounds bounds = new Bounds();
    [SerializeField] private Vector2 boundsPos  = new Vector2(0.0f, 0.0f);
    [SerializeField] private Vector2 max = new Vector2(0.0f, 0.0f);
    [SerializeField] private Vector2 dimension = new Vector2(0.0f, 0.0f);
    [SerializeField] private float factor = 0.0f;
    #endregion

    private void OnEnable()
    {
        max.x = 0;
        max.y = 0;
        bounds = new Bounds();
        Transform temp_width = transform;
        Transform temp_height = transform;
        foreach (Transform child in transform)
        {
            bounds.Encapsulate(child.position);
            if(child.position.x > max.x)
            {
                max.x = child.position.x;
                temp_width = child;
            }

            if(child.position.y > max.y)
            {
                max.y = child.position.y;
                temp_height = child;

            }
        }

        Collider childCollider = temp_width.GetComponent<Collider>();
        dimension.x = bounds.size.x + childCollider.bounds.size.x;
        childCollider = temp_height.GetComponent<Collider>();
        dimension.y = bounds.size.y + childCollider.bounds.size.y;

        // Change dimension to respect aspect ratio
        factor = dimension.x / Utils.AspectRatio.x;
        dimension.x = Utils.AspectRatio.x * factor * extendsFactor;
        dimension.y = Utils.AspectRatio.y * factor * extendsFactor;
        boundsPos = bounds.center;

        GameEvents.OnSceneUnloaded.AddListener(GiveNewBounds);
    }
    void Start()
    {
        //Debug
        GiveNewBounds();
    }
    private void GiveNewBounds()
    {
        StartCoroutine(OnGiveNewBounds());
    }
    private IEnumerator OnGiveNewBounds()
    {
        while (Camera.main.GetComponent<CameraStateDriven>().currentState == null)
        {
            yield return null;
        }
        CameraState state = Camera.main.GetComponentInChildren<CameraState>();
        state.SubscribeToCamera(boundsPos, dimension);
    }

    #region Debug
    //protected virtual void OnDrawGizmosSelected()
    //{
    //    // Draw Camera Bounds
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireCube(boundsPos, new Vector3(dimension.x, dimension.y, 1));
    //}
    #endregion
}
