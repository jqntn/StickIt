using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject[] mapsChair;
    public GameObject currentMapInstance;
    
    public enum TypeMods
    {
        CHAIR
    }
    public TypeMods currentMod;
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        

    }

    public void ChangeMod()
    {
        StartCoroutine(EndLevel());
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

    IEnumerator EndLevel()
    {
        yield return new WaitForSeconds(1.5f);
        Time.timeScale = 0.2f;
        yield return new WaitForSeconds(1);
        Time.timeScale = 1f;
        //InitMod(currentMod);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

}
