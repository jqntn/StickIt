using System.Collections;
using UnityEngine;
class Sun : Unique<Sun> {

    public bool isAnimate = false;
    public Vector3 rotation = new Vector3(0.1f, 0.1f, 0.1f);
    private IEnumerator Start()
    {
        while (isAnimate)
        {
            transform.Rotate(rotation);
            yield return null;
        }
    }

}