using System.Collections;
using UnityEngine;
using TMPro;

public class EndScore2 : MonoBehaviour
{
    [Header("TEST_______________________________")]
    public bool isStartingDirect = true;
    [Header("ANIMATION__________________________")]
    public float timeBetweenRankAppear = 1.0f;
    public float vfxTime = 2.0f;
    public float timeToUnlockController = 1.0f;
    [Header("CANVAS ELEMENTS____________________")]
    public GameObject[] panelPlayers;
    public TMP_Text[] textP;
    public TMP_Text[] textScores;
    [Header("HIERARCHY ELEMENTS_________________")]
    public Transform[] startPos;
    public GameObject[] canvasRank;
    
    [Header("DEBUG___________________________")]
    [SerializeField] private Player[] ranking;

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
    }

    private void Start()
    {
        // Debug
        if (isStartingDirect)
        {
            EndGame();
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
        MultiplayerManager.instance.players.CopyTo(ranking);
        foreach(Player player in ranking)
        {
            player.myMouvementScript.enabled = false;
        }

        // Debug
        if (isStartingDirect) { ranking[1].myDatas.score = 5; }

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


        int posIndex = 0;
        foreach(Player player in ranking)
        {
            player.transform.position = startPos[posIndex].position;
            posIndex++;
        }

        
        for (int i = 0; i < ranking.Length; i++)
        {
            yield return new WaitForSeconds(timeBetweenRankAppear);
            panelPlayers[i].SetActive(true);
            textP[i].text = "P" + ranking[i].myDatas.id.ToString();
            textScores[i].text = ranking[i].myDatas.score.ToString();
            canvasRank[i].SetActive(true);
        }

        yield return new WaitForSeconds(timeToUnlockController);
        // unlock player controllers
        foreach (Player player in ranking)
        {
            player.myMouvementScript.enabled = true;
        }
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
}
