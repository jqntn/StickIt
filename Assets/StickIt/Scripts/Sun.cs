using System.Collections;
using UnityEngine;
class Sun : Unique<Sun> {

    public bool isAnimate = false;
    public Vector3 rotation = new Vector3(.0f, 1.0f, .0f);
    private IEnumerator Start()
    {
        while (isAnimate)
        {
            transform.Rotate(rotation);
            yield return null;
        }
    }

}