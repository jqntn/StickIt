using System.Collections;
using UnityEngine;
using TMPro;

public class EndScore2 : MonoBehaviour
{
    [Header("ANIMATION__________________________")]
    public float timeBetweenRankAppear = 1.0f;
    public float vfxTime = 2.0f;
    [Header("CANVAS ELEMENTS____________________")]
    public GameObject[] panelPlayers;
    public TMP_Text[] textP;
    public TMP_Text[] textScores;
    

    [Header("DEBUG___________________________")]
    [SerializeField] private Player[] ranking;

    IEnumerator Start()
    {
        while(MultiplayerManager.instance.players.Count <= 0) { yield return null; }

        ranking = new Player[MultiplayerManager.instance.players.Count];
        MultiplayerManager.instance.players.CopyTo(ranking);

        // Debug
        ranking[2].myDatas.score = 10;
        ranking[1].myDatas.score = 5;

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

        foreach (GameObject panel in panelPlayers)
        {
            panel.SetActive(false);
        }

        for (int i = 0; i < ranking.Length; i++)
        {
            panelPlayers[i].SetActive(true);
            textP[i].text = "P" + ranking[i].myDatas.id.ToString();
            textScores[i].text = ranking[i].myDatas.score.ToString();
            yield return new WaitForSeconds(timeBetweenRankAppear);
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
