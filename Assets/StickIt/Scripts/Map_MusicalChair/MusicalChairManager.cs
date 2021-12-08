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
    [SerializeField] Color colorTextTransition;
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
    [SerializeField] Color colorChairActive;
    [SerializeField] Color colorChairInactive;
    bool spawning = true;
    [HideInInspector]
    public float durationSpawn;

    [SerializeField] public Color colorChairTaken;
    public GameObject winTxt;
    public MMFeedbacks spawnFeedback;
    bool GameLaunched = false;

    Spores sporeScript;

    private void Awake()
    {
        durationSpawn = transitionValue / 3;
    }
    private void Start()
    {
        countDownSave = int.MaxValue;
        textAnim = countdown.GetComponent<Animator>();
      
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
            duration = durationValue + 1;         
            transition -= Time.deltaTime;
            textValue = (int)transition;
            countdown.color = colorTextTransition;

            if (transition <= 0)
            {
                inTransition = false;
                //GameEvents.CameraShake_CEvent?.Invoke(durationValue / 0.4f, 1.0f);
            }
            else if(spawning && transition <= durationSpawn)
            {
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
            if (duration <= 0)
            {
                inTransition = true;
                spawning = true;
                ResetChairPool();
                //GameEvents.CameraShake_CEvent?.Invoke(durationValue / 0.4f, 1.0f);
            }
       
        }
        countdown.text = textValue.ToString();
        if(textValue < countDownSave)
        {
            textAnim.SetTrigger("Update");
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
                chairs[rand].ActivateChair(colorChairActive);
                chairsChanged++;
            }
        }
    }
    private void ResetChairPool()
    {
        //spawnFeedback.PlayFeedbacksInReverse();

        foreach (Chair c in chairs)
        {
            if(c.isActive)
                c.DeactivateChair(colorChairInactive);
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

        StartCoroutine(EndResetChairPool(losers.ToArray()));
    }

    IEnumerator EndResetChairPool(Player[] losers)
    {
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
        }
        mapManagered = false;
    }
}