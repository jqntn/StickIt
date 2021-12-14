using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class MultiplayerManager : MonoBehaviour
{
    [Serializable]
    public struct PlayerData
    {
        public string name;
        public int id;
        public int deviceID;
        public Material material;
        public RenderTexture renderTexture;
        public int mass;
        public uint score;
        public uint nbrDeath;
        public uint nbrVictories;
        public PlayerData(string _name, int _id, int _deviceID, Material _material, RenderTexture _renderTexture)
        {
            name = _name;
            id = _id;
            deviceID = _deviceID;
            material = _material;
            renderTexture = _renderTexture;
            mass = 100;
            score = 0;
            nbrDeath = 0;
            nbrVictories = 0;
        }
    }
    public static MultiplayerManager instance;
    public List<Material> materialsTemp = new List<Material>();
    public int nbrOfPlayer;
    [SerializeField] private Transform _prefabPlayer;
    [SerializeField] private AnimationCurve curve_ChangeMap_PosX;
    [SerializeField] private AnimationCurve curve_ChangeMap_PosY;
    [Header("------------DEBUG------------")]
    public List<Player> players = new List<Player>();
    public List<Player> alivePlayers = new List<Player>();
    public List<Player> deadPlayers = new List<Player>();
    public List<Material> materials = new List<Material>();
    public RenderTexture[] renderTextures;
    private List<PlayerData> datas = new List<PlayerData>();
    private Transform playersStartingPos;
    //  private int nbrDevicesLastFrame = 0;
    [HideInInspector] public float speedChangeMap = 1;
    private float t = 0f;
    private float y = 0f;
    private float[] initPosX;
    private float[] initPosY;
    public bool isChangingMap = false;

    public int massAddIfWin;
    public int[] massRemoveIfLoss;
    public uint scoreAddIfWin;
    public uint[] scoreAddIfLoss;

    [SerializeField] public bool isMenuSelection = true; // should be private

    private void Awake()
    {
        // Initialization();
        if (instance == null) instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(this);
#if UNITY_EDITOR
        if (SceneManager.GetActiveScene().name == "1_MenuSelection")
        {
            isMenuSelection = true;
        }
        else
        {
            isMenuSelection = false;
        }
#endif
    }
    private void Start()
    {
        playersStartingPos = FindObjectOfType<PlayerStartingPos>().transform;
#if UNITY_EDITOR
        if (!isMenuSelection)
            InitializePlayersWithoutMenuSelector(nbrOfPlayer);

        Level lvl = FindObjectOfType<Level>();
        if (lvl != null) StartCoroutine(lvl.Init());
#endif

    }
    private void Update()
    {
        if (isChangingMap)
        {
            LerpDuringChangeMap();
        }
    }
    public void SaveDatas(PlayerData playerData)
    {
        datas.Add(playerData);
    }
#if UNITY_EDITOR
    public void InitializePlayersWithoutMenuSelector(int numberOfPlayer)
    {
        for (int i = 0; i < numberOfPlayer; i++)
        {
            PlayerData newData = new PlayerData("Player" + i.ToString(), i, -1, materials[i], renderTextures[i]);
            datas.Add(newData);
        }
        for (int i = 0; i < datas.Count; i++)
        {
            Gamepad pad = null;
            PlayerInput newPlayer = PlayerInput.Instantiate(_prefabPlayer.gameObject, datas[i].id, "Gamepad", -1, pad);
            Player scriptPlayer = newPlayer.transform.GetComponent<Player>();
            scriptPlayer.myDatas = datas[i];
            scriptPlayer.transform.gameObject.name = scriptPlayer.myDatas.name;
            scriptPlayer.transform.GetComponentInChildren<SkinnedMeshRenderer>().material = scriptPlayer.myDatas.material;
            scriptPlayer.transform.GetComponentInChildren<Camera>().targetTexture = scriptPlayer.myDatas.renderTexture;
            players.Add(scriptPlayer);
            alivePlayers.Add(scriptPlayer);
            newPlayer.transform.position = playersStartingPos.GetChild(i).position;
        }

    }
#endif
    public void StartChangeMap(Transform nextStartPos)
    {
        SetMassEndLVL();
        playersStartingPos = nextStartPos;
        // Disable the players
        foreach (Player player in players)
        {
            player.PrepareToChangeLevel();
        }
        initPosX = new float[players.Count];
        initPosY = new float[players.Count];
        for (int i = 0; i < players.Count; i++)
        {
            initPosX[i] = players[i].transform.position.x;
            initPosY[i] = players[i].transform.position.y;
        }
        t = 0f;
        isChangingMap = true;
    }
    private void LerpDuringChangeMap()
    {
        for (int i = 0; i < players.Count; i++)
        {
            t += Time.unscaledDeltaTime * speedChangeMap;
            y = t;
            y = curve_ChangeMap_PosX.Evaluate(y);
            float currentPosX = Mathf.Lerp(initPosX[i], playersStartingPos.GetChild(i).transform.position.x, y);
            y = t;
            y = curve_ChangeMap_PosY.Evaluate(y);
            float currentPosY = Mathf.Lerp(playersStartingPos.GetChild(i).transform.position.y, initPosY[i], 1 - y);
            players[i].transform.position = new Vector3(currentPosX, currentPosY);
            if (y >= 1)
            {
                EndChangeMap();
            }
        }
    }
    public void EndChangeMap()
    {
        isChangingMap = false;
        // Reset the lists and re-enable the players
        alivePlayers = new List<Player>(players);
        deadPlayers.Clear();
        RespawnPlayers();
    }
    public void RespawnPlayers()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].Respawn();
        }
    }

    public void SetMassEndLVL()
    {
        // Winners
        bool isAWinner = false;
        for (int i = 0; i < alivePlayers.Count; i++)
        {
            alivePlayers[i].SetScoreAndMass(scoreAddIfWin, massAddIfWin) ;
            isAWinner = true;
            print(i);
        }

        // Losers
        print(deadPlayers.Count);
        for (int i = 0; i < deadPlayers.Count; i++)
        {
            int i2 = (isAWinner) ? i + 1 : i;
            deadPlayers[i].SetScoreAndMass(scoreAddIfLoss[i2], -massRemoveIfLoss[i2]);
            print(-massRemoveIfLoss[i2]);
        }
    }
}