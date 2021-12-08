using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using MoreMountains.Feedbacks;
using UnityEngine.UI;
public class MusicalChairManager : Level
{
    [Header("Countdown")]
    [SerializeField] bool mapManagered;
    [SerializeField] float durationValue;
    float duration;
    [SerializeField] float transitionValue;
    [SerializeField] Color colorTextRound;
    float transition;
    public Text countdown;
    private Animator textAnim;
    float countDownSave;
    float textValue;
    bool inTransition;
    [Header("Chairs")]
    public Chair[] chairs;
    int maxChairsActive;

    bool spawning = true;
    [HideInInspector]
    public float durationSpawn;

    public Material chairNotTaken;
    public Material chairTaken;
    public GameObject winTxt;
    public MMFeedbacks spawnFeedback;
    bool GameLaunched = false;

    Spores sporeScript;
    [SerializeField] MeshRenderer bigMushroomRenderer;
    [SerializeField] Material bigMushroomMat, bigMushroomAngryMat;


    private void Awake()
    {
        //durationSpawn = 2;
    }
    private void Start()
    {
        countDownSave = int.MaxValue;
        textAnim = countdown.GetComponent<Animator>();
        countdown.text = "";

      
    }
    // Update is called once per frame
    void Update()
    {
        if (mapManagered)
        {
            StartCoroutine(Init());
            mapManagered = false;
        }
        if (GameLaunched)
            UpdateText();
    }
    public override void StartMap()
    {
        base.StartMap();
        chairs = FindObjectsOfType<Chair>();
        inTransition = true;
        transition = transitionValue + 1;
        maxChairsActive = MultiplayerManager.instance.alivePlayers.Count - 1;
        GameLaunched = true;
        
    }
    private void UpdateText()
    {
        if (inTransition)
        {
                 
            transition -= Time.deltaTime;

            if (transition <= 0) // spawning over
            {
                inTransition = false;
                duration = durationValue + 1;
                GameEvents.CameraShake_CEvent?.Invoke(duration / 0.4f);
                //Haptics 
                for (int i = 0; i < UnityEngine.InputSystem.Gamepad.all.Count; i++)
                UnityEngine.InputSystem.Gamepad.all[i].PauseHaptics();
            }
            else if(spawning) //  Start Spawn
            {
                    for (int i = 0; i < UnityEngine.InputSystem.Gamepad.all.Count; i++)
                       UnityEngine.InputSystem.Gamepad.all[i].SetMotorSpeeds(0.1f, 0.1f);
                ChangeChairPool();
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
            if (duration < 1)
            {
                inTransition = true;
                spawning = true;
                ResetChairPool();
                GameEvents.CameraShake_CEvent?.Invoke(duration / 0.4f);
                
            }
       
        }
        
        if(textValue < countDownSave)
        {
            if(textValue > 3)
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
        //spawnFeedback.PlayFeedbacksInReverse();
        StartCoroutine("ResetText");
        foreach (Chair c in chairs)
        {
            if(c.isActive)
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

    IEnumerator EndResetChairPool(Player[] losers)
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
            winTxt.transform.parent.parent.gameObject.SetActive(true);
            winTxt.GetComponent<Text>().text = MultiplayerManager.instance.alivePlayers[0].myDatas.name + " win!";
        }
        else if (MultiplayerManager.instance.alivePlayers.Count <= 0)
        {
            winTxt.transform.parent.parent.gameObject.SetActive(true);
            winTxt.GetComponent<Text>().text = "Only losers...";
        } else
        {
            GameLaunched = true;
   
        }
        mapManagered = false;
    }

    IEnumerator ResetText()
    {
        yield return new WaitForSeconds(1);
        countdown.text = "";
    }
}