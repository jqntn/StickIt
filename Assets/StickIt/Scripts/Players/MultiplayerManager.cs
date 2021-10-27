using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
public class MultiplayerManager : MonoBehaviour
{

    public struct PlayerData
    {
        public string name;
        public int id;
        public int deviceID;
        public Material material;

        public PlayerData(string _name, int _id, int _deviceID, Material _material)
        {
            name = _name;
            id = _id;
            deviceID = _deviceID;
            material = _material;
        }
    }
    

    public static MultiplayerManager instance;
    public List<Material> materialsTemp = new List<Material>();
    public int nbrOfPlayer;
    [SerializeField] private Transform _prefabPlayer;
    Transform playersStartingPos;
    [Header("------------DEBUG------------")]
    public List<Player> players = new List<Player>();
    public List<Player> alivePlayers = new List<Player>();
    public List<Player> deadPlayers = new List<Player>();

    public List<Material> materials = new List<Material>();

    List<PlayerData> datas = new List<PlayerData>();


    int nbrDevicesLastFrame = 0;

#if UNITY_EDITOR
    [SerializeField] bool isMenuSelection = false;
#endif

    private void Awake()
    {

      // Initialization();
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(this);

    }

    private void Start()
    {
        playersStartingPos = FindObjectOfType<PlayerStartingPos>().transform;
        print(Gamepad.all.Count);
#if UNITY_EDITOR
        InitializePlayersWithoutMenuSelector(nbrOfPlayer);
#endif
    }

    //private void Initialization()
    //{
    //    nbrDevicesLastFrame = InputSystem.devices.Count;
    //    for (int i = 0; i < nbrOfPlayer; i++)
    //    {
    //        PlayerInput newPlayer = null;
    //        if (i < Gamepad.all.Count)
    //        newPlayer = PlayerInput.Instantiate(_prefabPlayer.gameObject, i, "Gamepad", -1, Gamepad.all[i]);
    //        else newPlayer = PlayerInput.Instantiate(_prefabPlayer.gameObject, i, "Gamepad", -1);

    //        newPlayer.transform.position = playersStartingPos.GetChild(i).position;
    //        Player scriptPlayer = newPlayer.transform.GetComponent<Player>();
    //        //scriptPlayer.id = i;
    //        newPlayer.gameObject.name = "Player" + i.ToString();

    //        players.Add(scriptPlayer);
    //        alivePlayers.Add(scriptPlayer);

    //        scriptPlayer.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = scriptPlayer.myDatas.material;

    //        //newPlayer.deviceLostEvent.AddListener((newPlayer) => LostDevice(newPlayer));
    //        //newPlayer.deviceRegainedEvent.AddListener((newPlayer) => RegainDevice(newPlayer));


    //    }
    //}

    public void SaveDatas(PlayerData playerData)
    {
        datas.Add(playerData);
    }

    void InitializePlayersWithoutMenuSelector(int numberOfPlayer)
    {
        for(int i = 0; i < numberOfPlayer; i++)
        {
            PlayerData newData = new PlayerData("Player" + i.ToString(), i, -1, materials[i]);
            datas.Add(newData);
        }

        for (int i = 0; i < datas.Count; i++)
        {
            PlayerInput newPlayer = null;
            Gamepad gamepad = null;
            foreach (Gamepad pad in Gamepad.all)
            {
                if (pad.deviceId == datas[i].deviceID)
                {
                    gamepad = pad;
                    break;
                }
            }
            newPlayer = PlayerInput.Instantiate(_prefabPlayer.gameObject, datas[i].id, "Gamepad", -1, gamepad);
            Player scriptPlayer = newPlayer.transform.GetComponent<Player>();
            scriptPlayer.myDatas = datas[i];
            scriptPlayer.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = scriptPlayer.myDatas.material;
            players.Add(scriptPlayer);
            alivePlayers.Add(scriptPlayer);

            newPlayer.transform.position = playersStartingPos.GetChild(i).position;

        }

    }



    public void ChangeMap()
    {
        playersStartingPos = FindObjectOfType<PlayerStartingPos>().transform;
        alivePlayers = players;
        deadPlayers.Clear();
        RespawnPlayers();
    }

    public void RespawnPlayers()
    {

    }

    //public void InstantiatePlayers()
    //{
    //    players.Clear();
    //    alivePlayers.Clear();
        
    //}

    //private void LateUpdate()
    //{
    //    if(InputSystem.devices.Count > nbrDevicesLastFrame)
    //    {
    //        AddDevice();
    //    }
    //    nbrDevicesLastFrame = InputSystem.devices.Count;

    //}

    //private void LostDevice(PlayerInput playerInput)
    //{
    //    print("Device lost");
    //}

    //private void RegainDevice(PlayerInput playerInput)
    //{
    //    print("Device regain");
    //}

    //private void AddDevice()
    //{
    //    //for (int i = 0; i < players.Count; i++)
    //    //{
    //    //    PlayerInput pi = players[i].GetComponent<PlayerInput>();
    //    //    if (pi.devices.Count == 0)
    //    //    {
    //    //        UnityEngine.InputSystem.Users.InputUser.PerformPairingWithDevice(Gamepad.all[Gamepad.all.Count - 1], pi.user);
    //    //        print("Paired");
    //    //        return;
    //    //    }

    //    //}
    //}




}
