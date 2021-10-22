using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
class MapManager : Unique<MapManager>
{
    [Range(0, 1)]
    public float smoothTime;
    Vector3 _velocity0,
    _velocity1;
    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 50), "NextMap")) StartCoroutine(NextMap("2_Map"));
    }
    IEnumerator NextMap(string nextMapName)
    {
        GameObject curMapRoot = GameObject.Find("MapRoot");
        GameObject nextMapRoot = null;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(nextMapName, LoadSceneMode.Additive);
        while (!asyncOperation.isDone)
        {
            // Do SlowMo on current map here
            yield return null;
        }
        foreach (var i in SceneManager.GetSceneByName(nextMapName).GetRootGameObjects())
            if (i.name == "MapRoot") { nextMapRoot = i; nextMapRoot.transform.position = new Vector3(50, 0); break; }
        // Do transition to new map from here
        // curMapRoot.transform.position = Vector3.SmoothDamp(curMapRoot.transform.position, new Vector3(-50, 0), ref _velocity0, smoothTime);
        // nextMapRoot.transform.position = Vector3.SmoothDamp(curMapRoot.transform.position, Vector3.zero, ref _velocity1, smoothTime);
        curMapRoot.transform.position = new Vector3(-50, 0);
        nextMapRoot.transform.position = Vector3.zero;
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
    }
}