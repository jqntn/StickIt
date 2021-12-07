using System.Collections;
using UnityEngine;
public class DelayAnim : MonoBehaviour
{
    [SerializeField]
    private float delay;
    private void Start() => StartCoroutine(Coroutine());
    private IEnumerator Coroutine()
    {
        yield return new WaitForSeconds(delay);
        GetComponent<Animation>().Play();
    }
}