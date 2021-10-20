using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPlatforms : MonoBehaviour
{
    [SerializeField]
    public Vector2 _directionPlatform;
    Vector3 _directionPlayer;
    [HideInInspector]
    public GameObject player;

    [SerializeField]
    bool _moveable;
    [SerializeField]
    bool _stickable;

    void Start()
    {
        _directionPlatform.Normalize();
    }

    public virtual void PlatformAction(GameObject p)
    {
        Debug.Log("No specific platform action");
    }


    private void OnCollisionStay(Collision c)
    {
        if (c.gameObject.CompareTag("Player"))
        {
            c.gameObject.GetComponent<Player>().state = Player.STATE.AIR;
            PlatformAction(c.gameObject);
        }
    }
    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, _directionPlatform, Color.red);
    }
}
