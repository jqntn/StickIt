using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxSettings : MonoBehaviour
{
    public GameObject[] ParallaxElements;
    public int parallaxMultiplier;
    public int posZBegin;
    public void ChangePosZ()
    {
        for (int i = 0; i < ParallaxElements.Length; i++)
        {
            ParallaxElements[i].transform.position = new Vector3(ParallaxElements[i].transform.position.x, ParallaxElements[i].transform.position.y, posZBegin * (Mathf.Pow(parallaxMultiplier, i)));
        }
    }
}
