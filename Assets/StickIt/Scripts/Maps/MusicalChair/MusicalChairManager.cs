using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicalChairManager : MonoBehaviour
{
    [Header("Countdown")]
    [SerializeField] float durationValue;
    GameManager _gameManager;
    MultiplayerManager _multiplayerManager;
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
    [Header("Players")]
    //public List<Player> playersInGame;
    public List<Player> chosenOnes;
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameManager.instance;
        _multiplayerManager = MultiplayerManager.instance;
        chairs = FindObjectsOfType<Chair>();
        inTransition = true;
        transition = transitionValue;
        maxChairsActive = _multiplayerManager.alivePlayers.Count - 1;
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
        /*for(int i = chosenOnes.Count-1; i >= 0; i--)
        {
            if (_multiplayerManager.alivePlayers.Find(x => chosenOnes[i].id != x.id))
            {
                //MAKE THE LOSERS EXPLODE
                Debug.Log(_multiplayerManager.players[i].name + " DIES");
                _multiplayerManager.alivePlayers[i].GetComponent<Player>().Death();
            }
        }*/
        for(int i = _multiplayerManager.alivePlayers.Count-1; i >= 0; i--)
        {

            if (!chosenOnes.Find(x => _multiplayerManager.alivePlayers[i] == x))
            {
                //MAKE THE LOSERS EXPLODE
                Debug.Log(_multiplayerManager.players[i].name + " DIES");
                _multiplayerManager.alivePlayers[i].GetComponent<Player>().Death();
            }
        }
        maxChairsActive = _multiplayerManager.alivePlayers.Count - 1;
        chosenOnes.Clear();
    }
}
