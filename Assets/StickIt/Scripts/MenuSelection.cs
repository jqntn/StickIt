using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuSelection : MonoBehaviour
{

    [SerializeField] private Transform _prefabPlayer;
    private Transform _playersStartingPos;
    public List<Material> materials = new List<Material>();

    [SerializeField] Animator animLaunchGame;

    [Header("----------- ANIMATIONS -----------")]
    public List<GameObject> tuyauxList = new List<GameObject>();
    public float animTime = 0.5f;
    public float yOffset = 10.0f;
    public AnimationCurve curve;
    private Coroutine coroutine;
    // Start is called before the first frame update
    void Start()
    {
        _playersStartingPos = FindObjectOfType<PlayerStartingPos>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            if (Gamepad.all[i].buttonSouth.isPressed)
            {
                bool isAlreadyActivated = false;
                foreach (Player player in MultiplayerManager.instance.players)
                {
                    if (player.myDatas.deviceID == Gamepad.all[i].deviceId)
                    {
                        isAlreadyActivated = true;
                    }
                }
                if (isAlreadyActivated) return; // ----- RETURN CONDITION

                int nextID = MultiplayerManager.instance.players.Count;
                AddPlayer(Gamepad.all[i], nextID );
                if(MultiplayerManager.instance.players.Count == 2)
                {
                    animLaunchGame.SetTrigger("Entry");
                }
            } else if (MultiplayerManager.instance.players.Count >= 2)
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
        if(coroutine == null)
        {
            coroutine = StartCoroutine(DoAddPlayer(gamepad, i));
        }
    }

    private IEnumerator DoAddPlayer(Gamepad gamepad, int i)
    {
        // Play tuyau showing
        float timer = 0;
        float startPosY = tuyauxList[i].transform.position.y;
        while(timer < animTime)
        {
            timer += Time.deltaTime;
            float ratio = timer / animTime;
            Vector3 newPos = new Vector3(
                tuyauxList[i].transform.position.x,
                Mathf.Lerp(startPosY, startPosY - yOffset, curve.Evaluate(ratio)),
                tuyauxList[i].transform.position.z);

            tuyauxList[i].transform.position = newPos;
            yield return null;
        }

        // Instantiate Player
        PlayerInput newPlayer = null;
        newPlayer = PlayerInput.Instantiate(_prefabPlayer.gameObject, i, "Gamepad", -1, gamepad);

        newPlayer.transform.position = _playersStartingPos.GetChild(i).position;
        Player scriptPlayer = newPlayer.transform.GetComponent<Player>();
        // Set Datas
        scriptPlayer.myDatas.id = i;
        scriptPlayer.myDatas.deviceID = gamepad.deviceId;
        scriptPlayer.myDatas.name = "Player" + i.ToString();
        scriptPlayer.myDatas.material = materials[i];

        newPlayer.gameObject.name = scriptPlayer.myDatas.name;
        scriptPlayer.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material = scriptPlayer.myDatas.material;

        MultiplayerManager.instance.players.Add(scriptPlayer);

        // Play tuyau unshowing
        timer = 0;
        while (timer < animTime)
        {
            timer += Time.deltaTime;
            float ratio = timer / animTime;
            Vector3 newPos = new Vector3(
                tuyauxList[i].transform.position.x,
                Mathf.Lerp(startPosY - yOffset, startPosY, curve.Evaluate(ratio)),
                tuyauxList[i].transform.position.z);

            tuyauxList[i].transform.position = newPos;
            yield return null;
        }
    }
    public void LaunchGame()
    {

        foreach(Player player in MultiplayerManager.instance.players)
        {
            MultiplayerManager.instance.SaveDatas(player.myDatas);
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1) ;
    }
}
