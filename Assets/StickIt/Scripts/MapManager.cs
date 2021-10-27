using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
class MapManager : Unique<MapManager>
{
    [Range(0, 1)]
    public float smoothTime;
    public float smoothMOE;
    public int mapOffset;
    public string nextMap;
    Coroutine _coroutine;
    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 200, 100), "NextMap")) PrepNextMap();
    }
    public bool PrepNextMap()
    {
        if (_coroutine == null) _coroutine = StartCoroutine(NextMap(nextMap));
        else return false;
        return true;
    }
    IEnumerator NextMap(string nextMapName)
    {
        GameObject curMapRoot = GameObject.Find("MapRoot"), nextMapRoot = null;
        Vector3 v0 = Vector3.zero, v1 = Vector3.zero, d0 = Vector3.one, d1 = Vector3.one;
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(nextMapName, LoadSceneMode.Additive);
        while (!asyncOp.isDone) yield return null;
        Time.timeScale = 0;
        foreach (var i in SceneManager.GetSceneByName(nextMapName).GetRootGameObjects())
            if (i.name == "MapRoot") { nextMapRoot = i; nextMapRoot.transform.position = new Vector3(mapOffset, 0); break; }
        while (d0.sqrMagnitude > smoothMOE && d1.sqrMagnitude > smoothMOE)
        {
            curMapRoot.transform.position = Vector3.SmoothDamp(curMapRoot.transform.position, new Vector3(-mapOffset, 0), ref v0, smoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
            nextMapRoot.transform.position = Vector3.SmoothDamp(nextMapRoot.transform.position, Vector3.zero, ref v1, smoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
            d0 = curMapRoot.transform.position - new Vector3(-mapOffset, 0);
            d1 = nextMapRoot.transform.position - Vector3.zero;
            yield return null;
        }
        nextMapRoot.transform.position = Vector3.zero;
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        Time.timeScale = 1;
        _coroutine = null;
    }
}