using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    public ParticleSystem VFXSnow;
    public float delayStop = 0.1f;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Icy"))
        {
            VFXSnow.Play();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Icy"))
        {
            StartCoroutine(OnIceCollide());
        }
    }
    private void Awake()
    {
        VFXSnow.Stop();
    }

    private IEnumerator OnIceCollide()
    {
        yield return new WaitForFixedUpdate();
        VFXSnow.Stop();

    }
    public void PlayVFXSnow()
    {
        VFXSnow.Play();
    }
}
