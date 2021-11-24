using System.Collections.Generic;
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
    public Color colorChairTaken;
    public GameObject winTxt;
    public MMFeedbacks spawnFeedback;
    bool GameLaunched = false;
    private void Awake()
    {
        durationSpawn = transitionValue / 3;
    }
    private void Start()
    {
        if(mapManagered)
            Init();
    }
    // Update is called once per frame
    void Update()
    {   
        if(GameLaunched)
            UpdateText();
    }
    public override void Init()
    {
        base.Init();
        chairs = FindObjectsOfType<Chair>();
        inTransition = true;
        transition = transitionValue;
        maxChairsActive = MultiplayerManager.instance.alivePlayers.Count - 1;
        GameLaunched = true;
    }
    private void UpdateText()
    {
        if (inTransition)
        {
            duration = durationValue;
            countdown.color = colorTextTransition;
            transition -= Time.deltaTime;
            textValue = Mathf.Round(transition);

            if (transition <= 0)
            {
                inTransition = false;
            }
            else if(spawning && transition <= durationSpawn && transition > 0)
            {
                ChangeChairPool();
                spawning = false;
            }
        }
        else
        {
            transition = transitionValue;
            countdown.color = colorTextRound;
            duration -= Time.deltaTime;
            textValue = Mathf.Round(duration);
            if (duration <= 0)
            {
                inTransition = true;
                spawning = true;
                ResetChairPool();
            }
        }
        countdown.text = textValue.ToString();
    }
    private void ChangeChairPool()
    {
        //spawnFeedback.PlayFeedbacks();
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
        for (int i = MultiplayerManager.instance.alivePlayers.Count - 1; i >= 0; i--)
        {
            if (!winners.Find(x => MultiplayerManager.instance.alivePlayers[i] == x))
            {
                //MAKE THE LOSERS EXPLODE
                MultiplayerManager.instance.alivePlayers[i].GetComponent<Player>().Death(true);
            }
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
    }
}