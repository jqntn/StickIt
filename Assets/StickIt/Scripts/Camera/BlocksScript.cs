using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocksScript : MonoBehaviour
{
    [Header("TEST______________________")]
    public bool isStartingDirect = true;
    public CameraData data;
    public float extendsFactor = 1.0f;
    public Vector3 offsets = new Vector3(0.0f, 0.0f, 0.0f);
    #region Grayed out
    private Bounds bounds = new Bounds();
    private Vector2 boundsPos  = new Vector2(0.0f, 0.0f);
    private Vector2 max = new Vector2(0.0f, 0.0f);
    private Vector2 dimension = new Vector2(0.0f, 0.0f);
    private Vector2 factors = new Vector2(0.0f, 0.0f);
    private Vector2 dimensionBase = new Vector2(0.0f, 0.0f);
    [SerializeField] private Vector3 parentPosition = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField] private List<Transform> childs = new List<Transform>();
    #endregion

    private void OnEnable()
    {
        max.x = 0;
        max.y = 0;
        bounds = new Bounds();
        Transform temp_width = transform;
        Transform temp_height = transform;
       
        // Create Bounds
        foreach (Transform child in transform)
        {
            bounds.Encapsulate(child.position);
            childs.Add(child);
            
        }

        parentPosition = transform.parent.position;
        boundsPos = bounds.center + offsets;

        // Calculate size of farthest children to add padding to bounds
        foreach(Transform child in transform)
        {
            Vector2 vCenterToChild = child.position - bounds.center;
            if (vCenterToChild.x > max.x)
            {
                max.x = child.position.x;
                temp_width = child;
            }

            if (vCenterToChild.y > max.y)
            {
                max.y = child.position.y;
                temp_height = child;
            }
        }

        Collider childCollider = temp_width.GetComponent<Collider>();
        if (childCollider == null)
        {
            childCollider = temp_width.gameObject.GetComponentInChildren<Collider>();
        }
        dimension.x = bounds.size.x + childCollider.bounds.size.x;
        childCollider = temp_height.GetComponent<Collider>();
        if (childCollider == null)
        {
            childCollider = temp_height.gameObject.GetComponentInChildren<Collider>();
        }
        dimension.y = bounds.size.y + childCollider.bounds.size.y;

        // Change dimension to respect aspect ratio
        factors.x = dimension.x / Utils.AspectRatio.x;
        factors.y = dimension.y / Utils.AspectRatio.y;
        float factor = factors.y;
        if (factors.x < factors.y)
        {
            //factor = factors.y;
        }

        dimension.x = Utils.AspectRatio.x * factor * extendsFactor;
        dimension.y = Utils.AspectRatio.y * factor * extendsFactor;

        // For debug
        dimensionBase.x = Utils.AspectRatio.x * factor;
        dimensionBase.y = Utils.AspectRatio.y * factor;

        GameEvents.OnSceneUnloaded.AddListener(GiveNewBounds);
    }
    void Start()
    {
        //Debug
        if(isStartingDirect) GiveNewBounds();
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
        state.SubscribeToCamera(boundsPos, dimension, data);
    }

    #region Debug
    protected virtual void OnDrawGizmos()
    {
        // Draw Camera Bounds
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(bounds.center + offsets, new Vector3(dimensionBase.x * extendsFactor, dimensionBase.y * extendsFactor, 1));
    }
    #endregion
}
