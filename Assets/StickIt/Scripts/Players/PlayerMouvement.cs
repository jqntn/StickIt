using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public enum STATE { STICK, AIR, ICED, STUCK }

public class PlayerMouvement : MonoBehaviour
{
    public STATE state = STATE.AIR;

    [Header("Dots Preview___________________________________")]
    public int numberOfDots = 10;
    public float spaceBetweenDots = .015f;
    [Header("Debug")]
    [SerializeField] private Transform[] dots;
    [SerializeField] private Transform dotPreview;
    [SerializeField] private bool isDotsEnabled = true;
    [SerializeField] private bool isPreviewDots = true;

    [Header("Movement_______________________________________")]
    [Tooltip("Force maximale du jump, et clamp de la velocite maximale")]
    public float maxSpeed = 40.0f;
    public float limitAngle = 45.0f;                // in Degree
    public bool isReversedDirection = false;
    public bool isLimitedMovement = false;

    [Header("Jump___________________________________________")]
    public int maxNumberOfJumps = 1;
    public float minForceJumpMultiplicator = .2f;
    public float speedIncreaseForceJump = 5.0f;
    public AnimationCurve animCurveJumpGravity = new AnimationCurve();

    [Header("Debug")]
    [Tooltip("% de la velocite ajoutee au saut en fonction du temps, cette valeur doit finir a 1")]
    [SerializeField] private int currentNumberOfJumps;
    [SerializeField] private float forceJumpMultiplicator = .0f;
    [SerializeField] private float y_jump = .0f;
    [SerializeField] private float time_jump = .0f;
    [SerializeField] private Vector2 direction = new Vector2(.0f, .0f);
    [SerializeField] private bool isChargingJump = false;
    [SerializeField] private bool hasJumped = false;

    [Header("Gravity________________________________________")]
    public float gravityStrength;
    public float attractionMultiplier;
    [Range(0, 1)]
    public float repulsionMultiplier;


    [Header("Debug")]
    [SerializeField] private List<ContactPointSurface> connectedPoints = new List<ContactPointSurface>();
    [SerializeField] private Vector3 initScale;

    [Header("Collision Variables____________________________")]
    public GameObject collisionEffect;
    public AnimationCurve CurveSpeedChargeIncrease = new AnimationCurve();
    public AnimationCurve CurveStrengthIncrease = new AnimationCurve();
    [Header("Debug")]
    [SerializeField] private float ratioMass;
    [SerializeField] private float valueSpeedChargeCurve;
    [SerializeField] private float valueStrengthCurve;
    [SerializeField] private float strengthRequiredToImpact, strengthRequiredToBigImpact, strengthMultiplicator;

    [Header("VFX____________________________________________")]
    public ParticleSystem ChocParticles;
    public GameObject VFXContact;
    public float VFXTime = 2.0f;
    [Header("Debug")]
    [SerializeField] private ParticleSystem VFXContactParticle;

    [Header("Mesh___________________________________________")]
    [SerializeField] private SkinnedMeshRenderer mesh;
    [SerializeField] private OurSphereSoft myScriptSoftBody;

    [Header("DEBUG__________________________________________")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private int connexions;
    [SerializeField] private Vector3 velocityLastFrame = Vector3.zero;
    [SerializeField] private bool isDebugLimitAngles = false;
    [SerializeField] private bool isGrounded = false;
    [SerializeField] private bool isSlippery = false;

    #region Property
    public Player myPlayer { get; set; }
    public float ForceJumpMultiplicator { get => forceJumpMultiplicator; }
    #endregion

    private void Awake()
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
        currentNumberOfJumps = maxNumberOfJumps;

        // VFX
        VFXContact.SetActive(true);
        VFXContactParticle = VFXContact.GetComponent<ParticleSystem>();
    }
    private void Start()
    {
        RescaleMeshWithMass();
    }
    private void Update()
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
            rb.velocity += new Vector3(0, -gravityStrength) * y_jump * Time.fixedDeltaTime;
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
        GUILayout.EndVertical();
    }
    // ----- INPUTS -----
    #region Inputs
    public void InputDirection(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            direction = context.ReadValue<Vector2>();
            if (isReversedDirection)
            {
                float angleDir = Vector2.Angle(Vector2.right, direction);
                if (direction.y < 0) angleDir = 360 - angleDir;
                angleDir += 180;
                direction = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angleDir), Mathf.Sin(Mathf.Deg2Rad * angleDir));
            }
        }
        else if (context.canceled) direction = Vector2.zero;
    }
    public void InputJump(InputAction.CallbackContext context)
    {
        if (context.started) isChargingJump = true;
        else if (context.canceled)
        {
            if (direction != Vector2.zero) Jump();
            forceJumpMultiplicator = minForceJumpMultiplicator;
            isChargingJump = false;
            EnableDots(false);
        }
    }
    #endregion Inputs
    // ----- JUMP -----
    #region JUMP
    private void Jump()
    {
        if (currentNumberOfJumps > 0 && connectedPoints.Count > 0)
        {
            float forceJump = maxSpeed * forceJumpMultiplicator;
            rb.velocity = direction * forceJump;
            isChargingJump = false;
            EnableDots(false);
            hasJumped = true;
            time_jump = 0;
            y_jump = 0;

            if (AudioManager.instance != null) { AudioManager.instance.PlayJumpSounds(this.gameObject); }
            foreach (ContactPointSurface contact in connectedPoints)
            {
                contact.attractionStrength = 100f;
            }
        }
    }
    private void IncreaseForceJump()
    {
        if (direction != Vector2.zero)
        {
            forceJumpMultiplicator += Time.deltaTime * (speedIncreaseForceJump / valueSpeedChargeCurve);
            forceJumpMultiplicator = Mathf.Clamp(forceJumpMultiplicator, 0, 1);
        }
        else forceJumpMultiplicator = minForceJumpMultiplicator;
    }
    #endregion JUMP
    // ----- COLLISIONS -----
    #region Collisions
    private void OnCollisionEnter(Collision collision)
    {
        currentNumberOfJumps = maxNumberOfJumps;
        switch (collision.transform.tag)
        {
            case "Player":
                Vector3 localPosFromPlayer = collision.contacts[0].point - transform.position;
                Vector3 localPosFromCol = collision.transform.position - collision.contacts[0].point;
                ContactPointSurface contact = new ContactPointSurface(collision.transform, localPosFromPlayer, localPosFromCol, 0);
                contact.localPositionFromCollision.z = transform.position.z;
                contact.vNormal = collision.contacts[0].normal;
                contact.limitsAngle = contact.CalculateLimiteAngle(limitAngle);
                connectedPoints.Add(contact);
                PlayerMouvement playerCollided = collision.transform.GetComponent<PlayerMouvement>();
                if (AudioManager.instance != null) { AudioManager.instance.PlayCollisionSounds(gameObject); }
                if (velocityLastFrame.magnitude * valueStrengthCurve > playerCollided.velocityLastFrame.magnitude * playerCollided.valueStrengthCurve)
                {
                    float strength = velocityLastFrame.magnitude * valueStrengthCurve * strengthMultiplicator / playerCollided.valueStrengthCurve;
                    if (strength >= strengthRequiredToImpact)
                        ImpactBetweenPlayers(playerCollided, collision.contacts[0], strength);
                }
                break;
            case "Icy":
                AkSoundEngine.PostEvent("Play_SFX_S_IceSlide", gameObject);
                break;
            default:

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
                contact.limitsAngle = contact.CalculateLimiteAngle(limitAngle);
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
                if (AudioManager.instance != null)
                {
                    AudioManager.instance.PlayLandSounds(gameObject);
                }
                hasJumped = false;
                #endregion Collision Untagged
                break;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPointSurface contact in connectedPoints.Where(x => collision.transform == x.transform))
        {
            contact.localPositionFromPlayer = collision.contacts[0].point - transform.position;
            contact.localPositionFromCollision = collision.transform.position - collision.contacts[0].point;
            contact.vNormal = collision.contacts[0].normal;
            contact.limitsAngle = contact.CalculateLimiteAngle(limitAngle);
            if (isDebugLimitAngles)
            {
                Debug.DrawLine(collision.contacts[0].point, collision.contacts[0].point + contact.limitsAngle[0], Color.blue);
                Debug.DrawLine(collision.contacts[0].point, collision.contacts[0].point + contact.limitsAngle[1], Color.blue);
            }
        }
        if (collision.gameObject.CompareTag("Icy"))
        {
            if (rb.velocity.magnitude < 0.1)
            {
                AkSoundEngine.PostEvent("Stop_SFX_S_IceSlide", gameObject);
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
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
        if (collision.gameObject.CompareTag("Icy"))
        {
            AkSoundEngine.PostEvent("Stop_SFX_S_IceSlide", gameObject);
        }
    }
    private void ImpactBetweenPlayers(PlayerMouvement playerCollided, ContactPoint contact, float strength)
    {
        Vector3 dir = (playerCollided.transform.position - transform.position).normalized;
        rb.velocity = Vector3.zero;
        playerCollided.GetImpacted(dir, strength);
        StartCoroutine(OnImpactBetweenPlayer());
        // Strong Impact
        bool isPowerfull = strength >= strengthRequiredToBigImpact;
        StartCoroutine(StrongImpact(isPowerfull));
        if (isPowerfull)
        {
            float angleNormal = Mathf.Atan(contact.normal.y / contact.normal.x) * Mathf.Rad2Deg;
            ParticleSystem particles = Instantiate(ChocParticles, contact.point, Quaternion.Euler(-angleNormal, 80, 0));
            Destroy(particles.gameObject, 1);
        }
    }
    private IEnumerator OnImpactBetweenPlayer()
    {
        VFXContact.SetActive(true);
        VFXContactParticle.Play();
        yield return new WaitForSeconds(VFXTime);
        VFXContactParticle.Stop();
    }
    public void GetImpacted(Vector3 dir, float strength)
    {
        rb.velocity = dir * strength;
    }
    #endregion Collisions
    private void Attraction()
    {
        for (int i = connectedPoints.Count - 1; i >= 0; i--)
        {
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
        if (isPreviewDots)
        {
            for (int i = 0; i < dots.Length; i++)
            {
                dots[i].gameObject.SetActive(isTrue);
            }
            isDotsEnabled = isTrue;
        }
    }
    private void PreviewDirection()
    {
        if (isChargingJump)
        {
            float forceJump = maxSpeed * forceJumpMultiplicator;
            if (isLimitedMovement)
                SetLimitedDirectionAndPreviewAngle();
            if (isPreviewDots)
            {
                Vector2 potentialVelocity = direction * forceJump;
                for (int i = 0; i < dots.Length; i++)
                {
                    dots[i].transform.position = GetDotPosition(i * (spaceBetweenDots * ratioMass), potentialVelocity);
                }
            }
        }
    }
    private Vector2 GetDotPosition(float time, Vector2 potentialVelocity)
    {
        Vector2 gravity = new Vector2(0, -animCurveJumpGravity.Evaluate(time) * gravityStrength);
        Vector2 pos = (Vector2)transform.position + (potentialVelocity * time) + 0.5f * gravity * (time * time);
        return pos;
    }
    #endregion PreviewDots
    // ----- ANIMATIONS CURVE -----
    #region Animations curve
    private void AnimCurveJumpGravity()
    {
        if (time_jump < animCurveJumpGravity.keys[animCurveJumpGravity.length - 1].time)
        {
            time_jump += Time.deltaTime;
            y_jump = time_jump;
            y_jump = animCurveJumpGravity.Evaluate(y_jump);
        }
        else
        {
            hasJumped = false;
        }
    }
    #endregion Animations curve
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
        Debug.Log(ratioMass);
        myScriptSoftBody.ReplaceBones(ratioMass);
        bonesParent.SetActive(true);
        rb.mass = myPlayer.myDatas.mass;
    }
    private Vector3[] GetPossibleAnglesDirectionJump()
    {
        Vector3[] angles = new Vector3[2];
        List<Vector3> intersections = new List<Vector3>();

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
                break;
            }
        }
        return angles;
    }
    private Vector3 GetPinchDirection()
    {
        // Dans le cas d'une collision entre joueurs, cette fonction est appel�e uniquement si le joueur collid� est bloqu� par quelque chose.
        // On renvoie alors soit le milieu de son angle de saut, soit le vecteur du joueur � son point d'intersection le plus �loign�.
        Vector3 dir = Vector3.zero;
        List<Vector3> intersections = new List<Vector3>();
        ContactPointSurface[] contacts = ReorderConnectedPoints();
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
        foreach (Vector3 intersection in intersections)
        {
            Vector3 dist = intersection - transform.position;
            if (dist.magnitude > maxMagnitude)
            {
                maxMagnitude = dist.magnitude;
                dir = dist;
            }
        }
        return dir.normalized;
    }
    private void SetLimitedDirectionAndPreviewAngle()
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
    private Vector3 GetIntersectionBetween2Vectors(Vector3 posA, Vector3 vA, Vector3 posB, Vector3 vB)
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
    private ContactPointSurface[] ReorderConnectedPoints()
    {
        ContactPointSurface[] array;
        array = connectedPoints.OrderBy(x => x.myAnglefromPlayer).ToArray();
        return array;
    }
}

[System.Serializable]
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
    public float myAnglefromPlayer;

    // Constructor
    public ContactPointSurface(Transform transform, Vector3 localPositionFromPlayer, Vector3 localPositionFromCollision, float attractionStrength)
    {
        this.transform = transform;
        this.localPositionFromPlayer = localPositionFromPlayer;
        this.localPositionFromCollision = localPositionFromCollision;
        this.attractionStrength = attractionStrength;
        myAnglefromPlayer = CalculateMyAngleFromPlayer();
    }
    public ContactPointSurface(){ }
    //-------------------------------

    // Method
    private float CalculateMyAngleFromPlayer()
    {
        float angle = Vector2.Angle(Vector2.right, localPositionFromPlayer);
        if (localPositionFromPlayer.y < 0) angle = 360f - angle;
        return angle;
    }

    public Vector3[] CalculateLimiteAngle(float limit)
    {
        float baseAngle = Vector3.Angle(Vector3.right, vNormal);
        if (vNormal.y < 0) baseAngle = 360 - baseAngle;
        angleRight = baseAngle - limit;
        angleLeft = baseAngle + limit;
        if (angleRight < 0) angleRight += 360f;
        if (angleLeft > 360) angleLeft -= 360f;
        limitsAngle[0] = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angleRight), Mathf.Sin(Mathf.Deg2Rad * angleRight));
        limitsAngle[1] = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angleLeft), Mathf.Sin(Mathf.Deg2Rad * angleLeft));
        return limitsAngle;
    }
    //-----------------------------------------------
}