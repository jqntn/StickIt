using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeShader : MonoBehaviour
{

    List<Renderer> renderers = new List<Renderer>();
    List<Material> matSave = new List<Material>();
    public Shader shader;

    public float speedFade = 1;

    public float edgeWidth = 0.01f;
    public int noiseSize = 30;
    public float speedRotation = 0f;


    bool isAppearAnimation;
    bool isDisappearAnimation;

    float time;


    private void Start()
    {

        StartCoroutine(test());
    }
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            if (isAppearAnimation)
            {
                float t = (Time.time - time) * speedFade;
                renderers[i].material.SetFloat("_Fade", 1 - t);
                if (t >= 1)
                {
                    isAppearAnimation = false;
                    SetBackMaterials();
                }
            }
            else if (isDisappearAnimation)
            {
                float t = (Time.time - time) * speedFade;
                renderers[i].material.SetFloat("_Fade", t);
                if (t >= 1) isDisappearAnimation = false;
            }
        }
        
    }


    public void AllObjectsDisappear()
    {
        SetShaders();
        isDisappearAnimation = true;
        time = Time.time;
    }

    public void AllObjectsAppear()
    {
        isAppearAnimation = true;
        time = Time.time;
    }

    Material CreateShaderFromMaterial(Material material)
    {
        Material mat = new Material(shader);
        mat.name = "Shader Disappear";
        mat.SetTexture("_BaseMap", material.GetTexture("_BaseMap"));
        mat.SetTexture("_MetallicMap", material.GetTexture("_MetallicGlossMap"));
        mat.SetTexture("_NormalMap", material.GetTexture("_BumpMap"));
        mat.SetTexture("_OcclusionMap", material.GetTexture("_OcclusionMap"));
        mat.SetTexture("_EmissionMap", material.GetTexture("_EmissionMap"));
        mat.SetColor("_Color", material.GetColor("_BaseColor"));
        mat.SetColor("_EmissionColor", material.GetColor("_EmissionColor"));
        mat.SetFloat("_EdgeWidth", edgeWidth);
        mat.SetFloat("_SpeedRotation", speedRotation);
        mat.SetInt("_NoiseSize", noiseSize);
        mat.SetFloat("_Fade", 1);
        return mat;
    }

    private void SetBackMaterials()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            renderers[i].material = matSave[i];
        }
    }

    public void SetShaders()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Renderer rend = transform.GetChild(i).GetComponent<Renderer>();
            if (rend == null)
            {
                rend = transform.GetChild(i).GetChild(0).GetComponent<Renderer>();
                print(rend);
            }
            renderers.Add(rend);
            matSave.Add(renderers[i].material);
            renderers[i].material = CreateShaderFromMaterial(renderers[i].material);
        }
    }


    IEnumerator test()
    {
        SetShaders();
        yield return new WaitForSeconds(2);
        AllObjectsAppear();
        yield return new WaitForSeconds(5);
        AllObjectsDisappear();
    }
}
