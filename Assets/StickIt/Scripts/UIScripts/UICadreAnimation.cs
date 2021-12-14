using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICadreAnimation : MonoBehaviour
{
    // Get the scale
    // Make the value change between a min and a max

    [Header("ANIMATION______________________")]
    public Vector2 minScale = new Vector2(1.0f, 1.0f);
    public Vector2 maxScale = new Vector2(1.05f, 1.05f);
    public float animationTime = 0.5f;
    public AnimationCurve curve = new AnimationCurve();
    [Header("DEBUG__________________________")]
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private float timer = 0.0f;
    [SerializeField] private bool hasReverse = false;

    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        while(timer <= animationTime)
        {
            if (hasReverse)
            {
                timer += Time.deltaTime;
                float ratio = timer / animationTime;
                rectTransform.localScale = new Vector3(
                    Mathf.Lerp(maxScale.x, minScale.x, curve.Evaluate(ratio)),
                    Mathf.Lerp(maxScale.y, minScale.y, curve.Evaluate(ratio)),
                    rectTransform.localScale.z);
            }
            else{
                timer += Time.deltaTime;
                float ratio = timer / animationTime;
                rectTransform.localScale = new Vector3(
                    Mathf.Lerp(minScale.x, maxScale.x, curve.Evaluate(ratio)),
                    Mathf.Lerp(minScale.y, maxScale.y, curve.Evaluate(ratio)),
                    rectTransform.localScale.z);
            }
            return;
        }

        timer = 0.0f;
        hasReverse = !hasReverse;

    }
}
