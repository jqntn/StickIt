using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeShader : MonoBehaviour
{

    public MeshRenderer renderer;

    // Update is called once per frame
    void Update()
    {
        renderer.sharedMaterial.SetFloat("_Fade", Mathf.PingPong(Time.time, 1));
    }
}
