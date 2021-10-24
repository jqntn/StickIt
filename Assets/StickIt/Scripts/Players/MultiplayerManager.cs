using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiplayerManager : MonoBehaviour
{
    public static MultiplayerManager instance;

    public int nbrOfPlayer;

    [SerializeField] private Transform _prefabPlayer;
    [SerializeField] private Transform _playersStartingPos;
    [Header("------------DEBUG------------")]
    public List<Player> players = new List<Player>();
    public List<Player> alivePlayers = new List<Player>();
    public List<Player> deadPlayers = new List<Player>();

    public List<Material> materials = new List<Material>();
    private void Awake()
    {
        Initialization();
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
       print(Gamepad.all.Count);
    }

    private void Initialization()
    {
        for (int i = 0; i < nbrOfPlayer; i++)
        {
            PlayerInput newPlayer = null;
            if (i < Gamepad.all.Count)
            newPlayer = PlayerInput.Instantiate(_prefabPlayer.gameObject, i, "Gamepad", -1, Gamepad.all[i]);
            else newPlayer = PlayerInput.Instantiate(_prefabPlayer.gameObject, i, "Gamepad", -1);

            newPlayer.transform.position = _playersStartingPos.GetChild(i).position;
            Player scriptPlayer = newPlayer.transform.GetComponent<Player>();
            scriptPlayer.id = i;
            newPlayer.gameObject.name = "Player" + i.ToString();

            players.Add(scriptPlayer);
            alivePlayers.Add(scriptPlayer);

            scriptPlayer.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = materials[i];
            print(materials[i]);
            if (scriptPlayer.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>())
                print("yes");

        }
    }


}
