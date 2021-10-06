using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPlatforms : MonoBehaviour
{
    [SerializeField]
    Vector2 _direction;
    [SerializeField]
    bool _moveable;
    [SerializeField]
    bool _stickable;
    // Start is called before the first frame update
    void Start()
    {
        _direction =Vector3.up;
    }
    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, _direction, Color.red);
    }
}
