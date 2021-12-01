using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Platform : MonoBehaviour
{
    [HideInInspector]
    public GameObject player;
    public virtual void Action(Collision c)
    {
        Debug.Log("No specific platform action");
    }
    private void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.TryGetComponent(out PlayerMouvement p))
        {
            Action(c);
        }
    }
}