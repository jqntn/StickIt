using UnityEngine;
internal class BouncyPlatform : MonoBehaviour
{
    public float impulseForce;
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null)
        {
            var v = other.gameObject.GetComponent<Rigidbody>().velocity.normalized;
            if (Physics.Raycast(other.transform.position, v, out RaycastHit hit))
            {
                other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.Reflect(v, hit.normal) * impulseForce;
                if (AudioManager.instance != null)
                {
                    AudioManager.instance.PlayBounceShroomSounds(gameObject);
                }
            }
        }
    }
}