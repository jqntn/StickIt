using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerAnimations : MonoBehaviour
{
    public ParticleSystem VFXSnow;
    public Vector3 velocityThresholdToStop = new Vector3(.0f, .0f, .0f);

    [Header("DEBUG___________________________________")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private bool hasCollidedWithSnow = false;

    public void ChangeBoolSnowToFalse()
    {
        hasCollidedWithSnow = false;
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        VFXSnow.Stop();
        hasCollidedWithSnow = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Icy"))
        {
            if (!hasCollidedWithSnow)
            {
                hasCollidedWithSnow = true;
                StartCoroutine(OnIceEnter());
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Icy"))
        {
            hasCollidedWithSnow = false;
            StartCoroutine(OnIceExit());
        }
    }

    private IEnumerator OnIceEnter()
    {
        while (hasCollidedWithSnow)
        {
            if (Mathf.Abs(rb.velocity.x) <= velocityThresholdToStop.x &&
                Mathf.Abs(rb.velocity.y) <= velocityThresholdToStop.y)
            {
                VFXSnow.Stop();
            }
            else
            {
                VFXSnow.Play();
            }

            yield return null;
        }

    }
    private IEnumerator OnIceExit()
    {
        yield return new WaitForFixedUpdate();
        VFXSnow.Stop();

    }
    public void PlayVFXSnow()
    {
        VFXSnow.Play();
    }
}
