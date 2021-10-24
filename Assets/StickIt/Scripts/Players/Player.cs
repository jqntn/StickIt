using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class Player : MonoBehaviour
{
    private MultiplayerManager _multiplayerManager;
    public PlayerMouvement myMouvementScript;
    public MultiplayerManager.PlayerData myDatas;
    public MMFeedbacks deathAnim;
    public GameObject deathPart;
    public int id;
    bool isDead;
    void Start()
    {
        _multiplayerManager = MultiplayerManager.instance;
        myMouvementScript = GetComponent<PlayerMouvement>();
        myMouvementScript.myPlayer = this;
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
        _multiplayerManager.alivePlayers.Add(this);
        _multiplayerManager.deadPlayers.Remove(this);
        myMouvementScript.enabled = true;
        GetComponentInChildren<MeshRenderer>().enabled = true;
        GetComponentInChildren<Collider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;

    }

    public void QuitGame()
    {
        Debug.Log("Quit Application");
        Application.Quit();
    }
}
