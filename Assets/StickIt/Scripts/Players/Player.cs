using System.Collections;
using UnityEngine;
using MoreMountains.Feedbacks;

public class Player : MonoBehaviour
{
    private MultiplayerManager _multiplayerManager;
    public PlayerMouvement myMouvementScript;
    public P_Mouvement2 myMouvementScript2;
    public PlayerData myDatas;
    public MMFeedbacks deathAnim;
    public GameObject deathPart;
    public bool isDead;
    void Start()
    {
        _multiplayerManager = MultiplayerManager.instance;
        myMouvementScript = GetComponent<PlayerMouvement>();
        myMouvementScript.myPlayer = this;

        DontDestroyOnLoad(this);
    }

    public void Death()
    {
        isDead = true;
        myMouvementScript.enabled = false;
        _multiplayerManager.alivePlayers.Remove(this);
        _multiplayerManager.deadPlayers.Add(this);

        // Play Death Animation
        StartCoroutine(OnDeath());
    }

    public void PrepareToChangeLevel()
    {
        if (!isDead)
        {
            myMouvementScript.PrepareToChangeLevel();
            myMouvementScript.enabled = false;
            GetComponentInChildren<Collider>().enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }
    IEnumerator OnDeath()
    {
        deathAnim.PlayFeedbacks();
        yield return new WaitForSeconds(deathAnim.TotalDuration);
        GetComponentInChildren<MeshRenderer>().enabled = false;
        GetComponentInChildren<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GameObject temp = Instantiate(deathPart, new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z), transform.rotation);
        temp.GetComponent<ParticleSystemRenderer>().material = myDatas.material;
        yield return null;
        GameEvents.CameraShake_CEvent?.Invoke();
        
    }
    public void Respawn()
    {
        myMouvementScript.enabled = true;
        GetComponentInChildren<MeshRenderer>().enabled = true;
        GetComponentInChildren<Collider>().enabled = true;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;


    }

    public void QuitGame()
    {
        Debug.Log("Quit Application");
        Application.Quit();
    }
}

[System.Serializable]
public struct PlayerData
{
    public string name;
    public int id;
    public int deviceID;
    public Material material;
    public int mass;

    public uint score;
    public uint nbrDeath;
    public uint nbrVictories;
    public PlayerData(string _name, int _id, int _deviceID, Material _material)
    {
        name = _name;
        id = _id;
        deviceID = _deviceID;
        material = _material;
        mass = 100;
        score = 0;
        nbrDeath = 0;
        nbrVictories = 0;
    }
}
