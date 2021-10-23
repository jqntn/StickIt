using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPlatforms : MonoBehaviour
{
    [SerializeField]
    public Vector2 _directionPlatform;
    [HideInInspector]
    public GameObject player;


    public virtual void PlatformAction(Collision c)
    {
        Debug.Log("No specific platform action");
    }

    private void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.TryGetComponent(out PlayerMouvement p))
        {
            PlatformAction(c);
        }
    }
}
