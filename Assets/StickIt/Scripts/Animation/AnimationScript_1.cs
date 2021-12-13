using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class AnimationScript_1 : MonoBehaviour
{
    [Header("ANIMATIONS_______________________")]
    public bool randTimeStart = true;
    [Range(.0f, 0.5f)]
    public float minTime = .0f;
    [Range(.1f, 0.5f)]
    public float maxTime = 1.0f;
    public float timeBeforeStart = 1.0f;
    [Range(.0f, 3.0f)]
    public float intensity = 1.0f;

    [Header("POSITION_________________________")]
    public bool changePosition = false;
    public Vector3 minPosition = new Vector3(.0f, .0f, .0f);
    public Vector3 maxPosition = new Vector3(.0f, .0f, .0f);
    public float posSeparationTime = 1.0f;
    public float posMaxTime = 2.0f;

    [Header("ROTATION_________________________")]
    public bool changeRotation = false;
    public Vector3 minRotation = new Vector3(.0f, .0f, .0f);
    public Vector3 maxRotation = new Vector3(.0f, .0f, .0f);
    public float rotSeparationTime = 1.0f;
    public float rotMaxTime = 2.0f;

    [Header("SCALE____________________________")]
    public bool changeScale = false;
    public Vector3 minScale = new Vector3(1.0f, 1.0f, .0f);
    public Vector3 maxScale = new Vector3(1.2f, 1.2f, .0f);
    public float scaSeparationTime = 1.0f;
    public float scaMaxTime = 2.0f;

    [Header("DEBUG____________________________")]
    [SerializeField] private Animation anim;
    [SerializeField] private AnimationClip clip;
    private float timer;
    private void Awake()
    {
        anim = GetComponent<Animation>();
        clip = anim.clip;

        if (randTimeStart)  { timeBeforeStart = Random.Range(minTime, maxTime); }
        if (changePosition) { ChangePos(); }
        if (changeRotation) { ChangeRot(); }
        if (changeScale)    { ChangeScale(); }
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

    private void ChangePos()
    {
        ChangeAnimation(posSeparationTime, posMaxTime, minPosition.x, maxPosition.x, "localPosition.x");
        ChangeAnimation(posSeparationTime, posMaxTime, minPosition.y, maxPosition.y, "localPosition.y");
        ChangeAnimation(posSeparationTime, posMaxTime, minPosition.z, maxPosition.z, "localPosition.z");
    }

    private void ChangeRot()
    {
        ChangeAnimation(rotSeparationTime, rotMaxTime, minRotation.x, maxRotation.x, "localEulerAnglesRaw.x");
        ChangeAnimation(rotSeparationTime, rotMaxTime, minRotation.y, maxRotation.y, "localEulerAnglesRaw.y");
        ChangeAnimation(rotSeparationTime, rotMaxTime, minRotation.z, maxRotation.z, "localEulerAnglesRaw.z");
    }

    private void ChangeScale()
    {
        ChangeAnimation(scaSeparationTime, scaMaxTime, minScale.x, maxScale.x, "localScale.x");
        ChangeAnimation(scaSeparationTime, scaMaxTime, minScale.y, maxScale.y, "localScale.y");
        ChangeAnimation(scaSeparationTime, scaMaxTime, minScale.z, maxScale.z, "localScale.z");
    }

    private void ChangeAnimation(float sepTime, float maxTime, float min, float max, string propertyName)
    {
        Keyframe[] keys;
        int nbKeys = (int)Mathf.Ceil(maxTime / sepTime);
        keys = new Keyframe[nbKeys + 1];

        float keyTime = .0f;
        for(int i = 0; i < keys.Length; i++)
        {
            // Even
            if(i % 2 == 0)
            {
                keys[i] = new Keyframe(keyTime, min * intensity);
            }
            // Odd
            else
            {
                keys[i] = new Keyframe(keyTime, max * intensity);
            }

            keyTime += sepTime;
            keyTime = Mathf.Clamp(keyTime, 0, maxTime);
        }
        AnimationCurve curve = new AnimationCurve(keys);

        clip.SetCurve("", typeof(Transform), propertyName, curve);
    }
}
