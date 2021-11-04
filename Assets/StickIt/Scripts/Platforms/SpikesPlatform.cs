using UnityEngine;
class SpikesPlatform : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null) player.Death();
    }
}