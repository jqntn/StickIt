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
        print(myDatas.deviceID);
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
    public void PrepareToChangeLevel()
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
                rb.isKinematic = true;
            }
        }
    }
    IEnumerator OnDeath(bool intensityAnim)
    {
        deathAnim.PlayFeedbacks();
        if (intensityAnim) yield return new WaitForSeconds(deathAnim.TotalDuration);
        GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        GetComponentInChildren<Collider>().enabled = false;
        GetComponentInChildren<Rigidbody>().isKinematic = true;
        GameObject temp = Instantiate(deathPart, new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z), transform.rotation);
        temp.GetComponent<ParticleSystemRenderer>().material = myDatas.material;
        GameEvents.CameraShake_CEvent?.Invoke();
        yield return new WaitForSeconds(2.0f);
        MapManager.instance.EndLevel();
    }
    public void Respawn()
    {
        myMouvementScript.enabled = true;
        myMouvementScript.Respawn();
        if (isDead)
        {
            isDead = false;
            GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
            GetComponentInChildren<Collider>().enabled = true;
            Rigidbody rb = GetComponentInChildren<Rigidbody>();
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
        else
        {
            foreach (Collider col in GetComponentsInChildren<Collider>())
            {
                col.enabled = true;
            }
            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            {
                rb.isKinematic = false;
            }
        }
    }
    public void QuitGame()
    {
        Debug.Log("Quit Application");
        Application.Quit();
    }
}