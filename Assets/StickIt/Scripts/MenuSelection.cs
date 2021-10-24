using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuSelection : MonoBehaviour
{

    [SerializeField] private Transform _prefabPlayer;
    [SerializeField] private Transform _playersStartingPos;
    public List<Material> materials = new List<Material>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            if (Gamepad.all[i].buttonSouth.isPressed)
            {
                AddPlayer(Gamepad.all[i], i);
            }
        }
    }

    private void AddPlayer(Gamepad gamepad, int i)
    {

        bool isAlreadyActivated = false;
        foreach (Player player in MultiplayerManager.instance.players)
        {
            if (player.myDatas.deviceID == gamepad.deviceId)
            {
                isAlreadyActivated = true;
            }
        }
        if (isAlreadyActivated) return;

        PlayerInput newPlayer = null;
        newPlayer = PlayerInput.Instantiate(_prefabPlayer.gameObject, i, "Gamepad", -1, Gamepad.all[i]);

        newPlayer.transform.position = _playersStartingPos.GetChild(i).position;
        Player scriptPlayer = newPlayer.transform.GetComponent<Player>();
        scriptPlayer.myDatas.id = i;
        scriptPlayer.myDatas.deviceID = gamepad.deviceId;
        scriptPlayer.myDatas.name = "Player" + i.ToString();
        newPlayer.gameObject.name = scriptPlayer.myDatas.name;

        MultiplayerManager.instance.players.Add(scriptPlayer);

        scriptPlayer.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = materials[i];
    }

    private void LaunchGame()
    {

        foreach(Player player in MultiplayerManager.instance.players)
        {
            MultiplayerManager.instance.SaveDatas(player.myDatas);
        }

    }
}
