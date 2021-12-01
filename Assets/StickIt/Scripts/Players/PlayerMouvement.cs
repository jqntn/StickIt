using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMouvement : MonoBehaviour
{
    [HideInInspector]
    public Player myPlayer;
    public enum STATE { STICK, AIR, ICED, STUCK }
    public STATE state = STATE.AIR;
    bool isGrounded = false;
    [Header("Dots Preview")] //-----------------------
    [SerializeField] Transform dotPreview;
    public float spaceBetweenDots;
    public int numberOfDots;
    private Transform[] dots;
    private bool isDotsEnabled = false;
    [Header("Movement")] //-----------------------
    [Tooltip("Force maximale du jump, et clamp de la velocite maximale")]
    public float maxSpeed;
    Vector3 addedVector;
    [Tooltip("% de la velocite ajoutee au saut en fonction du temps, cette valeur doit finir a 1")]
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
    Vector3 initScale;
    public float limitAngle;
    [Header("CollisionVariables")]
    private float ratioMass;

    private float valueSpeedChargeCurve;
    [SerializeField] private AnimationCurve CurveSpeedChargeIncrease;
    private float valueStrengthCurve;
    [SerializeField] private AnimationCurve CurveStrengthIncrease;
    public GameObject collisionEffect;
    [SerializeField] float strengthRequiredToImpact, strengthRequiredToBigImpact, strengthMultiplicator;
    public ParticleSystem ChocParticles;
    [Header("Mesh")]
    private SkinnedMeshRenderer mesh;
    private OurSphereSoft myScriptSoftBody;
    [Header("DEBUG")]
    public int connexions;
    public bool isDebugLimitAngles = false;
    void Start()
    {
        initScale = transform.localScale;
        rb = GetComponent<Rigidbody>();
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        myScriptSoftBody = GetComponent<OurSphereSoft>();
        dots = new Transform[numberOfDots];
        for (int i = 0; i < dots.Length; i++)
        {
            Transform newDot = Instantiate(dotPreview, transform);
            newDot.gameObject.SetActive(false);
            dots[i] = newDot;
        }
        RescaleMeshWithMass();
        currentNumberOfJumps = maxNumberOfJumps;
    }
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
        if (state != STATE.STICK || state != STATE.STUCK)
        {
            // realVelocity += new Vector2(0, -gravityStrength) * y_jump * Time.fixedDeltaTime;
            rb.velocity += new Vector3(0, -gravityStrength) * y_jump * Time.fixedDeltaTime;
            //rb.velocity -= addedVector;
            //addedVector = rb.velocity * y_speed;
            //rb.velocity += addedVector;
        }
        if (state == STATE.STICK || state == STATE.STUCK)
            Attraction();
        velocityLastFrame = rb.velocity;
    }
    private void OnGUI()
    {
        GUILayout.BeginVertical();
        GUIStyle style = new GUIStyle();
        style.fontSize = 24;
        style.normal.textColor = Color.white;
        if (myPlayer.myDatas.id == 0)
        {
            GUILayout.Label(" Velocity  = " + rb.velocity, style);
            GUILayout.Label("Magnitude = " + rb.velocity.magnitude, style);
        }
        GUILayout.EndVertical();
    }
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
        else if (context.canceled)
        {
            if (direction != Vector2.zero) Jump();
            forceJumpMultiplicator = minForceJumpMultiplicator;
        }
    }
    #endregion
    // ----- JUMP -----
    #region JUMP
    void Jump()
    {
        if (currentNumberOfJumps > 0 && connectedPoints.Count > 0)
        {
            // Debug.Break();
            float forceJump = maxSpeed * forceJumpMultiplicator;
            rb.velocity = direction * forceJump;
            isChargingJump = false;
            EnableDots(false);
            hasJumped = true;
            t_jump = 0;
            y_jump = 0;
            addedVector = Vector3.zero;
            foreach (ContactPointSurface contact in connectedPoints)
            {
                contact.attractionStrength = 100f;
            }
        }
    }

    void IncreaseForceJump()
    {
        if (direction != Vector2.zero)
        {

            forceJumpMultiplicator += Time.deltaTime * (speedIncreaseForceJump / valueSpeedChargeCurve);
            forceJumpMultiplicator = Mathf.Clamp(forceJumpMultiplicator, 0, 1);
        }
        else forceJumpMultiplicator = minForceJumpMultiplicator;
       
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

                Vector3 localPosFromPlayer = collision.contacts[0].point - transform.position;
                Vector3 localPosFromCol =  collision.transform.position - collision.contacts[0].point;

                ContactPointSurface contact = new ContactPointSurface(collision.transform, localPosFromPlayer, localPosFromCol, 0);
                contact.localPositionFromCollision.z = transform.position.z;
                contact.vNormal = collision.contacts[0].normal;
                contact.limitsAngle = contact.GetLimiteAngle(limitAngle);

                connectedPoints.Add(contact);
                PlayerMouvement playerCollided = collision.transform.GetComponent<PlayerMouvement>();
                if (velocityLastFrame.magnitude * valueStrengthCurve > playerCollided.velocityLastFrame.magnitude * playerCollided.valueStrengthCurve)
                {
                    float strength = velocityLastFrame.magnitude * valueStrengthCurve * strengthMultiplicator / playerCollided.valueStrengthCurve;
                    if (strength >= strengthRequiredToImpact) 
                    ImpactBetweenPlayers(playerCollided, collision.contacts[0], strength);
                }
                break;
            default:
                //if (collision.transform.tag != "Untagged") return; // ----- RETURN CONDITION !!!
                #region Collision Untagged
                if (isChargingJump)
                {
                    EnableDots(true);
                }
                Vector3 contactNormal = collision.contacts[0].normal;
                float dot = Vector2.Dot(contactNormal, velocityLastFrame);
                localPosFromPlayer = collision.contacts[0].point - transform.position;
                localPosFromCol = collision.transform.position - collision.contacts[0].point;

                contact = new ContactPointSurface(collision.transform, localPosFromPlayer, localPosFromCol, -dot * attractionMultiplier);
                contact.localPositionFromCollision.z = transform.position.z;
                contact.vNormal = collision.contacts[0].normal;
                contact.limitsAngle = contact.GetLimiteAngle(limitAngle);

                connectedPoints.Add(contact);
                switch (collision.transform.tag)
                {
                    case "Sticky":
                        state = STATE.STUCK;
                        break;
                    case "Icy":
                        state = STATE.ICED;
                        break;
                    default:
                        state = STATE.STICK;
                        break;
                }
                addedVector = Vector2.zero;
                hasJumped = false;
                #endregion
                break;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        //if (collision.transform.tag != "Untagged") return; // ----- RETURN CONDITION !!!
        foreach (ContactPointSurface contact in connectedPoints.Where(x => collision.transform == x.transform))
        {
            contact.localPositionFromPlayer =  collision.contacts[0].point - transform.position;
            contact.localPositionFromCollision = collision.transform.position - collision.contacts[0].point;
            contact.vNormal = collision.contacts[0].normal;
            contact.limitsAngle = contact.GetLimiteAngle(limitAngle);

            if (isDebugLimitAngles)
            {
                Debug.DrawLine(collision.contacts[0].point, collision.contacts[0].point + contact.limitsAngle[0], Color.blue);
                Debug.DrawLine(collision.contacts[0].point, collision.contacts[0].point + contact.limitsAngle[1], Color.blue);
            }
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
    private void ImpactBetweenPlayers(PlayerMouvement playerCollided, ContactPoint contact, float strength)
    {
        Vector3 dir = (playerCollided.transform.position - transform.position).normalized;
        RaycastHit hit;
        //dir = playerCollided.GetPinchDirection();
        // Debug.DrawRay(playerCollided.transform.position, dir * 3, Color.yellow, 10);
        // Debug.Break();
        //if (Physics.SphereCast(playerCollided.transform.position, playerCollided.initScale.x * playerCollided.ratioMass * 0.9f, dir, out hit, 50))
        //{
        //    dir = playerCollided.GetPinchDirection();
        //    //   Debug.DrawRay(playerCollided.transform.position, dir, Color.yellow);

        //}
        rb.velocity = Vector3.zero;
        playerCollided.GetImpacted(dir, strength);
        bool isPowerfull = strength >= strengthRequiredToBigImpact;
        StartCoroutine(StrongImpact(isPowerfull));
        if (isPowerfull)
        {
            float angleNormal = Mathf.Atan(contact.normal.y / contact.normal.x) * Mathf.Rad2Deg;
            Instantiate(ChocParticles, contact.point, Quaternion.Euler(-angleNormal, 80, 0));
        }
    }
    public void GetImpacted(Vector3 dir, float strength)
    {
        rb.velocity = dir * strength;
    }
    #endregion
    private void Attraction()
    {
        for (int i = connectedPoints.Count - 1; i >= 0; i--)
        {
            //if (connectedPoints[i].transform.tag != "Untagged") return; // RETURN CONDITION
            Vector3 localPlayerPosition = connectedPoints[i].transform.position - transform.position;
            Vector3 direction = (connectedPoints[i].localPositionFromCollision - localPlayerPosition).normalized;

            if (isSlippery)
            {
                Vector3 attraction = -direction * connectedPoints[i].attractionStrength;
                float repulsion = 0;
                if (state == STATE.STICK) repulsion = connectedPoints[i].attractionStrength * repulsionMultiplier;
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
                            connectedPoints[i].attractionStrength = Mathf.Clamp(connectedPoints[i].attractionStrength, 0, 2 * attractionMultiplier);
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
    // ----- DEATH & RESPAWN -----
    public void Death()
    {
        mesh.enabled = false;
        foreach (Collider col in GetComponentsInChildren<Collider>())
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
        transform.localScale = initScale;
        state = STATE.AIR;
        mesh.enabled = true;
        GetComponent<Collider>().enabled = true;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        RescaleMeshWithMass();
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
            //SetDirectionAndPreviewAngle();
            Vector2 potentialVelocity = direction * forceJump;
            for (int i = 0; i < dots.Length; i++)
            {
                dots[i].transform.position = GetDotPosition(i * (spaceBetweenDots * ratioMass), potentialVelocity);
            }
        }
    }
    Vector2 GetDotPosition(float t, Vector2 potentialVelocity)
    {
        Vector2 gravity = new Vector2(0, -animCurveJumpGravity.Evaluate(t) * gravityStrength);
        Vector2 pos = (Vector2) transform.position + (potentialVelocity * t) + 0.5f * gravity * (t * t);
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
    public IEnumerator StrongImpact(bool isPowerful)
    {
        if (isPowerful)
        {
            float slowmo = 0.1f;
            Time.timeScale = slowmo;
            Time.fixedDeltaTime *= slowmo;
            yield return new WaitForSeconds(0.05f);
            Time.fixedDeltaTime /= slowmo;
            Time.timeScale = 1;
        }
        state = STATE.AIR;

        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

    }
    public IEnumerator DelayStrongImpacted()
    {
        yield return new WaitForSeconds(0.1f);
        rb.detectCollisions = true;
    }

    public void RescaleMeshWithMass()
    {
        ratioMass = myPlayer.myDatas.mass / 100f;
        valueSpeedChargeCurve = CurveSpeedChargeIncrease.Evaluate(ratioMass);
        valueStrengthCurve = CurveStrengthIncrease.Evaluate(ratioMass);
        float newScale = initScale.x * ratioMass;
        transform.localScale = new Vector3(newScale, newScale, newScale);
        GameObject bonesParent = transform.Find("Bones").gameObject;
        bonesParent.SetActive(false);
        myScriptSoftBody.ReplaceBones(ratioMass);
        bonesParent.SetActive(true);
        rb.mass = myPlayer.myDatas.mass;
    }

    Vector3[] GetPossibleAnglesDirectionJump()
    {
        Vector3[] angles = new Vector3[2];
        List<Vector3> intersections = new List<Vector3>();
        bool isColliding = false;
        for (int i = 0; i < connectedPoints.Count; i++)
        {
            int i2 = (i + 1 == connectedPoints.Count) ? 0 : i + 1;
            Vector3 posA = connectedPoints[i].localPositionFromPlayer;
            Vector3 vAngleB = connectedPoints[i].limitsAngle[1];
            Vector3 posB = connectedPoints[i2].localPositionFromPlayer;
            Vector3 vAngleA = connectedPoints[i2].limitsAngle[0];

            Vector3 intersection = GetIntersectionBetween2Vectors(posA, vAngleA, posB, vAngleB);
  

            // If intersection = V0 -> les directions regard�es ne collideront pas : l'angle represente l'angle possible de saut 
            //(dans le cas d'un pinch : le milieu de l'angle repr�sente la direction de l'ejection)
            if (intersection == Vector3.zero)
            {
    
                angles[0] = vAngleA.normalized;
                angles[1] = vAngleB.normalized;
                isColliding = true;


                break;
            } 
        }

        // Dans le cas d'une collision entre joueurs, cette fonction est appel�e uniquement si le joueur collid� est bloqu� par quelque chose.
        // On renvoie alors soit le milieu de son angle de saut, soit le vecteur du joueur � son point d'intersection le plus �loign�.
        //if (isCollisionBetweenPlayer)
        //{
        //    if (isColliding)
        //    {
            
        //        Debug.Break();
        //    }
        //    else
        //    {
        //        print("intersections");
        //        Vector2 farestVector = intersections[0];
        //        for (int i = 1; i < intersections.Count; i++)
        //        {
        //            float farestDist = (intersections[0] - transform.position).magnitude;
        //            float distTocheck = (intersections[i] - transform.position).magnitude;
        //            if (distTocheck > farestDist)
        //            {
        //                farestDist = distTocheck;
        //                farestVector = intersections[i];
        //            }

        //        }
        //        print("intersections");

        //        angles[0] = farestVector.normalized;
        //    }
        //}

        return angles;
    }

    Vector3 GetPinchDirection()
    {
        // Dans le cas d'une collision entre joueurs, cette fonction est appel�e uniquement si le joueur collid� est bloqu� par quelque chose.
        // On renvoie alors soit le milieu de son angle de saut, soit le vecteur du joueur � son point d'intersection le plus �loign�.
        Vector3 dir = Vector3.zero;
        List<Vector3> intersections = new List<Vector3>();

        ContactPointSurface[] contacts = ReorderConnectedPoints();

        foreach(ContactPointSurface contact in contacts)
        {
            print( contact.transform.name);
        }

        for (int i = 0; i < contacts.Length; i++)
        {
            int i2 = i + 1;
            if (i2 == contacts.Length) i2 = 0;
            Vector3 posA = contacts[i].localPositionFromPlayer;
            Vector3 posB = contacts[i2].localPositionFromPlayer;
            Vector3 vAngleG = contacts[i].limitsAngle[1];
            Vector3 vAngleD = contacts[i2].limitsAngle[0];

            
            Vector3 intersection = GetIntersectionBetween2Vectors(posA, vAngleG, posB, vAngleD);

            Debug.DrawRay(contacts[i].localPositionFromPlayer + transform.position, vAngleG * 3, Color.black, 1);
            Debug.DrawRay(contacts[i2].localPositionFromPlayer + transform.position, vAngleD * 3, Color.red, 1);
            
         
            // If intersection = V0 -> les directions regard�es ne collideront pas : l'angle represente l'angle possible de saut 
            //(dans le cas d'un pinch : le milieu de l'angle repr�sente la direction de l'ejection)
            if (intersection == Vector3.zero)
            {

                dir = ((vAngleG + vAngleD) / 2);
                Debug.DrawRay(connectedPoints[i2].localPositionFromPlayer + transform.position, dir * 5, Color.green, 1);
                return dir.normalized;
            }

            intersections.Add(intersection);

        }

        // If this next code is read, this mean all the angles have an intersection.
        float maxMagnitude = 0f;
        foreach(Vector3 intersection in intersections)
        {
            Vector3 dist = intersection - transform.position;
            if(dist.magnitude > maxMagnitude)
            {
                maxMagnitude = dist.magnitude;
                dir = dist;
            }
            print("no exit");
        }
        return dir.normalized;
    }

    void SetDirectionAndPreviewAngle()
    {
        ReorderConnectedPoints();
        Vector3[] angles = GetPossibleAnglesDirectionJump();
        Vector2 vectorBase = (angles[0] + angles[1]) / 2;
        float angleDirection = Vector2.Angle(vectorBase, direction);

        if (angleDirection > limitAngle)
        {
            float maxGap = 90 - limitAngle;
            float gap = angleDirection - limitAngle;

            float result = gap / maxGap;

            forceJumpMultiplicator = Mathf.Lerp(forceJumpMultiplicator, 0, result);
                
        }



    }

    Vector3 GetIntersectionBetween2Vectors (Vector3 posA, Vector3 vA, Vector3 posB, Vector3 vB)
    {
        Vector3 intersection = Vector3.zero;

        if (((vA.y * vB.x) - (vA.x * vB.y)) != 0)
        {
            float k = ((posA.x - posB.x) * vB.y + (posB.y - posA.y) * vB.x) / ((vA.y * vB.x) - (vA.x * vB.y));
            if (k > 0)
            {
                intersection = posA + vA * k;
            }
        }

        return intersection;
    }

    ContactPointSurface[] ReorderConnectedPoints()
    {
        ContactPointSurface[] newList = new ContactPointSurface[connectedPoints.Count];

        float minAngle = -1f ;
        for(int i = 0; i < connectedPoints.Count; i++)
        {
            ContactPointSurface contactToOrder = new ContactPointSurface();
            float AngleMaxToCheck = 360f;
            float angle = 0f;
            for (int j = 0; j < connectedPoints.Count; j++)
            {
                angle = Vector2.Angle(Vector2.right, connectedPoints[j].localPositionFromPlayer);
                if (connectedPoints[j].localPositionFromPlayer.y < 0) angle = 360f - angle;
                if(angle < AngleMaxToCheck && angle > minAngle)
                {
                    AngleMaxToCheck = angle;
                    contactToOrder = connectedPoints[j];
                }
                
            }
            newList[i] = contactToOrder;
            minAngle = angle;
           
        }

            return newList;
    }

    

}
public class ContactPointSurface
{
    public Transform transform;
    public Vector3 localPositionFromPlayer;
    public Vector3 localPositionFromCollision;
    public float attractionStrength;
    public Vector3 vNormal;
    public Vector3[] limitsAngle = new Vector3[2];
    public float angleLeft;
    public float angleRight;

    public ContactPointSurface(Transform transform, Vector3 localPositionFromPlayer, Vector3 localPositionFromCollision, float attractionStrength)
    {
        this.transform = transform;
        this.localPositionFromPlayer = localPositionFromPlayer;
        this.localPositionFromCollision = localPositionFromCollision;
        this.attractionStrength = attractionStrength;
    }
    public ContactPointSurface() { }

    public Vector3[] GetLimiteAngle(float limit)
    {
        float baseAngle = Vector3.Angle(Vector3.right, vNormal);
        if (vNormal.y < 0) baseAngle = 360 - baseAngle;

        angleRight = baseAngle - limit;
        angleLeft= baseAngle + limit;

        if (angleRight < 0) angleRight += 360f;
        if (angleLeft > 360) angleLeft -= 360f;


        limitsAngle[0] = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angleRight), Mathf.Sin(Mathf.Deg2Rad * angleRight));
        limitsAngle[1] = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angleLeft), Mathf.Sin(Mathf.Deg2Rad * angleLeft));
        return limitsAngle;
    }


}