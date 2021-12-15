using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MusicalChairManager : Level
{
    [Header("Countdown")]
    [SerializeField] private float durationValue;
    private float duration;
    [SerializeField] private float transitionValue;
    [SerializeField] private Color colorTextRound;
    private float transition;
    public Text countdown;
    private Animator textAnim;
    private float countDownSave;
    private float textValue;
    public bool inTransition;
    [Header("Chairs")]
    public Chair[] chairs;
    private int maxChairsActive;
    private bool spawning = true;
    public float sizeShieldChair;
    public float intensityEmissive;
    [HideInInspector]
    public float durationSpawn;
    public GameObject winTxt;
    public MoreMountains.Feedbacks.MMFeedbacks spawnFeedback;
    public bool GameLaunched = false;
    private Spores sporeScript;
    [SerializeField] private MeshRenderer bigMushroomRenderer;
    [SerializeField] private Material bigMushroomMat, bigMushroomAngryMat;
    protected override void Awake()
    {
        AudioManager.instance.SwitchAmbianceToFall(gameObject);
        //durationSpawn = 2;
    }
    protected override void Start()
    {
        countDownSave = int.MaxValue;
        textAnim = countdown.GetComponent<Animator>();
        if (textAnim == null)
        {
            Debug.LogWarning("You need to add the Animator 'CountdownChair' to the countDown ! Project > Animations > Timer > CountdownChair");
        }
        countdown.text = "";
    }
    // Update is called once per frame
    protected override void Update()
    {
        if (GameLaunched)
            UpdateText();
    }
    protected override void StartMap()
    {
        base.StartMap();
        //Debug.Log("Child Start Map");
        chairs = FindObjectsOfType<Chair>();
        inTransition = true;
        transition = transitionValue + 1;
        maxChairsActive = MultiplayerManager.instance.alivePlayers.Count - 1;
        GameLaunched = true;
    }
    private void UpdateText()
    {
        if (Debug.isDebugBuild)
        {
            //Debug.Log("inTransition = " + inTransition);
            //Debug.Log("isSpawning = " + spawning);
        }
        if (inTransition)
        {
            transition -= Time.deltaTime;
            if (transition <= 0) // spawning over
            {
                inTransition = false;
                duration = durationValue + 1;
                //Haptics
                for (int i = 0; i < UnityEngine.InputSystem.Gamepad.all.Count; i++)
                    UnityEngine.InputSystem.Gamepad.all[i].PauseHaptics();
            }
            else if (spawning) //  Start Spawn
            {
                GameEvents.CameraShake_CEvent?.Invoke(transition, 1.0f);
                for (int i = 0; i < UnityEngine.InputSystem.Gamepad.all.Count; i++)
                    UnityEngine.InputSystem.Gamepad.all[i].SetMotorSpeeds(0.1f, 0.1f);
                ChangeChairPool();
                GameEvents.ShakeAppearChairEvent.Invoke(transition, 1.0f);
                spawning = false;
            }
        }
        else
        {
            transition = transitionValue + 1;
            duration -= Time.deltaTime;
            textValue = (int)duration;
            countdown.color = colorTextRound;
            countdown.text = textValue.ToString();
            if (duration < 0)
            {
                inTransition = true;
                spawning = true;
                ResetChairPool();
                GameEvents.CameraShake_CEvent.Invoke(0.4f, 1.0f);
            }
        }
        if (textValue < countDownSave)
        {
            if (textValue > 3)
                textAnim.SetTrigger("Update");
            else
            {
                textAnim.SetTrigger("UpdateCloseEnd");
            }
        }
        countDownSave = textValue;
    }
    private void ChangeChairPool()
    {
        if (Debug.isDebugBuild)
        {
            //Debug.Log("ChangeChairPool");
        }
        //spawnFeedback.PlayFeedbacks();
        if (sporeScript == null)
        {
            if (!(sporeScript = FindObjectOfType<Spores>()))
            {
                Debug.LogWarning("You need to put the prefab Spores in the map ! Prefabs > Particles > Spores");
            }
        }
        sporeScript.Initialize();
        bigMushroomRenderer.material = bigMushroomMat;
        int rand = Random.Range(0, chairs.Length);
        int chairsChanged = 0;
        while (chairsChanged < maxChairsActive)
        {
            if (chairs[rand].isActive)
            {
                rand = Random.Range(0, chairs.Length);
            }
            else
            {
                chairs[rand].ActivateChair(transition);
                chairsChanged++;
            }
        }
    }
    private void ResetChairPool()
    {
        StartCoroutine("ResetText");
        foreach (Chair c in chairs)
        {
            if (c.isActive)
                c.DeactivateChair(transition);
        }
        List<Player> losers = new List<Player>();
        for (int i = MultiplayerManager.instance.alivePlayers.Count - 1; i >= 0; i--)
        {
            if (!winners.Find(x => MultiplayerManager.instance.alivePlayers[i] == x))
            {
                //MAKE THE LOSERS EXPLODE
                losers.Add(MultiplayerManager.instance.alivePlayers[i]);
            }
        }
        sporeScript.KillLosers(losers);
        bigMushroomRenderer.material = bigMushroomAngryMat;
        StartCoroutine(EndResetChairPool(losers.ToArray()));
    }
    private IEnumerator EndResetChairPool(Player[] losers)
    {
        GameLaunched = false;
        yield return new WaitForSeconds(2);
        for (int i = 0; i < losers.Length; i++)
        {
            losers[i].Death();
        }
        maxChairsActive = MultiplayerManager.instance.alivePlayers.Count - 1;
        winners.Clear();
        // FIN LEVEL
        if (MultiplayerManager.instance.alivePlayers.Count == 1)
        {
            EndLvl();
            winTxt.GetComponent<Text>().text = MultiplayerManager.instance.alivePlayers[0].myDatas.name + " win!";
        }
        else if (MultiplayerManager.instance.alivePlayers.Count <= 0)
        {
            EndLvl();
            winTxt.GetComponent<Text>().text = "It's a draw";
            winTxt.GetComponent<Text>().color = Color.red;
        }
        else
        {
            GameLaunched = true;
        }
    }
    private IEnumerator ResetText()
    {
        yield return new WaitForSeconds(1);
        countdown.text = "";
    }
    private void EndLvl()
    {
        winTxt.transform.parent.parent.gameObject.SetActive(true);
        for (int i = 0; i < chairs.Length; i++)
        {
            chairs[i].DeactivateShield();
        }
    }
}