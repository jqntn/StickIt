using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    #region PLAYER_INPUTS
    [SerializeField] private bool isControllerGamepad;
    private PlayerInputs playerInputs;

    private void Awake()
    {
        playerInputs = new PlayerInputs();
    }
    private void OnEnable()
    { playerInputs.Enable(); }

    private void OnDisable()
    { playerInputs.Disable();}

    #endregion

    public enum STATE { STICK, AIR }
    STATE state = STATE.AIR;

    bool isGrounded = false;

    [Header("Dots Preview")] //-----------------------
    [SerializeField] Transform dotPreview;
    public float spaceBetweenDots;
    public int numberOfDots;
    private Transform[] dots;
    private bool isDotsEnabled = false;

    [Header("Movement")]     //-----------------------
    [Tooltip("Force maximale du jump, et clamp de la vélocité maximale")]
    public float maxSpeed;
    bool isAnimCurveSpeed;
    Vector3 addedVector;
    [Tooltip("% de la velocité ajoutée au saut en fonction du temps, cette valeur doit finir à 1")]
    [SerializeField] AnimationCurve animCurveJumpSpeed;
    float t_speed;
    float y_speed = 1;
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
    private Vector3 lastVelocity = Vector3.zero;

    private List<ContactPoint> connectedPoints = new List<ContactPoint>();
    public float attractionMultiplier;
    [Range(0,1)]public float repulsionMultiplier;
    [SerializeField] bool isSlippery;

    public float speedSlowDownCharge;
    public int maxNumberOfJumps;
    private int currentNumberOfJumps;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        dots = new Transform[numberOfDots];
        for (int i = 0; i < dots.Length; i++)
        {
            Transform newDot = Instantiate(dotPreview, transform);
            newDot.gameObject.SetActive(false);
            dots[i] = newDot;
        }
        //forceJumpMultiplicator = minForceJumpMultiplicator;
        playerInputs.NormalInputs.Jump.started += _ => isChargingJump = true;
        playerInputs.NormalInputs.Jump.canceled += _ => Jump();
        playerInputs.NormalInputs.Direction.performed += cntxt => direction = cntxt.ReadValue<Vector2>();
        playerInputs.NormalInputs.Direction.canceled += cntxt => direction = Vector2.zero;

        currentNumberOfJumps = maxNumberOfJumps;

       
    }

    // Update is called once per frame
    void Update()
    {
        
        PreviewDirection();

        if(isChargingJump &&  connectedPoints.Count > 0)
        {
            if(!isDotsEnabled)
            EnableDots(true);
            IncreaseForceJump();
        }

        if (hasJumped)
        {
            AnimCurveJumpGravity();

        }
        if (isAnimCurveSpeed)
        {
            AnimCurveJumpSpeed();
        }

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
        

        Attraction();

        lastVelocity = rb.velocity;

    }

    private void OnGUI()
    {
        if(connectedPoints.Count > 0)
        GUILayout.Label(" attraction strength = " + connectedPoints[0].attractionStrength);

        GUILayout.Label(" y_speed  = " + y_speed);

    }

    #region JUMP

    void Jump()
    {
        print(currentNumberOfJumps);
        if (currentNumberOfJumps > 0 && state == STATE.STICK)
        {
            // Debug.Break();
            float forceJump = maxSpeed * forceJumpMultiplicator;
            if (!isControllerGamepad)
            {
                Vector3 mousePos = playerInputs.NormalInputs.MousePosition.ReadValue<Vector2>();
                mousePos.z = transform.position.z - Camera.main.transform.position.z;
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);
                direction = (mousePos - transform.position).normalized;
            }
            rb.velocity = direction * forceJump;

            forceJumpMultiplicator = minForceJumpMultiplicator;
            isChargingJump = false;
            EnableDots(false);
            isAnimCurveSpeed = true;
            hasJumped = true;
            t_jump = 0;
            y_jump = 0;
            t_speed = 0;
            y_speed = 0;
            addedVector = Vector3.zero;
        }
    }

    void IncreaseForceJump()
    {
        forceJumpMultiplicator += Time.deltaTime * speedIncreaseForceJump;
        forceJumpMultiplicator = Mathf.Clamp(forceJumpMultiplicator, minForceJumpMultiplicator, 1);
        
    }
    #endregion


    private void OnCollisionEnter(Collision collision)
    {
        currentNumberOfJumps = maxNumberOfJumps;

        if (isChargingJump)
        {
            EnableDots(true);
        }

        Vector3 contactNormal = collision.contacts[0].normal;
        float dot = Vector2.Dot(contactNormal, lastVelocity);
        Vector3 localContactPos = collision.transform.position - collision.contacts[0].point;
        ContactPoint contact = new ContactPoint(collision.transform, localContactPos, -dot * attractionMultiplier);
        contact.localPosition.z = transform.position.z;
        
        connectedPoints.Add(contact);

        state = STATE.STICK;
        isAnimCurveSpeed = false;
        addedVector = Vector2.zero;
        y_speed = 0;
        t_speed = 0;
        hasJumped = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        for(int i =0; i < connectedPoints.Count; i++)
        {
            if(collision.transform == connectedPoints[i].transform)
            {
                connectedPoints[i].localPosition = collision.transform.position - collision.contacts[0].point;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
       
       for(int i =0; i < connectedPoints.Count; i++) 
       { 
            if(connectedPoints[i].transform == collision.transform)
            {
                connectedPoints.RemoveAt(i);
            }
       }
       if(connectedPoints.Count == 0)
       {
           state = STATE.AIR;
            if (!hasJumped)
            {
                currentNumberOfJumps--;
                isChargingJump = false;
                EnableDots(false);
            }
       }
    }


    private void Attraction()
    {
       

        for(int i = connectedPoints.Count -1; i >= 0; i--)
        {
            Vector3 localPlayerPosition = connectedPoints[i].transform.position - transform.position;
            Vector3 direction = (connectedPoints[i].localPosition - localPlayerPosition).normalized;


            if (isSlippery)
            {
                Vector3 attraction = -direction * connectedPoints[i].attractionStrength;
                float repulsion = connectedPoints[i].attractionStrength * repulsionMultiplier;
                rb.velocity += attraction * Time.fixedDeltaTime;
                if (!isGrounded)
                {
                    print(connectedPoints[i].transform.gameObject.name);
                    print(repulsion);
                    rb.velocity += new Vector3(0, -gravityStrength) * Time.fixedDeltaTime * 0.1f;
                    connectedPoints[i].attractionStrength -= gravityStrength * Time.fixedDeltaTime * repulsion;
                } else
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.GetChild(0).position, new Vector3(0, -1, 0), out hit, 0.1f))
                    {
                        if(hit.transform == connectedPoints[i].transform)
                        {
                            connectedPoints[i].attractionStrength += gravityStrength * Time.fixedDeltaTime * 3;
                            connectedPoints[i].attractionStrength = Mathf.Clamp(connectedPoints[i].attractionStrength, 0, 15 * attractionMultiplier);
                        }
                    }
                }

            } else
            {
                rb.velocity += -direction * 50 * Time.fixedDeltaTime;
            }
            
            if(connectedPoints[i].attractionStrength < 0)
            {
                connectedPoints.RemoveAt(i);
            }
        }
    }

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
            if (!isControllerGamepad)
            {
                Vector3 mousePos = playerInputs.NormalInputs.Direction.ReadValue<Vector2>();
                mousePos.z = transform.position.z - Camera.main.transform.position.z;
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);
                direction = (mousePos - transform.position).normalized;
            }
            Vector2 potentialVelocity = direction * forceJump;


            for (int i = 0; i < dots.Length; i++)
            {
                dots[i].transform.position = GetDotPosition(i * spaceBetweenDots, potentialVelocity );
            }

        }
    }

    Vector2 GetDotPosition(float t, Vector2 potentialVelocity)
    {
        Vector2 gravity = new Vector2(0, -animCurveJumpGravity.Evaluate(t) * gravityStrength);

        Vector2 pos = (Vector2)transform.position +(potentialVelocity * t) + 0.5f * gravity * (t*t);
        return pos;

    }

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

    void AnimCurveJumpSpeed()
    {
        if (t_speed < animCurveJumpSpeed.keys[animCurveJumpSpeed.length - 1].time)
        {

            t_speed += Time.deltaTime;
            y_speed = t_speed;
            y_speed = animCurveJumpSpeed.Evaluate(y_speed);
        }
        else
        {

            isAnimCurveSpeed = false;
        }
    }


}


public class ContactPoint
{
    public Transform transform;
    public Vector3 localPosition;
    public float attractionStrength;

    public ContactPoint(Transform transform, Vector3 position, float attractionStrength) 
    {
        this.transform = transform;
        this.localPosition = position;
        this.attractionStrength = attractionStrength;
    }

    public ContactPoint() { }

}