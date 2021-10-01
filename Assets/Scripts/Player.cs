using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region PLAYER_INPUTS
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
    STATE state;

    bool isChargingJump = false;
    [SerializeField]
    private float minForceJumpMultiplicator;
    float forceJumpMultiplicator;
    [SerializeField]
    float speedIncreaseForceJump;


    bool hasJumped = false;
    public float maxSpeed;
    [SerializeField] AnimationCurve animCurveJump;
    float t;
    float y = 1;
    public float gravityStrength;
    private Rigidbody rb;
    private float lastVelocityLength = 0;

    private List<ContactPoint> connectedPoints = new List<ContactPoint>();



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //forceJumpMultiplicator = minForceJumpMultiplicator;
        playerInputs.NormalInputs.Jump.started += _ => StartJumpCharge();
        playerInputs.NormalInputs.Jump.canceled += _ => Jump();
    }

    // Update is called once per frame
    void Update()
    {
        IncreaseForceJump();

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
        rb.velocity += new Vector3(0, -gravityStrength) * y * Time.fixedDeltaTime;

        lastVelocityLength = rb.velocity.magnitude;
    }

    private void OnGUI()
    {
        GUILayout.Label(" Velocity = " + rb.velocity);
        GUILayout.Label(" AnimCurve time = " + y);
    }

    #region JUMP
    void StartJumpCharge()
    {
        isChargingJump = true;
    }

    void Jump()
    {
       // Debug.Break();
        float forceJump = maxSpeed * forceJumpMultiplicator;
        Vector3 mousePos = playerInputs.NormalInputs.MousePosition.ReadValue<Vector2>();
        mousePos.z = transform.position.z - Camera.main.transform.position.z;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 dir = (mousePos - transform.position).normalized;
        rb.velocity = dir * forceJump;

        forceJumpMultiplicator = minForceJumpMultiplicator;
        isChargingJump = false;
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
        ContactPoint contact = new ContactPoint(collision.transform, collision.contacts[0].point, lastVelocityLength);        
        contact.position.z = transform.position.z;
        connectedPoints.Add(contact);
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
    }

    private void Attraction()
    {
        for(int i = 0; i < connectedPoints.Count; i++)
        {

        }
    }

}


public class ContactPoint
{
    public Transform transform;
    public Vector3 position;
    public float attractionStrength;

    public ContactPoint(Transform transform, Vector3 position, float attractionStrength) 
    {
        this.transform = transform;
        this.position = position;
        this.attractionStrength = attractionStrength;
    }

    public ContactPoint() { }

}