using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
class MapManager : MonoBehaviour
{
    [SerializeField]
    int _curMapID;
    [SerializeField]
    int _prevMapID;
    [SerializeField]
    int[] _nextMapIDs;
    public int loadedMapsBufferSize;
    public List<string> mapNames;
    void Start()
    {
        GenNextMapsIDs();
        StartCoroutine(LoadScene("2_Map"));
    }
    void GenNextMapsIDs()
    {
        int[] nextMapIDs = new int[loadedMapsBufferSize];
        int lastValue = -1;
        for (int i = 0; i < nextMapIDs.Length; i++)
        {
            nextMapIDs[i] = Random.Range(0, mapNames.Count);
            while (nextMapIDs[i] == lastValue) nextMapIDs[i] = Random.Range(0, mapNames.Count - 1);
            lastValue = nextMapIDs[i];
        }
        _nextMapIDs = nextMapIDs;
    }
    IEnumerator LoadScene(string mapName)
    {
        yield return null;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(mapName);
        asyncOperation.allowSceneActivation = false;
        if (asyncOperation.isDone) asyncOperation.allowSceneActivation = true;
        yield return null;
    }
}