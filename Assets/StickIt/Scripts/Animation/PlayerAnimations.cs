using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMouvement))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerAnimations : MonoBehaviour
{
    [Header("ANIMATION_______________________________")]
    public bool isRandomJumping = true;
    public float jumpMultiplicator = 1.0f;
    public bool hasRandomJumpMultiplicator = false;
    public float minJumpMultiplicator = 0.3f;
    public float maxJumpMultiplicator = 0.7f;
    public float rayGroundDistance = 1.0f;
    public LayerMask layerPlayer;

    [Header("PARTICLE________________________________")]
    public ParticleSystem VFXSnow;
    public Vector3 velocityThresholdToStop = new Vector3(.0f, .0f, .0f);

    [Header("DEBUG___________________________________")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerMouvement playerMovement;
    [SerializeField] private SphereCollider sphereCollider;
    [SerializeField] private bool hasCollidedWithSnow = false;
    [SerializeField] private bool isJumpingAnim = false;
    public bool IsJumpingAnim { get => isJumpingAnim; set => isJumpingAnim = value; }



    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        VFXSnow.Stop();
        VFXSnow.gameObject.SetActive(true);
        hasCollidedWithSnow = false;
        playerMovement = GetComponent<PlayerMouvement>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        IcyPlatform ice = collision.gameObject.GetComponent<IcyPlatform>();
        SpeedPlatfom dash = collision.gameObject.GetComponent<SpeedPlatfom>();
        if(ice != null || dash != null) 
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

    public void ChangeBoolSnowToFalse()
    {
        hasCollidedWithSnow = false;
    }

    public void PlayVictory()
    {
        if (playerMovement == null) { return; }
        if (!isRandomJumping) { return; }
        isJumpingAnim = true;
        StartCoroutine(OnPlayVictory());
    }

    public void PlayRank()
    {
        if (playerMovement == null) { return; }
        if (!isRandomJumping) { return; }
        isJumpingAnim = true;
        StartCoroutine(OnPlayRank());
    }

    private IEnumerator OnPlayVictory()
    {
        while (isJumpingAnim)
        {
            JumpAnimation();
            yield return null;
        }
    }

    private IEnumerator OnPlayRank()
    {
        while (isJumpingAnim)
        {
            JumpAnimation();
            yield return null;
        }
    }
    private void JumpAnimation()
    {
        float distance = sphereCollider.bounds.extents.y + rayGroundDistance;
        bool hit = Physics.Raycast(transform.position, Vector3.down, distance, ~layerPlayer);

        if (hit)
        {
            if (hasRandomJumpMultiplicator)
            {
                jumpMultiplicator = Random.Range(minJumpMultiplicator, maxJumpMultiplicator);
            }

            float forceJump = playerMovement.maxSpeed * jumpMultiplicator;
            rb.velocity = Vector3.up * forceJump;
        }
    }

    #region GIZMOS
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        float distance = GetComponent<SphereCollider>().bounds.extents.y + rayGroundDistance;
        Gizmos.DrawRay(transform.position, new Vector3(0.0f, -distance, 0.0f));
    }
    #endregion
}
