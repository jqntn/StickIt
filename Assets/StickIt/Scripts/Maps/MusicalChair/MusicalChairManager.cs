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
    public List<Player> playersInGame;
    public List<Player> chosenOnes;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < GameManager.gameManager.players.Length; i++)
        {
            playersInGame.Add(GameManager.gameManager.players[i].GetComponent<Player>());
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
        for(int i = playersInGame.Count-1; i >= 0; i--)
        {
            if (chosenOnes.Find(x => playersInGame[i].id != x.id))
            {
                //MAKE THE LOSERS EXPLODE
                //g.GetComponent<Player>().Die();
                Debug.Log(playersInGame[i].name + " DIES");
                playersInGame.Remove(playersInGame[i]);
            }
        }
        maxChairsActive = playersInGame.Count - 1;
        chosenOnes.Clear();
        if(playersInGame.Count <= 1)
        {
            GameManager.gameManager.ChangeMod();
        }
    }
}
