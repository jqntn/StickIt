using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerManager : Level
{
    public RaceDirection direction;
    private Queue<Player> orderPlayer = new Queue<Player>();

    public static RunnerManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    public void AddOrder(Player player)
    {
        orderPlayer.Enqueue(player);
        winners.Add(player);
        //if(orderPlayer.Count == )
    }

    public Queue<Player> GetOrder()
    {
        return orderPlayer;
    }
}
