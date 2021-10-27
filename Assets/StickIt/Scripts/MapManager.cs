using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
class MapManager : Unique<MapManager>
{
    [Range(0, 1)]
    public float smoothTime;
    public float smoothApprox;
    public int mapOffset;
    public string nextMap;
    Coroutine _coroutine;
    Vector3 _velocity0,
    _velocity1;
    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 200, 100), "NextMap") && _coroutine == null) _coroutine = StartCoroutine(NextMap(nextMap));
    }
    IEnumerator NextMap(string nextMapName)
    {
        Time.timeScale = 0;
        GameObject curMapRoot = GameObject.Find("MapRoot");
        GameObject nextMapRoot = null;
        Vector3 diff0 = Vector3.one, diff1 = Vector3.one;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(nextMapName, LoadSceneMode.Additive);
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
        foreach (var i in SceneManager.GetSceneByName(nextMapName).GetRootGameObjects())
            if (i.name == "MapRoot") { nextMapRoot = i; nextMapRoot.transform.position = new Vector3(mapOffset, 0); break; }
        while (diff0.sqrMagnitude > smoothApprox && diff1.sqrMagnitude > smoothApprox)
        {
            curMapRoot.transform.position = Vector3.SmoothDamp(curMapRoot.transform.position, new Vector3(-mapOffset, 0), ref _velocity0, smoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
            nextMapRoot.transform.position = Vector3.SmoothDamp(nextMapRoot.transform.position, Vector3.zero, ref _velocity1, smoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
            diff0 = curMapRoot.transform.position - new Vector3(-mapOffset, 0);
            diff1 = nextMapRoot.transform.position - Vector3.zero;
            yield return null;
        }
        nextMapRoot.transform.position = Vector3.zero;
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        _coroutine = null;
        Time.timeScale = 1;
    }
}