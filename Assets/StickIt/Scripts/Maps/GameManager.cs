using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    public GameObject[] players;
    public GameObject[] mapsChair;
    public GameObject currentMapInstance;
    public enum TypeMods
    {
        CHAIR
    }
    public TypeMods currentMod;
    private void Awake()
    {
        if(gameManager != null)
        {
            Destroy(this);
        }
        else
        {
            gameManager = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    public void ChangeMod()
    {
        InitMod(currentMod);
    }

    private void InitMod(TypeMods type)
    {
        switch (type)
        {
            case TypeMods.CHAIR:
                currentMapInstance = Instantiate(mapsChair[Random.Range(0, mapsChair.Length)]);
                break;
        }
    }

    public void GivePointPlayer()
    {

    }
}
