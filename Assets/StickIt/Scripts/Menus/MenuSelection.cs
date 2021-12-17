using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class MenuSelection : MonoBehaviour
{
    [SerializeField] private Transform _prefabPlayer;
    [SerializeField] private int nbOfPlayersToLaunch;
    private Transform _playersStartingPos;
    public List<Material> materials = new List<Material>();
    private int counterID = 0;
    [SerializeField] private Animator animLaunchGame;
    [Header("----------- ANIMATIONS -----------")]
    private bool[] isSpawnDeactivated = new bool[4];
    private List<int> devicesID = new List<int>();
    private List<Tuyau> tuyauxList = new List<Tuyau>();
    public float animTime = 0.5f;
    public float yOffset = 1f;
    public AnimationCurve curve;
    public void Menu()
    {
        Time.timeScale = 1;
        AkSoundEngine.PostEvent("Play_SFX_UI_Return", gameObject);
        SceneManager.LoadScene("0_MainMenu");
        Destroy(MultiplayerManager.instance.gameObject);
        MultiplayerManager.instance = null;
        Destroy(MainCamera.instance.gameObject);
        MainCamera.instance = null;
        Destroy(MapManager.instance.gameObject);
        MapManager.instance = null;
        Destroy(Sun.instance.gameObject);
        Sun.instance = null;
        foreach (var item in GameObject.FindGameObjectsWithTag("Player")) Destroy(item);
    }
    private void Start()
    {
        _playersStartingPos = FindObjectOfType<PlayerStartingPos>().transform;
        for (int i = 0; i < 4; i++)
        {
            Tuyau tuyau = transform.GetChild(0).GetChild(i).GetComponent<Tuyau>();
            tuyauxList.Add(tuyau);
        }
    }
    private void Update()
    {
        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            if (Gamepad.all[i].buttonEast.wasPressedThisFrame) { Menu(); return; }
            if (Gamepad.all[i].buttonSouth.wasPressedThisFrame && MultiplayerManager.instance.players.Count < 5)
            {
                bool isAlreadyActivated = false;
                // --- CHECK PLAYERS DEVICE ID
                //foreach (Player player in MultiplayerManager.instance.players)
                //{
                //    if (player.myDatas.deviceID == Gamepad.all[i].deviceId)
                //    {
                //        isAlreadyActivated = true;
                //    }
                //}
                // --- CHECK FOR DEVICES ID IN LIST
                for (int j = 0; j < devicesID.Count; j++)
                {
                    if (Gamepad.all[i].deviceId == devicesID[j])
                    {
                        isAlreadyActivated = true;
                    }
                }
                if (isAlreadyActivated) continue; // ----- RETURN CONDITION
                devicesID.Add(Gamepad.all[i].deviceId);
                AddPlayer(Gamepad.all[i], counterID);
                counterID++;
            }
            else if (MultiplayerManager.instance.players.Count >= nbOfPlayersToLaunch)
            {
                if (Gamepad.all[i].startButton.isPressed)
                {
                    LaunchGame();
                }
            }
        }
    }
    private void AddPlayer(Gamepad gamepad, int i)
    {
        tuyauxList[i].menuSelection = this;
        tuyauxList[i].gamepad = gamepad;
        tuyauxList[i].id = i;
        tuyauxList[i].PlayAnimation();
    }
    public void SpawnPlayer(Gamepad gamepad, int i)
    {
        // Instantiate Player
        PlayerInput newPlayer = null;
        newPlayer = PlayerInput.Instantiate(_prefabPlayer.gameObject, i, "Gamepad", -1, gamepad);
        Player scriptPlayer = newPlayer.transform.GetComponentInParent<Player>();
        scriptPlayer.transform.position = _playersStartingPos.GetChild(i).position;
        // Set Datas
        scriptPlayer.myDatas.id = i;
        scriptPlayer.myDatas.deviceID = gamepad.deviceId;
        scriptPlayer.myDatas.name = "Player" + i.ToString();
        scriptPlayer.myDatas.material = materials[i];
        scriptPlayer.myDatas.mass = MultiplayerManager.instance.initialMass;
        scriptPlayer.gameObject.name = scriptPlayer.myDatas.name;
        if (scriptPlayer.transform.parent)
        {
            scriptPlayer.transform.GetComponentInChildren<MeshRenderer>().material = scriptPlayer.myDatas.material;
        }
        else
        {
            scriptPlayer.transform.GetComponentInChildren<SkinnedMeshRenderer>().material = scriptPlayer.myDatas.material;
        }
        MultiplayerManager.instance.players.Add(scriptPlayer);
        MultiplayerManager.instance.alivePlayers.Add(scriptPlayer);
        if (MultiplayerManager.instance.players.Count == 2)
        {
            animLaunchGame.SetTrigger("Entry");
        }
    }

    public void LaunchGame()
    {
        foreach (Player player in MultiplayerManager.instance.players)
        {
            MultiplayerManager.instance.SaveDatas(player.myDatas);
            player.PrepareToChangeLevel();
        }
        MapManager.instance.NextMap("", true);
    }
}