using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;
using MoreMountains.Feedbacks;

[RequireComponent(typeof(PlayerMouvement))]
[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    [Header("DATA_________________________________")]
    public PlayerData myDatas;
    public bool isDead;

    [Header("VFX__________________________________")]
    public MMFeedbacks deathAnim;
    public GameObject deathPart;
    private ParticleSystem deathParticle;
    public GameObject VFXExplosion;
    private VisualEffect VFXExplosionParticle;

    [Header("MASS_________________________________")]
    [SerializeField] private int minMass = 100;
    [SerializeField] private int maxMass = 250;

    [Header("DEBUG________________________________")]
    [SerializeField] private PlayerMouvement myMouvementScript;
    public PlayerMouvement MyMouvementScript { get => myMouvementScript; }
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private MultiplayerManager multiplayerManager;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        multiplayerManager = MultiplayerManager.instance;
        if (TryGetComponent<PlayerMouvement>(out PlayerMouvement pm))
        {
            myMouvementScript = pm;
            myMouvementScript.myPlayer = this;
        }

        DontDestroyOnLoad(this);

        deathPart.SetActive(true);
        VFXExplosion.SetActive(true);
        deathParticle = deathPart.GetComponent<ParticleSystem>();
        VFXExplosionParticle = VFXExplosion.GetComponent<VisualEffect>();
    }

    private void Start()
    {
        // VFX
        ParticleSystemRenderer particleRenderer = deathPart.GetComponent<ParticleSystemRenderer>();
        if (particleRenderer != null) { particleRenderer.material = myDatas.material; }
    }
    public void Death(bool intensityAnim = false)
    {
        isDead = true;
        myMouvementScript.enabled = false;
        multiplayerManager.alivePlayers.Remove(this);
        multiplayerManager.deadPlayers.Add(this);

        // Play Death Animation
        StartCoroutine(OnDeath(intensityAnim));
    }
    private IEnumerator OnDeath(bool intensityAnim)
    {
        deathAnim.PlayFeedbacks();
        if (intensityAnim)
        {
            yield return new WaitForSeconds(deathAnim.TotalDuration / deathAnim.DurationMultiplier);
        }

        if (AudioManager.instance != null) { 
            AudioManager.instance.PlayDeathSounds(gameObject); 
        }

        myMouvementScript.Death();

        // Death VFX
        deathParticle.Play();
        GameEvents.CameraShake_CEvent?.Invoke(2.0f, 1.0f);
        if(!VFXExplosion.activeInHierarchy) VFXExplosion.SetActive(true);
        VFXExplosionParticle.SendEvent("Trigger");

        MapManager.instance.EndLevel();
    }
    public void PrepareToChangeLevel() // When the player is still alive
    {
        if (!isDead)
        {
            myMouvementScript.enabled = false;
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
    }
    public void Respawn()
    {
        myMouvementScript.enabled = true;
        myMouvementScript.Respawn();
        isDead = false;
    }

    public void SetScoreAndMass(int score, int mass)
    {
        myDatas.score += score;
        myDatas.mass += mass;
        myDatas.mass = Mathf.Clamp(myDatas.mass, minMass, maxMass);
        myMouvementScript.RescaleMeshWithMass();
    }

    // INPUT TOOLS TO REMOVE LATER
    //public void InputTestMassP25(UnityEngine.InputSystem.InputAction.CallbackContext context)
    //{
    //    if (!MultiplayerManager.instance.isMenuSelection && context.started)
    //    {
    //        SetScoreAndMass(0, 25);
    //        myMouvementScript.RescaleMeshWithMass();
    //    }
    //}
    //public void InputTestMassP5(UnityEngine.InputSystem.InputAction.CallbackContext context)
    //{
    //    if (!MultiplayerManager.instance.isMenuSelection && context.started)
    //    {
    //        SetScoreAndMass(0, 5);
    //        myMouvementScript.RescaleMeshWithMass();
    //    }
    //}
    //public void InputTestMassM25(UnityEngine.InputSystem.InputAction.CallbackContext context)
    //{
    //    if (!MultiplayerManager.instance.isMenuSelection && context.started)
    //    {
    //        SetScoreAndMass(0, -25);
    //        myMouvementScript.RescaleMeshWithMass();
    //    }
    //}
    //public void InputTestMassM5(UnityEngine.InputSystem.InputAction.CallbackContext context)
    //{
    //    if (!MultiplayerManager.instance.isMenuSelection && context.started)
    //    {
    //        SetScoreAndMass(0, -5);
    //        myMouvementScript.RescaleMeshWithMass();
    //    }
    //}

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed && !isDead && !MapManager.instance.isBusy && SceneManager.GetActiveScene().name != "1_MenuSelection" && SceneManager.GetActiveScene().name != "100_EndScene")
        {
            AkSoundEngine.PostEvent("Play_SFX_UI_Return", gameObject);
            if (MapManager.instance.CurModName == "MusicalChairs" && FindObjectOfType<MusicalChairManager>().inTransition)
                for (var i = 0; i < Gamepad.all.Count; i++)
                    if (Pause.instance.isPaused) Gamepad.all[i].SetMotorSpeeds(.1f, .1f);
                    else Gamepad.all[i].PauseHaptics();
            Pause.instance.mainLayerSwitch.ChangeLayer("Layer_Main");
            Pause.instance.PauseGame();
            if (Pause.instance.isPaused) GetComponent<PlayerInput>().actions.FindActionMap("UIInputs").Enable();
            else GetComponent<PlayerInput>().actions.FindActionMap("UIInputs").Disable();
        }
    }
    public void OnReturn(InputAction.CallbackContext context)
    {
        if (context.performed && Pause.instance.isPaused)
        {
            if (Pause.instance.mainLayer.activeSelf)
            {
                Pause.instance.PauseGame();
            }
            else
            {
                Pause.instance.mainLayerSwitch.ChangeLayer("Layer_Main");
                Pause.instance.oLayerSwitch.ChangeLayer("Layer_Video");
            }
        }
    }

    public void SoundMove(InputAction.CallbackContext context)
    { if (context.performed && Pause.instance.isPaused) AkSoundEngine.PostEvent("Play_SFX_UI_Move", gameObject); }
    public void SoundSubmit(InputAction.CallbackContext context)
    { if (context.performed && Pause.instance.isPaused) AkSoundEngine.PostEvent("Play_SFX_UI_Submit", gameObject); }
    public void SoundReturn(InputAction.CallbackContext context)
    { if (context.performed && SceneManager.GetActiveScene().name != "1_MenuSelection") AkSoundEngine.PostEvent("Play_SFX_UI_Return", gameObject); }
    public void SoundY(InputAction.CallbackContext context)
    { if (context.performed && Pause.instance.mainLayer.activeSelf && Pause.instance.isPaused) AkSoundEngine.PostEvent("Play_SFX_UI_Submit", gameObject); }
    public void OnLeftPage(InputAction.CallbackContext context)
    {
        if (context.performed && Pause.instance.isPaused)
            if (Pause.instance.hLayerSwitch.gameObject.activeSelf) Pause.instance.hLayerSwitch.IncLayer(-1);
            else Pause.instance.oLayerSwitch.IncLayer(-1);
    }
    public void OnRightPage(InputAction.CallbackContext context)
    {
        if (context.performed && Pause.instance.isPaused)
            if (Pause.instance.hLayerSwitch.gameObject.activeSelf) Pause.instance.hLayerSwitch.IncLayer(1);
            else Pause.instance.oLayerSwitch.IncLayer(1);
    }
    public void Jump(InputAction.CallbackContext context)
    { if (context.performed && Pause.instance.easterEgg.layer.activeSelf && Pause.instance.easterEgg.canJump && Pause.instance.isPaused) StartCoroutine(Pause.instance.easterEgg.MainCoroutine()); }
}