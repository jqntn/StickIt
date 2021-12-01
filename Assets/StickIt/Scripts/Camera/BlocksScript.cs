using UnityEngine;

public class BlocksScript : MonoBehaviour
{
    public Vector2 extend = new Vector2(0.0f, 0.0f);

    #region Grayed out
    public Bounds bounds = new Bounds();
    public Vector2 boundsPos  = new Vector2(0.0f, 0.0f);
    public Vector2 max = new Vector2(0.0f, 0.0f);
    public Vector2 dimension = new Vector2(0.0f, 0.0f);
    public static BlocksScript Instance { get; set; }
    #endregion

    private void OnEnable()
    {
        Instance = this;

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
        dimension.x = childCollider.bounds.size.x + extend.x;
        childCollider = temp_height.GetComponent<Collider>();
        dimension.y = childCollider.bounds.size.y + extend.y;

        boundsPos = bounds.center;
    }

    #region Debug
    protected virtual void OnDrawGizmosSelected()
    {
        // Draw Camera Bounds
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(boundsPos, new Vector3(bounds.size.x + dimension.x, bounds.size.y + dimension.y, 1));
    }
    #endregion
}
