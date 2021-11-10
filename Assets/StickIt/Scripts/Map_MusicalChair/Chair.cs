using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class Chair : MonoBehaviour
{
    MusicalChairManager musicalChairManager;
    [Header("Player")]
    public List<Player> playersInChair;
    public Player chosenOne;
    [Header("Settings")]
    public float offsetSpawn;
    float duration;
    public AnimationCurve animCurve;
    [HideInInspector]
    public bool isActive;
    [HideInInspector]
    public bool isTaken;
    Color activatedColor;
    Color deactivatedColor;
    Vector3 spawnPosition;
    Vector3 originalPos;
    // Start is called before the first frame update
    void Start()
    {
        musicalChairManager = FindObjectOfType<MusicalChairManager>();
        duration = musicalChairManager.durationSpawn;
        spawnPosition = transform.position + transform.up * offsetSpawn;
        originalPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        TestPlayer();
    }
    void TestPlayer()
    {
        if (isActive)
        {
            if (playersInChair.Count > 0)
            {
                isTaken = true;
            }
            else
            {
                isTaken = false;
            }
        }
        if (isTaken)
        {
            gameObject.GetComponent<MeshRenderer>().material.color = musicalChairManager.colorChairTaken;
        }
        else if (!isTaken && isActive)
        {
            gameObject.GetComponent<MeshRenderer>().material.color = activatedColor;
        }
        else if (!isTaken && !isActive)
        {
            gameObject.GetComponent<MeshRenderer>().material.color = deactivatedColor;
        }
    }
    public void ActivateChair(Color c)
    {
        activatedColor = c;
        SpawnChair(c);
        isActive = true;
    }
    public void DeactivateChair(Color c)
    {
        deactivatedColor = c;
        //FAIRE TREMBLER LA CHAISE A L'INVERSE
        if (isActive)
        {
            if (playersInChair.Count > 1)
            {
                chosenOne = playersInChair[0];
                for (int i = 0; i < playersInChair.Count; i++)
                {
                    if (Vector3.Distance(chosenOne.transform.position, transform.position) > Vector3.Distance(playersInChair[i].transform.position, transform.position))
                    {
                        chosenOne = playersInChair[i];
                    }
                }
            }else if (playersInChair.Count == 1)
            {
                chosenOne = playersInChair[0];
            }
            gameObject.GetComponent<MeshRenderer>().material.color = c;
            if (isTaken)
                musicalChairManager.winners.Add(chosenOne);
            isActive = false;
            DespawnChair();
        }
    }
    void SpawnChair(Color c) 
    {
        StartCoroutine(SpawnChairCor(c));
    }
    void DespawnChair()
    {
        StartCoroutine(DespawnChairCor());
    }
    IEnumerator SpawnChairCor(Color c)
    {
        GameEvents.CameraShake_CEvent?.Invoke(duration);
        float elapsed = 0;
        float ratio = 0;
        while(elapsed < duration)
        {
            ratio = elapsed / duration;
            ratio = animCurve.Evaluate(ratio);
            transform.position = Vector3.Lerp(originalPos, spawnPosition, ratio);
            elapsed += Time.deltaTime;
            yield return null;
        }
        gameObject.GetComponent<MeshRenderer>().material.color = c;
    }
    IEnumerator DespawnChairCor()
    {
        GameEvents.CameraShake_CEvent?.Invoke(duration);
        float elapsed = 0;
        float ratio = 0;
        while (elapsed < duration)
        {
            ratio = elapsed / duration;
            ratio = animCurve.Evaluate(ratio);
            transform.position = Vector3.Lerp(spawnPosition, originalPos, ratio);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    private void OnTriggerEnter(Collider c)
    {
        if (isActive)
        {
            if (c.tag == "Player")
            {
                playersInChair.Add(c.gameObject.GetComponentInParent<Player>());
            }
        }
    }
    private void OnTriggerExit(Collider c)
    {
        if (c.tag == "Player")
        {
            playersInChair.Remove(playersInChair.Find(x => c.gameObject.GetComponentInParent<Player>()));
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position + transform.up * offsetSpawn, new Vector3(1.8762f, 1.8762f, 1.8762f));
    }
}
