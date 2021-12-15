using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeShader : MonoBehaviour
{
    [SerializeField]
    MeshRenderer[] renderers;
    [SerializeField]
    List<Material> matSave = new List<Material>();
    public Shader shader;

    public float speedFade = 1;

    public float edgeWidth = 0.01f;
    public int noiseSize = 30;
    public float speedRotation = 0f;


    bool isAppearAnimation;
    bool isDisappearAnimation;

    [ColorUsageAttribute(true, true), SerializeField] Color colorEdgeDisappear;
    float time;


    private void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime * speedFade;
        if (isAppearAnimation)
        {
            for (int i = 0; i < renderers.Length; i++)
            {

                //float t = (Time.time - time) * speedFade;
                if (!renderers[i].gameObject.CompareTag("Chair"))
                    renderers[i].material.SetFloat("_Fade", 1 - time);
            }
            if (time >= 1)
            {
                isAppearAnimation = false;
                SetBackMaterials();
                //print("SetBackMaterialsAppear");
            }
        }
        else if (isDisappearAnimation)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                //float t = (Time.time - time) * speedFade;
                if (!renderers[i].gameObject.CompareTag("Chair"))
                    renderers[i].material.SetFloat("_Fade", time);
            }
            if (time >= 1)
            {
                isDisappearAnimation = false;
            }
        }
        
        
    }


    public void AllObjectsDisappear()
    {
        SetShaders(false);
        isDisappearAnimation = true;
        time = 0;
        //time = Time.time;
    }

    public void AllObjectsAppear()
    {
        isAppearAnimation = true;
        time = 0;
        //time = Time.time;
    }

 

    private void SetBackMaterials()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (!renderers[i].gameObject.CompareTag("Chair"))
                renderers[i].material = matSave[i];
        }
        matSave.Clear();
    }

    public void SetShaders(bool saveMaterials)
    {
        renderers = FindObjectsOfType<MeshRenderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material != null)
            {
                if(saveMaterials)
                matSave.Add(renderers[i].material);
                if(!renderers[i].gameObject.CompareTag("Chair"))
                    renderers[i].material = CreateShaderFromMaterial(renderers[i].material, renderers[i].gameObject.name, saveMaterials);
            }
        }
    }

    Material CreateShaderFromMaterial(Material material, string name, bool isAppear)
    {
        Material mat = new Material(shader);
        mat.name = "Shader Disappear - " + name;

        Texture texture;
        if(texture = material.GetTexture("_BaseMap"))
            mat.SetTexture("_BaseMap", texture);
        if (texture = material.GetTexture("_MetallicGlossMap"))
            mat.SetTexture("_MetallicMap", texture);
        if (texture = material.GetTexture("_BumpMap"))
            mat.SetTexture("_NormalMap", texture);
        if (texture = material.GetTexture("_OcclusionMap"))
            mat.SetTexture("_OcclusionMap", texture);
        if (texture = material.GetTexture("_EmissionMap"))
            mat.SetTexture("_EmissionMap", texture);

        mat.SetColor("_Color", material.GetColor("_BaseColor"));
        mat.SetColor("_EmissionColor", material.GetColor("_EmissionColor"));
        if(!isAppear) mat.SetColor("_ColorEdge", colorEdgeDisappear);
        if (material.IsKeywordEnabled("_EMISSION")) mat.SetFloat("_Intensity", 1);
        mat.SetFloat("_EdgeWidth", edgeWidth);
        mat.SetFloat("_SpeedRotation", speedRotation);
        mat.SetInt("_NoiseSize", noiseSize);
        mat.SetFloat("_Fade", 1);
        return mat;
    }
}
