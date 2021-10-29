using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RunnerManager : Level
{
    public Text[] textDebug = new Text[2];
    public RaceDirection direction;
    [Header("-------- End Level Condition ---------")]
    [Tooltip("If one player still in game\n" +
        "He still need to pass the end checkpoint")]
    public bool needToPassEnd = false;
    public bool hasBonus = false;
    [Header("--------- Score Systems ----------")]
    [Tooltip("False = dead don't gain\n" +
        "True = dead gain score")]
    public bool doDeadGainScore = false;
    [Tooltip("Fixed Score =\n" +
        "Order get you fixed amount of score")]
    public bool hasFixedScore = true;
    public uint[] fixedScores = new uint[4];
    //[Tooltip("Divide Score = \n" +
    //    "MaxScoreToDivide divide by number of player and then score gain depending of order\n" +
    //    "Ex : Players = 4, MaxScore = 100\n" +
    //    "1st = 100 / 4 * 4 `= 100\n" +
    //    "2nd = 100 / 4 * 3 = 75\n" +
    //    "3rd = 100 / 4 * 2 = 50\n" +
    //    "4th = 100 / 4 * 1 = 25\n")]
    //public bool hasDivideScore = false;
    //public uint maxScoreToDivide = 100;
    //[Tooltip("Percentage score =\n max Score * percentage depending of the order")]
    //public bool hasPercentageScore = false;
    //public uint maxScore = 100;
    //public uint[] percentageScores = new uint[4];
    //public bool hasFirstOnlyGetScore = false;
    //public uint score = 100;
    //[Tooltip("Dynamic score depend on a timer\n" +
    //    "Dead score higher = timer high\n" +
    //    "Alive score higher = timer low when passing end checkpoint")]
    //public bool hasDynamicScore = false;
    //public uint maxDynamicScore = 100;

    [Header("--------- DEBUG ----------")]
    [SerializeField] private List<float> arriveTimePlayers = new List<float>();
    [SerializeField] private List<float> deadTimePlayers = new List<float>();
    [SerializeField] private uint bonusEnd = 0;
    [SerializeField] private bool hasEnclenchedEndCheckpoint = false;
    [SerializeField] private float timer = 0;
    [SerializeField] private bool hasStopTimer = false;
    private Queue<Player> orderPlayer = new Queue<Player>();
    private Stack<Player> deadPlayer = new Stack<Player>();
   
    public bool hasEndLevel = false;

    private void Update()
    {
        if (hasStopTimer) { return; }
        timer += Time.deltaTime;
    }
    public static RunnerManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    protected override void EndMap()
    {
        // Last player does not need to pass last checkpoint
        if (!needToPassEnd && _multiplayerManager.alivePlayers.Count == 1)
        {
            Player player = _multiplayerManager.alivePlayers[0];
            orderPlayer.Enqueue(player);
            arriveTimePlayers.Add(timer);
            hasStopTimer = true;
        }

        int count = orderPlayer.Count + deadPlayer.Count;
        if (count == _multiplayerManager.nbrOfPlayer)
        {
            hasEndLevel = true;
            DistributeScore();
            //_multiplayerManager.ChangeMap();
        }
    }

    private void ChangeText(Text text, uint score, int i)
    {
        text.text = "p" + (i + 1).ToString() + " : " + score;
    }
    private void DistributeScore()
    {
        short i = 0;
        uint scoreToAdd = 0;

        // Fixed Score
        if (hasFixedScore)
        {
            i = 0;
            while(orderPlayer.Count > 0)
            {
                scoreToAdd = fixedScores[i];
                AddBonus(i, ref scoreToAdd);
                Player player = orderPlayer.Dequeue();
                AddScore(scoreToAdd, player);
                i++;
                ChangeText(textDebug[i], player.myDatas.score, i);
            }

            if (!doDeadGainScore)
            {
                Debug.Log("Dead Player don't gain anything");
                return;
            }

            i = 0;
            while(deadPlayer.Count > 0)
            {
                scoreToAdd = fixedScores[i];
                Player player = deadPlayer.Pop();
                AddScore(scoreToAdd, player);
                i++;
                ChangeText(textDebug[i], scoreToAdd, i);
            }
        }
        //// Divide Score
        //else if (hasDivideScore)
        //{
        //    i = 1;
        //    while(orderPlayer.Count > 0) {

        //        scoreToAdd = (uint)(maxScoreToDivide / i);
        //        AddBonus(i, ref scoreToAdd);
        //        Player player = orderPlayer.Dequeue();
        //        AddScore(scoreToAdd, player);
        //        i++;
        //        ChangeText(textDebug[i], scoreToAdd, i);
        //    }

        //    if (!doDeadGainScore)
        //    {
        //        Debug.Log("Dead Player don't gain anything");
        //        return;
        //    }

        //    i = 1;
        //    while (deadPlayer.Count > 0)
        //    {
        //        scoreToAdd = (uint)(maxScoreToDivide / i);
        //        Player player = deadPlayer.Pop();
        //        AddScore(scoreToAdd, player);
        //        i++;
        //        ChangeText(textDebug[i], scoreToAdd, i);
        //    }
        //}
        //// Percentage Score
        //else if (hasPercentageScore)
        //{
        //    i = 0;
        //    while (orderPlayer.Count > 0)
        //    {
        //        scoreToAdd = maxScore * percentageScores[i] / 100;
        //        AddBonus(i, ref scoreToAdd);
        //        Player player = orderPlayer.Dequeue();
        //        AddScore(scoreToAdd, player);
        //        i++;
        //        ChangeText(textDebug[i], scoreToAdd, i);
        //    }

        //    if (!doDeadGainScore)
        //    {
        //        Debug.Log("Dead Player don't gain anything");
        //        return;
        //    }

        //    i = 0;
        //    while (deadPlayer.Count > 0)
        //    {
        //        scoreToAdd = maxScore * percentageScores[i] / 100;
        //        Player player = deadPlayer.Pop();
        //        AddScore(scoreToAdd, player);
        //        i++;
        //        ChangeText(textDebug[i], scoreToAdd, i);
        //    }
        //}
        //// First Only Get Score
        //else if (hasFirstOnlyGetScore)
        //{
        //    scoreToAdd = maxScore;
        //    Player player = orderPlayer.Dequeue();
        //    AddScore(scoreToAdd, player);
        //    Debug.Log("Other player don't gain anything");
        //}
        //// Dynamic Score
        //else if (hasDynamicScore)
        //{
        //    i = 0;
        //    while (orderPlayer.Count > 0)
        //    {
        //        scoreToAdd = maxDynamicScore / (uint)arriveTimePlayers[i];
        //        AddBonus(i, ref scoreToAdd);
        //        Player player = orderPlayer.Dequeue();
        //        AddScore(scoreToAdd, player);
        //        i++;
        //        ChangeText(textDebug[i], scoreToAdd, i);
        //    }

        //    if (!doDeadGainScore)
        //    {
        //        Debug.Log("Dead Player don't gain anything");
        //        return;
        //    }

        //    i = 0;
        //    while (deadPlayer.Count > 0)
        //    {
        //        scoreToAdd = maxDynamicScore * (uint)deadTimePlayers[i] / 100;
        //        Player player = deadPlayer.Pop();
        //        player.myDatas.score += scoreToAdd;
        //        AddScore(scoreToAdd, player);
        //        i++;
        //        ChangeText(textDebug[i], scoreToAdd, i);
        //    }
        //}
    }

    private void AddScore(uint scoreToAdd, Player player)
    {
        player.myDatas.score += scoreToAdd;
        winners.Add(player);
    }

    private void AddBonus(short i, ref uint scoreToAdd)
    {
        if (i == 0 && hasBonus && hasEnclenchedEndCheckpoint)
        {
            scoreToAdd += bonusEnd;
        }
    }

#region Public Methods
    public void AddOrder(Player player)
    {
        orderPlayer.Enqueue(player);
        EndMap();
    }
    public void AddDeath(Player player)
    {
        deadPlayer.Push(player);
        EndMap();
    }
    public void AddArriveTime(float time)
    {
        arriveTimePlayers.Add(time);
    }
    public void AddDeadTime(float time)
    {
        deadTimePlayers.Add(time);
    }
    public void AddBonus(uint bonus)
    {
        bonusEnd = bonus;
    }
    public void TriggerEndCheckpoint(bool value)
    {
        hasEnclenchedEndCheckpoint = value;
    }
    public Queue<Player> GetOrder()
    {
        return orderPlayer;
    }

    public Stack<Player> GetDead()
    {
        return deadPlayer;
    }

    #endregion
}