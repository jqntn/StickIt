using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Level : MonoBehaviour
{
    public List<Player> winners;
    public GameObject canvasStartMap;
    public IEnumerator Init()
    {
        if (canvasStartMap != null)
        {
            canvasStartMap.SetActive(true);
            yield return new WaitForSeconds(4.5f);
        }
        else
        {
            Debug.LogWarning("You need to set the CanvasStartMap ! It should be included in the map prefab. \n " +
            "If not : Prefabs > UI > CanvasStartMap. (Modify the title and description if needed)");
        }
        StartMap();
        yield return null;
    }
    public virtual void StartMap()
    {
    }
    public virtual void EndMap()
    { }
}