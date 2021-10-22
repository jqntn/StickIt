using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicalChairManager : MonoBehaviour
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
    [Header("Players")]
    public List<GameObject> playersInGame;
    public List<GameObject> chosenOnes;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < GameManager.gameManager.players.Length; i++)
        {
            playersInGame.Add(GameManager.gameManager.players[i]);
        }
        chairs = FindObjectsOfType<Chair>();
        inTransition = true;
        transition = transitionValue;
        maxChairsActive = playersInGame.Count - 1;
    }

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
            if(duration <= 0)
            {
                inTransition = true;
                ResetChairPool();
            }
        }

        countdown.text = textValue.ToString();
    }

    private void ChangeChairPool()
    {
        int rand = Random.Range(0, chairs.Length);
        int chairsChanged = 0;
        while (chairsChanged < maxChairsActive && !chairs[rand].isActive)
        {
            chairs[rand].ActivateChair(colorChairActive);
            chairsChanged++;
            rand = Random.Range(0, chairs.Length);
        }
    }
    private void ResetChairPool()
    {
        foreach (Chair c in chairs)
        {
            c.DeactivateChair(colorChairInactive);
        }
        foreach(GameObject g in playersInGame)
        {
            if(!chosenOnes.Find(x => g))
            {
                //MAKE THE LOSERS EXPLODE
                //g.GetComponent<Player>().Die();
                playersInGame.Remove(g);
            }
        }
        maxChairsActive = playersInGame.Count - 1;
    }
}
