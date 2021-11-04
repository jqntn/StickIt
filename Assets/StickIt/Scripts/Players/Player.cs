using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
public class Player : MonoBehaviour
{
    private MultiplayerManager _multiplayerManager;
    public PlayerMouvement myMouvementScript;
    public MultiplayerManager.PlayerData myDatas;
    public MMFeedbacks deathAnim;
    public GameObject deathPart;
    public bool isDead;
    void Start()
    {
        _multiplayerManager = MultiplayerManager.instance;
        if (TryGetComponent<PlayerMouvement>(out PlayerMouvement pm))
        {
            myMouvementScript = pm;
            myMouvementScript.myPlayer = this;
        }
        DontDestroyOnLoad(this);
    }
    public void Death(bool intensityAnim = false)
    {
        isDead = true;
        myMouvementScript.enabled = false;
        _multiplayerManager.alivePlayers.Remove(this);
        _multiplayerManager.deadPlayers.Add(this);
        // Play Death Animation
        StartCoroutine(OnDeath(intensityAnim));
    }

    IEnumerator OnDeath()
    {
        deathAnim.PlayFeedbacks();
        yield return new WaitForSeconds(deathAnim.TotalDuration);
        myMouvementScript.Death();
        GameObject temp = Instantiate(deathPart, new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z), transform.rotation);
        temp.GetComponent<ParticleSystemRenderer>().material = myDatas.material;
        GameEvents.CameraShake_CEvent?.Invoke();
        MapManager.instance.EndLevel();
    }

    public void PrepareToChangeLevel() // When the player is still alive
    {
        if (!isDead)
        {
            myMouvementScript.enabled = false;
            foreach (Collider col in GetComponentsInChildren<Collider>())
            {
                col.enabled = false;
            }
            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            {
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                rb.isKinematic = true;
            }
        }
    }
   
    public void Respawn()
    {
        myMouvementScript.enabled = true;
        myMouvementScript.Respawn();
        isDead = false;                      
    }
    public void QuitGame()
    {
        Debug.Log("Quit Application");
        Application.Quit();
    }
}