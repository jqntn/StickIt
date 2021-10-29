using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerManager : Level
{
    public RaceDirection direction;
    [Header("Score Systems")]
    [Tooltip("False = dead don't gain\n" +
        "True = dead gain score")]
    public bool doDeadGainScore = false;
    [Tooltip("Fixed Score =\n" +
        "Order get you fixed amount of score")]
    public bool hasFixedScore = false;
    public uint[] fixedScores = new uint[4];
    [Tooltip("Divide Score = \n" +
        "MaxScoreToDivide divide by number of player and then score gain depending of order\n" +
        "Ex : Players = 4, MaxScore = 100\n" +
        "1st = 100 / 4 * 4 `= 100\n" +
        "2nd = 100 / 4 * 3 = 75\n" +
        "3rd = 100 / 4 * 2 = 50\n" +
        "4th = 100 / 4 * 1 = 25\n")]
    public bool hasDivideScore = false;
    public uint maxScoreToDivide = 100;
    [Tooltip("Percentage score =\n max Score * percentage depending of the order")]
    public bool hasPercentageScore = false;
    public uint maxScore = 100;
    public uint[] percentageScores = new uint[4];
    public bool hasFirstOnlyGetScore = false;
    public uint score = 100;
    [Tooltip("Dynamic score depend on a timer\n" +
        "Dead score higher = timer high\n" +
        "Alive score higher = timer low when passing end checkpoint")]
    public bool hasDynamicScore = false;
    public uint maxDynamicScore = 100;

    [Header("--------- DEBUG ----------")]
    [SerializeField] private List<float> arriveTimePlayers = new List<float>();
    [SerializeField] private List<float> deadTimePlayers = new List<float>();
    private Queue<Player> orderPlayer = new Queue<Player>();
    private Stack<Player> deadPlayer = new Stack<Player>();
    public bool hasEndLevel = false;

    public static RunnerManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    protected override void EndMap()
    {
        int count = orderPlayer.Count + deadPlayer.Count;
        if (count == _multiplayerManager.nbrOfPlayer)
        {
            hasEndLevel = true;
            DistributeScore();
        }
    }

    private void DistributeScore()
    {
        // Fixed Score
        if (hasFixedScore)
        {
            int i = 0;
            while(orderPlayer.Count > 0)
            {
                uint scoreToAdd = fixedScores[i];
                Player player = orderPlayer.Dequeue();
                AddScore(scoreToAdd, player);
                i++;
            }

            if (!doDeadGainScore)
            {
                Debug.Log("Dead Player don't gain anything");
                return;
            }

            i = 0;
            while(deadPlayer.Count > 0)
            {
                uint scoreToAdd = fixedScores[i];
                Player player = deadPlayer.Pop();
                AddScore(scoreToAdd, player);
                i++;
            }
        }
        // Divide Score
        else if (hasDivideScore)
        {
            uint i = 1;
            while(orderPlayer.Count > 0) {

                uint scoreToAdd = maxScoreToDivide / i;
                Player player = orderPlayer.Dequeue();
                AddScore(scoreToAdd, player);
                i++;

            }

            if (!doDeadGainScore)
            {
                Debug.Log("Dead Player don't gain anything");
                return;
            }

            i = 1;
            while (deadPlayer.Count > 0)
            {
                uint scoreToAdd = maxScoreToDivide / i;
                Player player = deadPlayer.Pop();
                AddScore(scoreToAdd, player);
                i++;
            }
        }
        // Percentage Score
        else if (hasPercentageScore)
        {
            int i = 0;
            while (orderPlayer.Count > 0)
            {
                uint scoreToAdd = maxScore * percentageScores[i] / 100;
                Player player = orderPlayer.Dequeue();
                AddScore(scoreToAdd, player);
                i++;
            }

            if (!doDeadGainScore)
            {
                Debug.Log("Dead Player don't gain anything");
                return;
            }

            i = 0;
            while (deadPlayer.Count > 0)
            {

                uint scoreToAdd = maxScore * percentageScores[i] / 100;
                Player player = deadPlayer.Pop();
                AddScore(scoreToAdd, player);
                i++;
            }
        }
        // First Only Get Score
        else if (hasFirstOnlyGetScore)
        {
            uint scoreToAdd = maxScore;
            Player player = orderPlayer.Dequeue();
            AddScore(scoreToAdd, player);
            Debug.Log("Other player don't gain anything");
        }
        // Dynamic Score
        else if (hasDynamicScore)
        {
            int i = 0;
            while (orderPlayer.Count > 0)
            {
                uint scoreToAdd = maxDynamicScore / (uint)arriveTimePlayers[i];
                Player player = orderPlayer.Dequeue();
                AddScore(scoreToAdd, player);
                i++;
            }

            if (!doDeadGainScore)
            {
                Debug.Log("Dead Player don't gain anything");
                return;
            }

            i = 0;
            while (deadPlayer.Count > 0)
            {
                uint scoreToAdd = maxDynamicScore * (uint)arriveTimePlayers[i] / 100;
                Player player = deadPlayer.Pop();
                player.myDatas.score += scoreToAdd;
                AddScore(scoreToAdd, player);
                i++;
            }
        }
    }

    private void AddScore(uint scoreToAdd, Player player)
    {
        player.myDatas.score += scoreToAdd;
        winners.Add(player);
        Debug.Log("Player " + player.myDatas.id + " gain " + scoreToAdd);
    }

#region Public Methods
    public void AddOrder(Player player)
    {
        orderPlayer.Enqueue(player);
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
    public void AddDeath(Player player)
    {
        deadPlayer.Push(player);
        EndMap();
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