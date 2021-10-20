using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessEffects : MonoBehaviour
{
    private Volume _volume;
    [SerializeField] private MMFeedbacks grosChoc;


    // Start is called before the first frame update
    void Start()
    {
        _volume = GetComponent<Volume>();

    }

    // Update is called once per frame
    void Update()
    {

    }
}
