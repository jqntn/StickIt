using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BlocksScript : MonoBehaviour
{
    [Header("TEST______________________")]
    public bool isStartingDirect = true;

    [Header("DATA______________________")]
    public CameraData data;
    public float extendsFactor = 1.0f; 
    public Vector3 offsets = new Vector3(0.0f, 0.0f, 0.0f);

    #region Grayed out
    [SerializeField] private Bounds bounds = new Bounds();
    [SerializeField] private Vector2 boundsPos  = new Vector2(0.0f, 0.0f);
    [SerializeField] private Vector2 dimension = new Vector2(0.0f, 0.0f);
    [SerializeField] private Vector2 factors = new Vector2(0.0f, 0.0f);
    [SerializeField] private Vector2 dimensionBase = new Vector2(0.0f, 0.0f);
    [SerializeField] private List<Transform> childs = new List<Transform>();
    #endregion
    public Vector2 Dimension { get => dimension; }
    public static BlocksScript Instance { get; set; }
    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().name == "100_EndScene")
        {
            Instance = this;
        }

        bounds = new Bounds(transform.position, new Vector3(1.0f, 1.0f, 1.0f));
        // Create Bounds
        foreach (Transform child in transform)
        {
            Collider childCollider = child.GetComponent<Collider>();
            if(childCollider == null) { childCollider = child.GetComponentInChildren<Collider>(); }

            Bounds childBounds = childCollider.bounds;
            bounds.Encapsulate(childBounds);
            childs.Add(child); ;
        }

        boundsPos = bounds.center + offsets;

        // Get new Height depending of width
        dimension.x = bounds.size.x;
        dimension.y = dimension.x / Camera.main.aspect;
        
        // If too small on height > get new width
        if(dimension.y < bounds.size.y)
        {
            dimension.y = bounds.size.y;
            dimension.x = dimension.y * Camera.main.aspect;
        }

        // Debug
        dimensionBase.x = dimension.x;
        dimensionBase.y = dimension.y;

        // Add manual extends Factor
        dimension *= extendsFactor;

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
    //protected virtual void OnDrawGizmos()
    //{
    //    // Draw Camera Bounds
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireCube(bounds.center + offsets, new Vector3(dimensionBase.x * extendsFactor, dimensionBase.y * extendsFactor, 1));
    //}
    #endregion
}
