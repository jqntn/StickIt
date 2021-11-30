using UnityEngine;
class BouncyPlatform : MonoBehaviour
{
    public float impulseForce;
    void OnTriggerEnter(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null)
        {
            var v = other.gameObject.GetComponent<Rigidbody>().velocity;
            if (Physics.Raycast(other.transform.position, v, out RaycastHit hit))
            {
                var incomingVec = hit.point - other.transform.position;
                var reflectVec = Vector3.Reflect(incomingVec, hit.normal);
                other.gameObject.GetComponent<Rigidbody>().velocity = reflectVec.normalized * impulseForce;
            }
        }
    }
}