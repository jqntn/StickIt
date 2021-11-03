using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class CameraState : MonoBehaviour
{
    [Header("------- Data -------")]
    public CameraType type = CameraType.BARYCENTER;

    [Header("------- Move -------")]
    public float moveTime = 0.2f;

    [Header("------- Zoom -------")]
    public float maxOut_Z = -110.0f;
    public float maxIn_Z = -70.0f;
    public float zoomOutMargin = -5.0f;
    public float zoomInMargin = -10.0f;
    public float zoomOutValue = 20.0f;
    public float zoomInValue = 10.0f;
    public float zoomTime = 0.2f;

    [Header("----- End Animation -----")]
    public bool hasZoomOutAtEnd = true;
    public bool hasCenterCamera = true;
    //public bool hasFreeRoaming = false;
    //public int randomRadius = 5;
    //public float roamingTime = 3.0f;

    [Header("----- Prototype -----")]
    public List<CameraData> datas = new List<CameraData>();

    [Header("----- Debug -----")]
    [SerializeField] private Vector3 moveVelocity = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField] private MapManager mapManager = null;
    [SerializeField] private MultiplayerManager multiplayerManager = null;
    [SerializeField] private List<Player> playerList = new List<Player>();

    protected void Start()
    {
        mapManager = MapManager.instance;
        multiplayerManager = MultiplayerManager.instance;
        if(multiplayerManager != null)
        {
            playerList = multiplayerManager.players;
        }
    }

    protected virtual void Update()
    {
        // Protections
        if (SceneManager.GetActiveScene().name == "0_MenuSelection" || 
            SceneManager.GetActiveScene().buildIndex == 0) {
            return; 
        }

        if (mapManager.isBusy) { return; }
    }

    protected void LateUpdate()
    {
        
    }

    private bool VerifyAllDead()
    {
        bool allDead = true;
        foreach (Player player in playerList)
        {
            if (!player.isDead)
            {
                allDead = false;
                break;
            }
        }

        return allDead;
    }

    public CameraType GetCameraType()
    {
        return type;
    }
}
