using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Level : MonoBehaviour
{
    // Start is called before the first frame update

    public List<Player> winners;
    public Transform startingPos;

    void Start()
    {
        Init();
    }

    public virtual void Init()
    {
    }

    public virtual void EndMap()
    {
        Debug.Log("End Map");
    }
}
