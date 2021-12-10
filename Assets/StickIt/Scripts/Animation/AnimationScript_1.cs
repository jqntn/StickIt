using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class AnimationScript_1 : MonoBehaviour
{
    [Header("ANIMATIONS_______________________")]
    public bool randTimeStart = true;
    [Range(0.0f, 1.0f)]
    public float minTime = 0.0f;
    [Range(0.1f, 2.0f)]
    public float maxTime = 1.0f;
    public float timeBeforeStart = 1.0f;
    [Header("DEBUG____________________________")]
    private Animation anim;
    private float timer;
    private void Awake()
    {
        anim = GetComponent<Animation>();
        if (randTimeStart)
        {
            timeBeforeStart = Random.Range(minTime, maxTime);
        }

    }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        timer = 0.0f;
        while(timer < timeBeforeStart)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        anim.Play();
    }
}
