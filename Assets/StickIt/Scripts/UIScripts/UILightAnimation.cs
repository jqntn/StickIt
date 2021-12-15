using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class UILightAnimation : MonoBehaviour
{
    public float minIntensity = 5.0f;
    public float maxIntensity = 15.0f;
    public float animationTime = 2.0f;
    public AnimationCurve curve = new AnimationCurve();
    public bool hasRandomStartTime = true;
    public float minStartTime = 0.0f;
    public float maxStartTime = 2.0f;
    public float timeBeforeStart = 2.0f;
    
    [Header("DEBUG_____________________")]
    [SerializeField] private Light myLight;
    [SerializeField] private float timer = .0f;
    [SerializeField] private float startTimer = .0f;
    [SerializeField] private bool hasReverse = false;
    [SerializeField] private bool hasStart = false;

    private void Awake()
    {
        myLight = GetComponent<Light>();
        if(hasRandomStartTime) {
            timeBeforeStart = Random.Range(minStartTime, maxStartTime);
        }

        myLight.intensity = minIntensity;
    }
    private void Update()
    {
        if(startTimer < timeBeforeStart && !hasStart)
        {
            startTimer += Time.deltaTime;
            return;
        }

        hasStart = true;

        if(timer < (animationTime / 2f))
        {
            timer += Time.deltaTime;
            float ratio = timer / (animationTime / 2.0f);

            if (hasReverse)
            {
                myLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, curve.Evaluate(ratio));
            }
            else {
                myLight.intensity = Mathf.Lerp(maxIntensity, minIntensity, curve.Evaluate(ratio));
            }
            return;
        }

        hasReverse = !hasReverse;
        timer = 0.0f;
    }
}
