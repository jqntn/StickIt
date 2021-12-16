using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class EndScore2 : MonoBehaviour
{
    [Header("TEST_______________________________")]
    public bool isStartingDirect = true;

    [Header("DATA_______________________________")]
    public bool hasTimer = false;
    public float timerBeforeReturnToMenu = 10.0f;
    [SerializeField] private float secondsToPress = 1.0f;

    [Header("ANIMATION__________________________")]
    public float timeBetweenRankAppear = 1.0f;
    public float vfxTime = 2.0f;
    public float timeToUnlockController = 1.0f;

    [Header("CANVAS ELEMENTS____________________")]
    public GameObject[] panelPlayers;
    public TMP_Text[] textP;
    public TMP_Text[] textScores;
    public GameObject returnToMenu;
    public ParticleSystem victory;

    [Header("HIERARCHY ELEMENTS_________________")]
    public Transform[] startPos;
    public GameObject[] canvasRank;
    public TMP_Text[] textRank;
    
    [Header("DEBUG___________________________")]
    [SerializeField] private Player[] ranking;
    [SerializeField] private PlayerAnimations[] animations;
    [SerializeField] private PlayerInput[] playerinputs;
    [SerializeField] private float timer = 0.0f;
    [SerializeField] private PlayerInputs controller;

    private void OnEnable()
    {
        foreach (GameObject panel in panelPlayers)
        {
            panel.SetActive(false);
        }

        foreach(GameObject canvas in canvasRank)
        {
            canvas.SetActive(false);
        }

        GameEvents.OnSwitchCamera.AddListener(EndGame);
        timer = 0.0f;
        returnToMenu.SetActive(false);
        Canvas canva = GetComponent<Canvas>();
        canva.worldCamera = Camera.main;

    }


    private void Start()
    {
        // Debug
        if (isStartingDirect)
        {
            EndGame();
        }
        AkSoundEngine.PostEvent("Stop_Music_Main", gameObject);
        AkSoundEngine.PostEvent("Play_Music_End", gameObject);
    }

    
    private void Update()
    {
        if (hasTimer)
        {
            if (timer < timerBeforeReturnToMenu)
            {
                timer += Time.deltaTime;
                return;
            }

            Menu();
        }

    }

    public void EndGame()
    {
        StartCoroutine(OnEndGame());
    }

    IEnumerator OnEndGame()
    {
        while(MultiplayerManager.instance.players.Count <= 0) { yield return null;}

        ranking = new Player[MultiplayerManager.instance.players.Count];
        playerinputs = new PlayerInput[MultiplayerManager.instance.players.Count];
        animations = new PlayerAnimations[MultiplayerManager.instance.players.Count];
        MultiplayerManager.instance.players.CopyTo(ranking);

        // Sort Ranking (slow sorting > change to quicksort)
        bool hasPermute = false;
        do
        {
            hasPermute = false;
            for (int i = 0; i < ranking.Length - 1; i++)
            {
                if (ranking[i].myDatas.score < ranking[i + 1].myDatas.score)
                {
                    Swap(i, i + 1);
                    hasPermute = true;
                }
            }
        } while (hasPermute);


        int k = 0;
        foreach(Player player in ranking)
        {
            // Disable Player Controller
            PlayerInput playerInput = player.GetComponent<PlayerInput>();
            playerInput.enabled = false;
            playerinputs[k] = playerInput;
            
            // Save PlayerAnimations
            PlayerAnimations playerAnimation = player.GetComponent<PlayerAnimations>();
            animations[k] = playerAnimation;
            k++;
        }

        // Debug
        //if (isStartingDirect) { ranking[1].myDatas.score = 5; }

        // Adding listerner to Return to menu
        //controller = ranking[0].GetComponent<PlayerInput>();
        controller = new PlayerInputs();

        int posIndex = 0;
        foreach(Player player in ranking)
        {
            player.transform.position = startPos[posIndex].position;
            posIndex++;
        }

        for (int i = 0; i < ranking.Length; i++)
        {
            yield return new WaitForSeconds(timeBetweenRankAppear);
            textP[i].color = ranking[i].myDatas.material.color;
            textP[i].text = "Player " + (ranking[i].myDatas.id + 1).ToString();
            textScores[i].color = ranking[i].myDatas.material.color;
            textScores[i].text = ranking[i].myDatas.score.ToString();
            textRank[i].color = ranking[i].myDatas.material.color;
            canvasRank[i].SetActive(true);
            panelPlayers[i].SetActive(true);

            if(i == 0)
            {
                animations[i].PlayVictory();
            }
            else
            {
                animations[i].PlayRank();
            }

            if(i == 0)
            {
                yield return new WaitForFixedUpdate();
                victory.Play();

            }

        }

        yield return new WaitForSeconds(timeToUnlockController);

        // Stop Animation
        foreach (PlayerAnimations anim in animations)
        {
            anim.IsJumpingAnim = false;
        }

        // Unlock player controllers
        foreach (PlayerInput input in playerinputs)
        {
            input.enabled = true;
        }

        controller.Enable();
        returnToMenu.SetActive(true);
        controller.NormalInputs.Menu.performed += _ => Menu();
    }

    private void Swap(int i, int j)
    {
        Player temp = ranking[i];
        ranking[i] = ranking[j];
        ranking[j] = temp;
    }

    IEnumerator OnEndScreen()
    {
        yield return null;
    }

    public void Menu()
    {
        StartCoroutine(PressCoroutine(() =>
        {
            Time.timeScale = 1;
            AkSoundEngine.PostEvent("Play_SFX_UI_Submit", gameObject);
            Destroy(MultiplayerManager.instance.gameObject);
            MultiplayerManager.instance = null;
            Destroy(MainCamera.instance.gameObject);
            MainCamera.instance = null;
            Destroy(MapManager.instance.gameObject);
            MapManager.instance = null;
            Destroy(Sun.instance.gameObject);
            Sun.instance = null;
            foreach (var item in GameObject.FindGameObjectsWithTag("Player")) Destroy(item);
            Pause.instance = null;
            SceneManager.LoadScene("0_MainMenu");

            AkSoundEngine.PostEvent("Play_Music_Main", gameObject);
            AkSoundEngine.PostEvent("Stop_Music_End", gameObject);
        }));
    }

    public IEnumerator PressCoroutine(Action func)
    {
        yield return new WaitForSecondsRealtime(secondsToPress);
        func?.Invoke();
    }
}
