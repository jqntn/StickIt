using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCharTest : MonoBehaviour
{
    CharacterTest player;
    // Start is called before the first frame update
    void Awake()
    {
        player = GetComponent<CharacterTest>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        //collision.collider;
    }
}
