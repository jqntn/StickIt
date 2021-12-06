using UnityEngine;
public class Platform : MonoBehaviour
{
    public virtual void Action(Collision c)
    { Debug.Log("No specific platform action."); }
    private void OnCollisionEnter(Collision c)
    {
        Player player = c.gameObject.GetComponent<Player>();
        if (player != null) Action(c);
    }
}