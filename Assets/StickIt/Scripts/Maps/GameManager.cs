using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        players = GameObject.FindGameObjectsWithTag("Player");
    }
    // Start is called before the first frame update
    void Start()
    {
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

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 50, 50), "Reload Scene"))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
