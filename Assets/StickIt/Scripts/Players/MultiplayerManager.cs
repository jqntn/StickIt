using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class MultiplayerManager : MonoBehaviour
{
    [Range(0, 4)]
    public int nbrOfPlayer;
    [SerializeField] private Transform _prefabPlayer;

    [Header("----------- Animation -----------")]
    public AnimationCurve curve_ChangeMap_PosX;
    public AnimationCurve curve_ChangeMap_PosY;

    [Header("------------DEBUG------------")]
    public List<Player> players = new List<Player>();
    public List<Player> alivePlayers = new List<Player>();
    public List<Player> deadPlayers = new List<Player>();
    public List<Material> materials = new List<Material>();

    private List<PlayerData> datas = new List<PlayerData>();
    private Transform playersStartingPos;
    private int nbrDevicesLastFrame = 0;
    [HideInInspector] public float speedChangeMap = 1;
    private float t = 0f;
    private float y = 0f;
    private float[] initPosX;
    private float[] initPosY;
    private float[] currentPosX;
    private float[] currentPosY;
    private bool isChangingMap = false;

#if UNITY_EDITOR
    [SerializeField] private bool isMenuSelection = false;
#endif

    public static MultiplayerManager instance;
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(this);
#if UNITY_EDITOR
        // Is Menu Selection ?
        if(SceneManager.GetActiveScene().name == "0_MenuSelection")
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
        if(!isMenuSelection)
        InitializePlayersWithoutMenuSelector(nbrOfPlayer);
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

    void InitializePlayersWithoutMenuSelector(int numberOfPlayer)
    {
        for (int i = 0; i < numberOfPlayer; i++)
        {
            // Creating player data
            PlayerData newData = new PlayerData("Player" + i.ToString(), i, -1, materials[i]);
            datas.Add(newData);

            // Pair device to player
            Gamepad gamepad = null;
            foreach (Gamepad pad in Gamepad.all)
            {
                if (pad.deviceId == datas[i].deviceID)
                {
                    gamepad = pad;
                    break;
                }
            }

            // Creating New Player
            PlayerInput newPlayer = PlayerInput.Instantiate(_prefabPlayer.gameObject, datas[i].id, "Gamepad", -1, gamepad);
            Player scriptPlayer = newPlayer.transform.GetComponent<Player>();
            scriptPlayer.myDatas = datas[i];
            scriptPlayer.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = scriptPlayer.myDatas.material;
            players.Add(scriptPlayer);
            alivePlayers.Add(scriptPlayer);

            // Spawning player at Spawn Point
            newPlayer.transform.position = playersStartingPos.GetChild(i).position;
        }
    }

    public void StartChangeMap()
    {
        // Disable the players
        foreach(Player player in players)
        {
            player.PrepareToChangeLevel();
        }

        // Prepare to lerp Players
        foreach(Transform child in MapManager.instance.nextMapRoot.transform)
        {
            if (child.GetComponent<PlayerStartingPos>())
            {
                playersStartingPos = child;
                break;
            }
        }
        initPosX = new float[players.Count];
        initPosY = new float[players.Count];
        for(int i = 0; i < players.Count; i++)
        {
            initPosX[i] = players[i].transform.position.x;
            initPosY[i] = players[i].transform.position.y;
        }
        t = 0f;
        isChangingMap = true;
    }

    private void LerpDuringChangeMap()
    {
        for(int i = 0; i < players.Count; i++)
        {
            t += Time.unscaledDeltaTime * speedChangeMap;
            y = t;
            y = curve_ChangeMap_PosX.Evaluate(y);
            float currentPosX = Mathf.Lerp(initPosX[i], playersStartingPos.GetChild(i).transform.position.x, y);
            y = t;
            y = curve_ChangeMap_PosY.Evaluate(y);
            float currentPosY = Mathf.Lerp(playersStartingPos.GetChild(i).transform.position.y, initPosY[i] , 1-y);
            print(y);
            players[i].transform.position = new Vector3(currentPosX, currentPosY);
            if(y >= 1)
            {
                EndChangeMap();
            }
        }
    }

    public void EndChangeMap()
    {
        isChangingMap = false;

        // Reset the lists and re-enable the players
        alivePlayers = players;
        deadPlayers.Clear();
        RespawnPlayers();
    }

    public void RespawnPlayers()
    {
        for(int i = 0; i < players.Count; i++)
        {
            players[i].Respawn();
        }
    }
}

