using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMouvement : MonoBehaviour
{
    [HideInInspector]
    public Player myPlayer;

    public enum STATE { STICK, AIR }
    public STATE state = STATE.AIR;

    bool isGrounded = false;

    [Header("Dots Preview")] //-----------------------
    [SerializeField] Transform dotPreview;
    public float spaceBetweenDots;
    public int numberOfDots;
    private Transform[] dots;
    private bool isDotsEnabled = false;

    [Header("Movement")]     //-----------------------
    [Tooltip("Force maximale du jump, et clamp de la v�locit� maximale")]
    public float maxSpeed;
    Vector3 addedVector;
    [Tooltip("% de la velocit� ajout�e au saut en fonction du temps, cette valeur doit finir � 1")]
    Vector2 direction;
    bool isChargingJump = false;
    [SerializeField]
    private float minForceJumpMultiplicator;
    float forceJumpMultiplicator;
    [SerializeField]
    float speedIncreaseForceJump;
    bool hasJumped = false;
    [SerializeField] AnimationCurve animCurveJumpGravity;
    float t_jump;
    float y_jump = 1;
    public float gravityStrength;
    private Rigidbody rb;
    public Vector3 velocityLastFrame = Vector3.zero;

    private List<ContactPointSurface> connectedPoints = new List<ContactPointSurface>();
    public float attractionMultiplier;
    [Range(0, 1)] public float repulsionMultiplier;
    [SerializeField] bool isSlippery;

    public int maxNumberOfJumps;
    private int currentNumberOfJumps;


    [Header("CollisionVariables")]
    private SkinnedMeshRenderer mesh;
    public Transform firstBone;
    public GameObject collisionEffect;
    [SerializeField] float strengthRequiredToBigImpact;

    [Header("DEBUG")]
    public int connexions;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        firstBone = transform.GetChild(1);
        dots = new Transform[numberOfDots];
        for (int i = 0; i < dots.Length; i++)
        {
            Transform newDot = Instantiate(dotPreview, transform);
            newDot.gameObject.SetActive(false);
            dots[i] = newDot;
        }

        currentNumberOfJumps = maxNumberOfJumps;
    }

    // Update is called once per frame
    void Update()
    {

        PreviewDirection();

        if (isChargingJump && connectedPoints.Count > 0)
        {

            if (!isDotsEnabled)
            {
                EnableDots(true);

            }

            IncreaseForceJump();
        }

        if (hasJumped)
        {
            AnimCurveJumpGravity();

        }

        connexions = connectedPoints.Count;

    }

    private void FixedUpdate()
    {
        if (Physics.Raycast(transform.GetChild(0).position, new Vector3(0, -1, 0), 0.1f))
        {
            isGrounded = true;
        }
        else isGrounded = false;


        if (state != STATE.STICK)
        {
            // realVelocity += new Vector2(0, -gravityStrength) * y_jump * Time.fixedDeltaTime;

            rb.velocity += new Vector3(0, -gravityStrength) * y_jump * Time.fixedDeltaTime;

            //rb.velocity -= addedVector;
            //addedVector = rb.velocity * y_speed;
            //rb.velocity += addedVector;
        }

        if (state == STATE.STICK)
            Attraction();

        velocityLastFrame = rb.velocity;

    }

    //private void OnGUI()
    //{
    //    GUILayout.BeginVertical();
    //    GUIStyle style = new GUIStyle();
    //    style.fontSize = 24;
    //    style.normal.textColor = Color.white;

    //    if (myPlayer.id == 0)
    //    {
    //        GUILayout.Label(" Velocity  = " + rb.velocity, style);
    //        GUILayout.Label("Magnitude = " + rb.velocity.magnitude, style);
    //        if(connectedPoints.Count>0)
    //        GUILayout.Label("Attraction = " + connectedPoints[0].attractionStrength, style);
    //    }
    //    GUILayout.EndVertical();

        
    //}

    // ----- INPUTS -----
    #region Inputs

    public void InputDirection(InputAction.CallbackContext context)
    {
        if (context.performed) direction = context.ReadValue<Vector2>();
        else if (context.canceled) direction = Vector2.zero;
    }

    public void InputJump(InputAction.CallbackContext context)
    {
        if (context.started) isChargingJump = true;
        else if (context.canceled && direction != Vector2.zero) Jump();
    }

    #endregion

    // ----- JUMP -----
    #region JUMP
    void Jump()
    {
        if (currentNumberOfJumps > 0 && connectedPoints.Count >0)
        {
            // Debug.Break();
            float forceJump = maxSpeed * forceJumpMultiplicator;

            rb.velocity = direction * forceJump;

            forceJumpMultiplicator = minForceJumpMultiplicator;
            isChargingJump = false;
            EnableDots(false);
            hasJumped = true;
            t_jump = 0;
            y_jump = 0;

            addedVector = Vector3.zero;

            foreach(ContactPointSurface contact in connectedPoints)
            {
                contact.attractionStrength = 100f;
            }
        }



    }
    void IncreaseForceJump()
    {
        forceJumpMultiplicator += Time.deltaTime * speedIncreaseForceJump;
        forceJumpMultiplicator = Mathf.Clamp(forceJumpMultiplicator, minForceJumpMultiplicator, 1);
        print(speedIncreaseForceJump);
    }

    void GetPossibleAngle()
    {
        foreach(ContactPointSurface point in connectedPoints)
        {

        }
    }

    #endregion

    // ----- COLLISIONS -----
    #region Collisions
    private void OnCollisionEnter(Collision collision)
    {
        currentNumberOfJumps = maxNumberOfJumps;
        switch (collision.transform.tag)
        {
            case "Player":             
                Vector3 localContactPos = collision.transform.position - collision.contacts[0].point;
                ContactPointSurface contact = new ContactPointSurface(collision.transform, localContactPos, 0);
                contact.localPosition.z = transform.position.z;

                connectedPoints.Add(contact);


                PlayerMouvement playerCollided = collision.transform.GetComponent<PlayerMouvement>();
                if (velocityLastFrame.magnitude > playerCollided.velocityLastFrame.magnitude)
                {
                    print(velocityLastFrame.magnitude);
                    print(gameObject.name);
                    if(velocityLastFrame.magnitude >= strengthRequiredToBigImpact) 
                    BigImpactBetweenPlayers(playerCollided, collision.contacts[0]);
                }
                    break;

            default:
                if (collision.transform.tag != "Untagged") return; // ----- RETURN CONDITION !!!
                #region Collision Untagged


                if (isChargingJump)
                {
                    EnableDots(true);
                }

                Vector3 contactNormal = collision.contacts[0].normal;
                float dot = Vector2.Dot(contactNormal, velocityLastFrame);
                localContactPos = collision.transform.position - collision.contacts[0].point;
                contact = new ContactPointSurface(collision.transform, localContactPos, -dot * attractionMultiplier);
                contact.localPosition.z = transform.position.z;

                connectedPoints.Add(contact);

                state = STATE.STICK;
                addedVector = Vector2.zero;
                hasJumped = false;
                #endregion
                break;
        }

    }



    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.tag != "Untagged") return; // ----- RETURN CONDITION !!!
        foreach (ContactPointSurface point in connectedPoints.Where(point => collision.transform == point.transform))
        {
            point.localPosition = collision.transform.position - collision.contacts[0].point;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        //if (collision.transform.tag != "Untagged") return; // ----- RETURN CONDITION !!!
        for (int i = 0; i < connectedPoints.Count; i++)
        {
            if (connectedPoints[i].transform == collision.transform)
            {
                connectedPoints.RemoveAt(i);
            }
        }
        if (connectedPoints.Count == 0)
        {
            state = STATE.AIR;
            if (!hasJumped)
            {
                currentNumberOfJumps = 0;
                EnableDots(false);
            }
        }
    }


    private void BigImpactBetweenPlayers(PlayerMouvement playerCollided, ContactPoint contact)
    {
        rb.velocity = Vector3.zero;
        //GetComponent<Collider>().enabled = false;
        //foreach (Collider col in GetComponentsInChildren<Collider>())
        //{
        //    col.enabled = false;
        //}
        Vector3 dir = (playerCollided.firstBone.position - firstBone.position).normalized;
        rb.velocity = -dir;
        float strength = velocityLastFrame.magnitude;
        playerCollided.GetBigImpacted(dir, strength);
        foreach(ContactPointSurface point in connectedPoints)
        {
            if(point.transform == playerCollided.transform)
            {
                connectedPoints.Remove(point);
                break;
            }
        }
        foreach (ContactPointSurface point in playerCollided.connectedPoints)
        {
            if (point.transform == transform)
            {
                connectedPoints.Remove(point);
                break;
            }
        }
        StartCoroutine(ImmunityStrongImpact());
        Debug.DrawRay(firstBone.position, -dir * 2, Color.red);
        Debug.DrawRay(playerCollided.firstBone.position, dir * strength * 2, Color.yellow);

    }

    public void GetBigImpacted(Vector3 dir, float strength)
    {
        rb.velocity = dir * strength * 2;
        rb.detectCollisions = false;
       
        StartCoroutine(DelayStrongImpacted());
    }


    #endregion

    private void Attraction()
    {
        for (int i = connectedPoints.Count - 1; i >= 0; i--)
        {
            if (connectedPoints[i].transform.tag != "Untagged") return; // RETURN CONDITION
            Vector3 localPlayerPosition = connectedPoints[i].transform.position - transform.position;
            Vector3 direction = (connectedPoints[i].localPosition - localPlayerPosition).normalized;


            if (isSlippery)
            {
                Vector3 attraction = -direction * connectedPoints[i].attractionStrength;
                float repulsion = connectedPoints[i].attractionStrength * repulsionMultiplier;
                rb.velocity += attraction * Time.fixedDeltaTime;
                if (!isGrounded)
                {
                    rb.velocity += new Vector3(0, -gravityStrength) * Time.fixedDeltaTime * 0.1f;
                    connectedPoints[i].attractionStrength -= gravityStrength * Time.fixedDeltaTime * repulsion;
                }
                else
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.GetChild(0).position, new Vector3(0, -1, 0), out hit, 0.1f))
                    {
                        if (hit.transform == connectedPoints[i].transform)
                        {
                            connectedPoints[i].attractionStrength += gravityStrength * Time.fixedDeltaTime * 3;
                            connectedPoints[i].attractionStrength = Mathf.Clamp(connectedPoints[i].attractionStrength, 0, 2* attractionMultiplier);
                        }
                    }
                }
            }


            if (connectedPoints[i].attractionStrength < 0)
            {
                connectedPoints.RemoveAt(i);
            }
        }
    }

    public void Death()
    {
        mesh.enabled = false;
        foreach(Collider col in GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            rb.isKinematic = true;
        }
    }

    public void Respawn()
    {
        connectedPoints.Clear();
        state = STATE.AIR;
        mesh.enabled = true;
        GetComponent<Collider>().enabled = true;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            col.enabled = true;
        }
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }

    // ----- PREVIEW DOTS -----
    #region PreviewDots
    private void EnableDots(bool isTrue)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].gameObject.SetActive(isTrue);
        }
        isDotsEnabled = isTrue;
    }
    private void PreviewDirection()
    {
        if (isChargingJump)
        {
            float forceJump = maxSpeed * forceJumpMultiplicator;
            Vector2 potentialVelocity = direction * forceJump;


            for (int i = 0; i < dots.Length; i++)
            {
                dots[i].transform.position = GetDotPosition(i * spaceBetweenDots, potentialVelocity);
            }

        }
    }
    Vector2 GetDotPosition(float t, Vector2 potentialVelocity)
    {
        Vector2 gravity = new Vector2(0, -animCurveJumpGravity.Evaluate(t) * gravityStrength);

        Vector2 pos = (Vector2)transform.position + (potentialVelocity * t) + 0.5f * gravity * (t * t);
        return pos;

    }
    #endregion

    // ----- ANIMATIONS CURVE -----
    #region Animations curve
    void AnimCurveJumpGravity()
    {
        if (t_jump < animCurveJumpGravity.keys[animCurveJumpGravity.length - 1].time)
        {

            t_jump += Time.deltaTime;
            y_jump = t_jump;
            y_jump = animCurveJumpGravity.Evaluate(y_jump);
        }
        else
        {
            hasJumped = false;

        }
    }
    #endregion


    public IEnumerator ImmunityStrongImpact()
    {
        
        yield return new WaitForSeconds(0.1f);


        state = STATE.AIR;
        //GetComponent<Collider>().enabled = true;
        //foreach (Collider col in GetComponentsInChildren<Collider>())
        //{
        //    col.enabled = true;
        //}


        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        //foreach (Collider col in GetComponentsInChildren<Collider>())
        //{
        //    col.enabled = true;
        //}
        //foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        //{
        //    rb.isKinematic = false;
        //    rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        //}
    }

    public IEnumerator DelayStrongImpacted()
    {

        yield return new WaitForSeconds(0.1f);

        rb.detectCollisions = true;
    }

}

public class ContactPointSurface
{
    public Transform transform;
    public Vector3 localPosition;
    public float attractionStrength;

    public ContactPointSurface(Transform transform, Vector3 position, float attractionStrength)
    {
        this.transform = transform;
        this.localPosition = position;
        this.attractionStrength = attractionStrength;
    }

    public ContactPointSurface() { }

}



