using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterTest : MonoBehaviour
{
    #region PLAYER_INPUTS
    [SerializeField] private bool isControllerGamepad;
    private PlayerInputs playerInputs;
    private void OnEnable()
    { playerInputs.Enable(); }

    private void OnDisable()
    { playerInputs.Disable(); }
    #endregion



    [Header("Physics")]
    Vector2 velocity, desiredVelocity;
    Vector2 playerInput;
    Rigidbody body;
    [SerializeField, Range(0f, 100f)]
    float gravityForce;
    //JUMP
    [Header("Jump")]
    [SerializeField, Range(0f, 100f)]
    float maxJumpForce;
    float currJumpForce;
    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f;

    Vector3 direction;


    [Header("Booleans States")]
    public bool isChargingJump;
    void Awake()
    {
        playerInputs = new PlayerInputs();
        body = GetComponent<Rigidbody>();
    }
    private void Start()
    {

        //forceJumpMultiplicator = minForceJumpMultiplicator;
        playerInputs.NormalInputs.Jump.started += _ => StartJumpCharge();
        playerInputs.NormalInputs.Jump.canceled += _ => Jump();
        playerInputs.NormalInputs.Direction.performed += cntxt => direction = cntxt.ReadValue<Vector2>();
        playerInputs.NormalInputs.Direction.canceled += cntxt => direction = Vector2.zero;
    }
    // Update is called once per frame
    void Update()
    {
        if (isChargingJump)
        {
            currJumpForce = Mathf.Clamp(currJumpForce + Time.deltaTime * maxAcceleration, maxJumpForce / 3, maxJumpForce);
        }
    }
    private void FixedUpdate()
    {
        
        //transform.position = velocity;
    }

    void StartJumpCharge()
    {
        isChargingJump = true;
    }
    void Jump()
    {
        body.velocity += direction * currJumpForce;
        isChargingJump = false;
    }
    private void OnGUI()
    {
        GUILayout.Label(" Velocity = " + velocity);
        GUILayout.Label(" isChargingJump = " + isChargingJump);
        GUILayout.Label(" direction = " + direction);
    }
}
