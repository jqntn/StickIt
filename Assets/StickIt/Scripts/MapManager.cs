using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;
public class MapManager : Unique<MapManager>
{
    [Header("DATAS____________________")]
    public float numberOfRounds = 2.0f;
    [Range(0, 1)]
    public float smoothTime;
    public float slowTime;
    public float smoothMOE;
    public int mapOffset;
    public float timeScale;
    public bool isBusy;

    //[Header("PREFABS___________________")]
    //public VisualEffect onDeathVFX;                                             // Take Player death Animation to know how long to wait before transition

    [Header("DEBUG____________________")]
    public GameObject curMapRoot;
    public GameObject nextMapRoot;
    public ModsData modsData;
    public string nextMapManual;
    public string prevMod;
    public string prevMap;
    public string curMod;
    public string curMap = "";
    [SerializeField] private uint roundCount = 0;
    private Coroutine _coroutine;
    private CameraStateDriven camManager;
    bool levelEnded = false;
    private FadeShader shaderScript;
    //void OnGUI()
    //{
    //    if (GUI.Button(new Rect(0, 0, 200, 100), "NextMap")) NextMap(nextMapManual, true);
    //}
    protected override void Awake()
    {
        base.Awake();
        camManager = Camera.main.GetComponent<CameraStateDriven>();
        shaderScript = GetComponent<FadeShader>();
    }
    private void Start()
    {
        if(AudioManager.instance != null) {
            AudioManager.instance.PlayAmbiantSounds(gameObject);
        }

    }
    public bool EndLevel()
    {
        if (!levelEnded && MultiplayerManager.instance.alivePlayers.Count <= 1)
        {
            shaderScript.AllObjectsDisappear();
            NextMap();
            levelEnded = true;
            return true;
        }
        return false;
    }
    public bool NextMap(string nextMap = "", bool fromMenu = false)
    {
        //if (fromMenu)
        //{
        //    nextMap = SelectNextMap();
        //    SceneManager.LoadScene(nextMap);
        //    camManager.SwitchStates(Utils.GetCameraType(curMod));
        //    return true;
        //}
        foreach (Player player in MultiplayerManager.instance.players) player.PrepareToChangeLevel();
        if (_coroutine == null) _coroutine = StartCoroutine(BeginTransition(nextMap, fromMenu));
        else return false;
        return true;
    }
    private string SelectNextMap()
    {
        // End Game
        if (roundCount == numberOfRounds)
        {
            curMod = "End";
            curMap = "100_EndScene";
            return "100_EndScene";
        }
        // Next Map
        ModsData.Mod mod;
        string map = SceneManager.GetActiveScene().name;
        if (modsData.mods.Count == 0) return map;
        if (modsData.mods.Count > 1)
            do mod = modsData.mods[Random.Range(0, modsData.mods.Count)];
            while (mod.name == curMod);
        else mod = modsData.mods[0];
        if (mod.maps.Count == 0) return map;
        if (mod.maps.Count > 1)
            do map = mod.maps[Random.Range(0, mod.maps.Count)];
            while (map == curMap);
        else map = mod.maps[0];
        prevMod = curMod;
        prevMap = curMap;
        curMod = mod.name;
        curMap = map;
        roundCount++;
        return map;
    }
    private IEnumerator BeginTransition(string nextMap, bool fromMenu)
    {
        isBusy = true;
        if (nextMap == "") nextMap = SelectNextMap();

        // Wait for animation death player to end
        foreach (Player player in MultiplayerManager.instance.players)
        {
            PlayerAnimations snowAnim = player.GetComponent<PlayerAnimations>();
            if (snowAnim != null)
            {
                snowAnim.VFXSnow.Stop();
                snowAnim.ChangeBoolSnowToFalse();
            }
        }
        yield return new WaitForSecondsRealtime(2.0f);
        //--------

        Time.timeScale = .5f;
        timeScale = Time.timeScale;
        if (curMapRoot == null) curMapRoot = GameObject.Find("MapRoot");
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(nextMap, LoadSceneMode.Additive);
        asyncOp.allowSceneActivation = false;
        float timeToLoad = 0;
        while (!asyncOp.isDone)
        {
            timeToLoad += Time.unscaledDeltaTime;
            if (asyncOp.progress >= .9f)
            {
                if (!fromMenu && timeToLoad < slowTime) yield return new WaitForSecondsRealtime(slowTime - timeToLoad);
                asyncOp.allowSceneActivation = true;
            }
            yield return null;
        }
        Time.timeScale = 0;
        timeScale = Time.timeScale;
        GameObject[] objs = GameObject.FindGameObjectsWithTag("MapRoot");
        nextMapRoot = objs[objs.Length - 1];
        nextMapRoot.transform.position = new Vector3(mapOffset, 0);
        // CAMERA EVENT
        GameEvents.OnSceneUnloaded.Invoke();
        // ----------
        GameObject[] nextStartPos = GameObject.FindGameObjectsWithTag("StartPos");
        MultiplayerManager.instance.speedChangeMap = 1 / slowTime;
        MultiplayerManager.instance.StartChangeMap(nextStartPos[nextStartPos.Length - 1].transform);
        while (MultiplayerManager.instance.isChangingMap) yield return null;
        StartCoroutine(EndTransition());
    }
    public IEnumerator EndTransition()
    {
      
        //Vector3 v0 = Vector3.zero, v1 = Vector3.zero, d0 = Vector3.one, d1 = Vector3.one;
   
        //while (d0.sqrMagnitude > smoothMOE && d1.sqrMagnitude > smoothMOE)
        //{
        //    curMapRoot.transform.position = Vector3.SmoothDamp(curMapRoot.transform.position, new Vector3(-mapOffset, 0), ref v0, smoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
        //    nextMapRoot.transform.position = Vector3.SmoothDamp(nextMapRoot.transform.position, Vector3.zero, ref v1, smoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
        //    d0 = curMapRoot.transform.position - new Vector3(-mapOffset, 0);
        //    d1 = nextMapRoot.transform.position - Vector3.zero;
        //    yield return null;
        //}
       
        AsyncOperation asyncOp = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

        while (!asyncOp.isDone)
        {        
             yield return null;
        }
        shaderScript.SetShaders(true);
        nextMapRoot.transform.position = Vector3.zero;
        // Switch camera state depending of map mode
        camManager.SwitchStates(Utils.GetCameraType(curMod));
        //-------
        Time.timeScale = 1;
        timeScale = Time.timeScale;
        curMapRoot = nextMapRoot;
        nextMapRoot = null;
        isBusy = false;
        _coroutine = null;
        shaderScript.AllObjectsAppear();
        var lvl = FindObjectOfType<Level>();
        if (lvl != null) StartCoroutine(lvl.Init());
        levelEnded = false;
        yield return null;
    }
}