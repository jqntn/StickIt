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
    public List<GameObject> players = new List<GameObject>();
    

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
       Initialization();
    }

    private void Initialization()
    {
        print(Gamepad.all.Count);
        for (int i = 0; i < nbrOfPlayer; i++)
        {
            PlayerInput newPlayer = PlayerInput.Instantiate(_prefabPlayer.gameObject, i, "Gamepad", -1, Gamepad.all[i]);
            newPlayer.transform.position = _playersStartingPos.GetChild(i).position;
            Player scriptPlayer = newPlayer.transform.GetComponent<Player>();
            print(scriptPlayer.gameObject.name);
            scriptPlayer.id = i;

            players.Add(scriptPlayer.gameObject);
        }

    }


}
