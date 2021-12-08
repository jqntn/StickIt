using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EndScore : MonoBehaviour
{
    public GameObject[] panelPlayers;
    public TMP_Text[] textP;
    public TMP_Text[] textScores;
    public RawImage[] rawImages;

    [Header("Debug____________________")]
    [SerializeField] protected Queue<Player> rankPlayers = new Queue<Player>();
    [SerializeField] protected Queue<uint> scores = new Queue<uint>();

    private void OnEnable()
    {
        List<Player> players = MultiplayerManager.instance.players;
        //foreach(Player player in players)
        //{
            
        //}
    }
}
