using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Chair : MonoBehaviour
{
    private MusicalChairManager musicalChairManager;
    [Header("Player")]
    public List<Player> playersInChair;
    public Player chosenOne;
    [Header("Settings")]
    [SerializeField]
    public float offsetSpawn;
    private float duration;
    public AnimationCurve animCurve;
    [HideInInspector]
    public bool isActive;
    [HideInInspector]
    public bool isTaken;
    private Vector3 spawnPosition;
    [SerializeField]
    public GameObject shield;
    MeshRenderer shieldMesh;
    [Header("ChosenOne")]
    private LineRenderer lr;
    Color colShield;
    [SerializeField]
    private Transform lrBeginPos;
    private MeshRenderer myMeshRenderer;
    Material matChair;
    // Start is called before the first frame update
    private void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.SetPosition(0, lrBeginPos.position + transform.forward * offsetSpawn);
        lr.enabled = false;
        myMeshRenderer = GetComponent<MeshRenderer>();
        musicalChairManager = FindObjectOfType<MusicalChairManager>();
        duration = musicalChairManager.durationSpawn;
        spawnPosition = transform.position + transform.forward * offsetSpawn;
        shieldMesh = shield.GetComponent<MeshRenderer>();
        matChair = myMeshRenderer.material;
        shield.SetActive(false);
        transform.position = spawnPosition;
    }
    // Update is called once per frame
    private void Update()
    {
        TestPlayer();
    }
    private void TestPlayer()
    {
        if (isTaken)
        {
            for (int i = 1; i < playersInChair.Count; i++)
            {
                if (playersInChair.Count > 1 && Vector3.Distance(chosenOne.transform.position, transform.position) > Vector3.Distance(playersInChair[i].transform.position, transform.position))
                {
                    chosenOne = playersInChair[i];
                    AudioManager.instance.PlayGainShieldSounds(gameObject);
                }
            }
            if (chosenOne)
            {
                shield.transform.SetParent(chosenOne.transform);
                shield.transform.localScale =  new Vector3(musicalChairManager.sizeShieldChair, musicalChairManager.sizeShieldChair, musicalChairManager.sizeShieldChair) / 100;
                shield.transform.localPosition = new Vector3(0, 0, 0);
                colShield = chosenOne.GetComponent<Player>().myDatas.material.GetColor("_Color");
                colShield.a = shieldMesh.material.GetColor("_Tint").a;
                shieldMesh.material.SetColor("_Tint", colShield);
                lr.SetPosition(1, chosenOne.transform.position);
            }
        }
    }
    public void ActivateChair(float duration)
    {
        isActive = true;
        StartCoroutine(SpawnChairCor(duration));
    }
    private IEnumerator SpawnChairCor(float duration)
    {
        float elapsed = 0;
        float ratio;
        while (elapsed < duration)
        {
            ratio = elapsed / duration;
            ratio = animCurve.Evaluate(ratio);
            matChair.SetFloat("_Fade", 1 - ratio);
            //transform.position = Vector3.Lerp(originalPos, spawnPosition, ratio);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    public void DeactivateChair(float duration)
    {
        if (isTaken)
            musicalChairManager.winners.Add(chosenOne);
        isActive = false;
        lr.enabled = false;
        chosenOne = null;
        playersInChair.Clear();
        StartCoroutine(DespawnChairCor(duration));
    }
    private IEnumerator DespawnChairCor(float duration)
    {
        float elapsed = 0;
        float ratio;
        while (elapsed < duration)
        {

            ratio = elapsed / duration;
            ratio = animCurve.Evaluate(ratio);
            matChair.SetFloat("_Fade", ratio);
            elapsed += Time.deltaTime;
            yield return null;
        }
        DeactivateShield();
    }
    public void DeactivateShield()
    {
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
                if (playersInChair.Count == 1)
                {
                    // if (!isSpawnAnimation)
                    //    matChair.SetFloat("_Intensity", musicalChairManager.intensityEmissive);
                    //myMeshRenderer.material = musicalChairManager.chairTaken;
                    chosenOne = playersInChair[0];
                    shield.SetActive(true);
                    if (AudioManager.instance != null)
                    {
                        AudioManager.instance.PlayGainShieldSounds(gameObject);
                    }
                }
                if(playersInChair.Count >= 1)
                {
                    isTaken = true;
                    lr.enabled = true;
                    matChair.SetFloat("_Intensity", musicalChairManager.intensityEmissive);
                }
            }
        }
    }
    private void OnTriggerExit(Collider c)
    {
        if (c.tag == "Player")
        {
            playersInChair.Remove(playersInChair.Find(x => c.gameObject.GetComponentInParent<Player>()));
            if (playersInChair.Count < 1)
            {
                isTaken = false;
                lr.enabled = false;
                matChair.SetFloat("_Intensity", 0.5f);
                //if (!isSpawnAnimation)
                //myMeshRenderer.material = musicalChairManager.chairNotTaken;
                shield.transform.SetParent(transform);
                shield.SetActive(false);
            }
        }
        if (isActive)
        {
            
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawMesh(GetComponent<MeshFilter>().sharedMesh, transform.position + transform.forward * offsetSpawn, transform.rotation, transform.localScale);
        Gizmos.DrawCube(lrBeginPos.position, new Vector3(0.5f, 0.5f, 0.5f));
    }
}