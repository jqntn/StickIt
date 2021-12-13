using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeShader : MonoBehaviour
{

    public MeshRenderer renderer;

    public float speedFade;
    bool isAppearAnimation;
    bool isDisappearAnimation;

    float time;


    private void Start()
    {
        renderer.sharedMaterial.SetFloat("_Fade", 1);
    }
    // Update is called once per frame
    void Update()
    {
        if (isAppearAnimation)
        {
            float t = Time.time - time * speedFade;
            renderer.sharedMaterial.SetFloat("_Fade", 1 - t);
            if (t >= 1) isAppearAnimation = false;
        } 
        else if (isDisappearAnimation)
        {
            float t = Time.time - time * speedFade;
            renderer.sharedMaterial.SetFloat("_Fade", t);
            if (t >= 1) isDisappearAnimation = false;
        }
        
    }


    public void AllObjectsDisappear()
    {
        isDisappearAnimation = true;
        time = Time.time;
    }

    public void AllObjectsAppear()
    {
        isAppearAnimation = true;
        time = Time.time;
    }

}
