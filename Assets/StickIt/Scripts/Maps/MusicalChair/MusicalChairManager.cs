using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MusicalChairManager : Level
{
    [Header("Countdown")]
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
    public Color colorChairTaken;
    public GameObject winTxt;
    // Update is called once per frame
    void Update()
    {
        UpdateText();
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
                ChangeChairPool();
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
                ResetChairPool();
            }
        }
        countdown.text = textValue.ToString();
    }
    protected override void StartMap()
    {
        base.StartMap();
        chairs = FindObjectsOfType<Chair>();
        inTransition = true;
        transition = transitionValue;
        maxChairsActive = _multiplayerManager.alivePlayers.Count - 1;
        print(_multiplayerManager.alivePlayers.Count);
    }
    private void ChangeChairPool()
    {
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
        foreach (Chair c in chairs)
        {
            c.DeactivateChair(colorChairInactive);
        }
        for (int i = _multiplayerManager.alivePlayers.Count - 1; i >= 0; i--)
        {
            if (!winners.Find(x => _multiplayerManager.alivePlayers[i] == x))
            {
                //MAKE THE LOSERS EXPLODE
                _multiplayerManager.alivePlayers[i].GetComponent<Player>().Death();
            }
        }
        maxChairsActive = _multiplayerManager.alivePlayers.Count - 1;
        winners.Clear();
        // FIN LEVEL
        if (_multiplayerManager.alivePlayers.Count == 1)
        {
            winTxt.transform.parent.parent.gameObject.SetActive(true);
            winTxt.GetComponent<Text>().text = _multiplayerManager.alivePlayers[0].myDatas.name + " win!";
        }
        else if (_multiplayerManager.alivePlayers.Count <= 0)
        {
            winTxt.transform.parent.parent.gameObject.SetActive(true);
            winTxt.GetComponent<Text>().text = "Only losers...";
        }
    }
}