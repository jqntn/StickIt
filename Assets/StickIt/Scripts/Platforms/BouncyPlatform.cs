using UnityEngine;
internal class BouncyPlatform : MonoBehaviour
{
    public float impulseForce;
    private void OnCollisionEnter(Collision other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null)
        {
            other.gameObject.GetComponent<Rigidbody>().velocity = (other.transform.position - other.GetContact(0).point).normalized * impulseForce;
            if (AudioManager.instance != null) AudioManager.instance.PlayBounceShroomSounds(gameObject);
        }
    }
}