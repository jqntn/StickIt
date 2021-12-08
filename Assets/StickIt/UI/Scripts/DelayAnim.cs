using System.Collections;
using UnityEngine;
public class DelayAnim : MonoBehaviour
{
    [SerializeField] private float delay;
    private void OnEnable() => StartCoroutine(Coroutine());
    private void OnDisable() => transform.localScale = Vector3.one;
    private IEnumerator Coroutine()
    {
        yield return new WaitForSeconds(delay);
        GetComponent<Animation>().Play();
    }
}