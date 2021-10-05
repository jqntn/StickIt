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

    [Header("Movement")]     //-----------------------
    public float maxSpeed;
    Vector2 direction;
    bool isChargingJump = false;
    [SerializeField]
    private float minForceJumpMultiplicator;
    float forceJumpMultiplicator;
    [SerializeField]
    float speedIncreaseForceJump;
    bool hasJumped = false;   
    [SerializeField] AnimationCurve animCurveJump;
    float t;
    float y = 1;
    public float gravityStrength;
    private Rigidbody rb;
    private Vector3 lastVelocity = Vector3.zero;

    private List<ContactPoint> connectedPoints = new List<ContactPoint>();
    public float attractionMultiplier;
    [Range(0,1)]public float desattractionMultiplier;
    [SerializeField] bool isSlippery;



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
        playerInputs.NormalInputs.Jump.started += _ => StartJumpCharge();
        playerInputs.NormalInputs.Jump.canceled += _ => Jump();
        playerInputs.NormalInputs.Direction.performed += cntxt => direction = cntxt.ReadValue<Vector2>();
        playerInputs.NormalInputs.Direction.canceled += cntxt => direction = Vector2.zero;

       
    }

    // Update is called once per frame
    void Update()
    {
        IncreaseForceJump();
        PreviewDirection();

        if (hasJumped)
        {
            if (t < animCurveJump.keys[animCurveJump.length - 1].time)
            {
              
                t += Time.deltaTime;
                y = t;
                y = animCurveJump.Evaluate(y);
            } else {
                hasJumped = false;
 
            }
        }

    }

    private void FixedUpdate()
    {
        if (Physics.Raycast(transform.GetChild(0).position, new Vector3(0, -1, 0), 0.1f))
        {
            isGrounded = true;
        }
        else isGrounded = false;

        if(state != STATE.STICK)
        rb.velocity += new Vector3(0, -gravityStrength) * y * Time.fixedDeltaTime;

        Attraction();

        lastVelocity = rb.velocity;

    }

    private void OnGUI()
    {
        if(connectedPoints.Count > 0)
        GUILayout.Label(" attraction strength = " + connectedPoints[0].attractionStrength);
        
    }

    #region JUMP
    void StartJumpCharge()
    {
        isChargingJump = true;
        EnableDots(true);
    }

    void Jump()
    {
        // Debug.Break();
        connectedPoints.Clear();
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
        hasJumped = true;
        t = 0;
        y = 0;

    }

    void IncreaseForceJump()
    {
        if (isChargingJump)
        {
            forceJumpMultiplicator += Time.deltaTime * speedIncreaseForceJump;
            forceJumpMultiplicator = Mathf.Clamp(forceJumpMultiplicator, minForceJumpMultiplicator, 1);
        }
    }
    #endregion


    private void OnCollisionEnter(Collision collision)
    {
        Vector3 contactNormal = collision.contacts[0].normal;
        float dot = Vector2.Dot(contactNormal, lastVelocity);
        Vector3 localContactPos = collision.transform.position - collision.contacts[0].point;
        ContactPoint contact = new ContactPoint(collision.transform, localContactPos, -dot * attractionMultiplier);
        contact.localPosition.z = transform.position.z;
        
        connectedPoints.Add(contact);

        state = STATE.STICK;
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
                rb.velocity += attraction * Time.fixedDeltaTime;
                if (!isGrounded)
                {
                    
                    rb.velocity += new Vector3(0, -gravityStrength) * Time.fixedDeltaTime * desattractionMultiplier;
                    connectedPoints[i].attractionStrength -= gravityStrength * Time.fixedDeltaTime * desattractionMultiplier;
                } else
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.GetChild(0).position, new Vector3(0, -1, 0), out hit, 0.1f))
                    {
                        if(hit.transform == connectedPoints[i].transform)
                        {
                            connectedPoints[i].attractionStrength += gravityStrength * Time.fixedDeltaTime * 3;
                            connectedPoints[i].attractionStrength = Mathf.Clamp(connectedPoints[i].attractionStrength, 0, 30 * attractionMultiplier);
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
            print(direction.magnitude);


            for (int i = 0; i < dots.Length; i++)
            {
                dots[i].transform.position = GetDotPosition(i * spaceBetweenDots, potentialVelocity );
            }

        }
    }

    Vector2 GetDotPosition(float t, Vector2 potentialVelocity)
    {
        /* Vector2 PointPosition(float t){
         * Vector 2 position = (Vector2)shotPoint.position + (direction.normalized * launchForce * t) + 0.5f * gravity * (t*t);
        return position;
         * }
        */
        Vector2 gravity = new Vector2(0, -animCurveJump.Evaluate(t) * gravityStrength);

        Vector2 pos = (Vector2)transform.position +(potentialVelocity * t) + 0.5f * gravity * (t*t);
        return pos;

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