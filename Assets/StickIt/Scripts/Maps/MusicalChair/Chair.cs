using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : MonoBehaviour
{
    MusicalChairManager musicalChairManager;
    public bool isActive;
    public bool isTaken;
    public List<Player> playersInChair;
    public Player chosenOne;
    Color activatedColor;
    Color deactivatedColor;
    // Start is called before the first frame update
    void Start()
    {
        musicalChairManager = FindObjectOfType<MusicalChairManager>();
    }

    // Update is called once per frame
    void Update()
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
        else if(!isTaken && isActive)
        {
            gameObject.GetComponent<MeshRenderer>().material.color = activatedColor;
        }else if(!isTaken && !isActive)
        {
            gameObject.GetComponent<MeshRenderer>().material.color = deactivatedColor;
        }
    }

    public void ActivateChair(Color c)
    {
        activatedColor = c;
        //FAIRE TREMBLER LA CHAISE
        isActive = true;
        gameObject.GetComponent<MeshRenderer>().material.color = c;
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
}
