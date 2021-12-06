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
    [SerializeField]
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
    [SerializeField]
    public GameObject shield;
    [Header("ChosenOne")]
    LineRenderer lr;
    [SerializeField]
    Transform lrBeginPos;
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();  
        lr.SetPosition(0, lrBeginPos.position + transform.forward * offsetSpawn);
        lr.enabled = false;
        musicalChairManager = FindObjectOfType<MusicalChairManager>();
        duration = musicalChairManager.durationSpawn;
        spawnPosition = transform.position + transform.forward * offsetSpawn;
        originalPos = transform.position;
        shield.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        TestPlayer();
    }
    void TestPlayer()
    {
        if (isTaken)
        {
            for (int i = 1; i < playersInChair.Count; i++)
            {
                if (playersInChair.Count > 1 && Vector3.Distance(chosenOne.transform.position, transform.position) > Vector3.Distance(playersInChair[i].transform.position, transform.position))
                {
                    chosenOne = playersInChair[i];

                }
            }
            if (chosenOne)
            {
                shield.transform.SetParent(chosenOne.transform);
                shield.transform.localPosition = new Vector3(0, 0, 0);
                lr.SetPosition(1, chosenOne.transform.position);
            }
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
        gameObject.GetComponent<MeshRenderer>().material.color = c;
        //FAIRE TREMBLER LA CHAISE A L'INVERSE
        if (isTaken)
            musicalChairManager.winners.Add(chosenOne);
        isActive = false;
        lr.enabled = false;
        chosenOne = null;
        playersInChair.Clear();
        DespawnChair();
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
        GameEvents.CameraShake_CEvent?.Invoke(duration / 0.4f);
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
        GameEvents.CameraShake_CEvent?.Invoke(duration / 0.4f);
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

        shield.transform.SetParent(transform);
        shield.SetActive(false);
    }
    private void OnTriggerEnter(Collider c)
    {
        if (isActive)
        {
            if (c.tag == "Player")
            {
                playersInChair.Add(c.gameObject.GetComponentInParent<Player>());
                if(playersInChair.Count == 1)
                {
                    isTaken = true;
                    lr.enabled = true;
                    gameObject.GetComponent<MeshRenderer>().material.color = musicalChairManager.colorChairTaken;
                    chosenOne = playersInChair[0];
                    shield.SetActive(true);
                }
            }
        }
    }
    private void OnTriggerExit(Collider c)
    {
        if (isActive)
        {
            if (c.tag == "Player")
            {
                playersInChair.Remove(playersInChair.Find(x => c.gameObject.GetComponentInParent<Player>()));
                if (playersInChair.Count < 1)
                {
                    isTaken = false;
                    lr.enabled = false;
                    gameObject.GetComponent<MeshRenderer>().material.color = activatedColor;
                    shield.transform.SetParent(transform);
                    shield.SetActive(false);
                }
            }
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawMesh(GetComponent<MeshFilter>().sharedMesh,transform.position + transform.forward * offsetSpawn, transform.rotation, transform.localScale);
        Gizmos.DrawCube(lrBeginPos.position, new Vector3(0.5f, 0.5f, 0.5f));
    }
}
