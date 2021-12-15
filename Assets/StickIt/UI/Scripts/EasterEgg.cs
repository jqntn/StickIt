using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class EasterEgg : MonoBehaviour
{
    public GameObject layer;
    public bool canJump;
    [SerializeField] private List<GameObject> slimes;
    [SerializeField] private float waitDelay;
    [SerializeField] private float jumpTime;
    [SerializeField] private float jumpHeight;
    private List<Vector3> startPos;
    public void Jump(InputAction.CallbackContext context)
    { if (context.performed && layer.activeSelf && canJump) StartCoroutine(MainCoroutine()); }
    public IEnumerator MainCoroutine()
    {
        StartCoroutine(CanJump());
        for (var i = 0; i < slimes.Count; i++)
        {
            if (layer.activeSelf) StartCoroutine(SubCoroutine(slimes[i], startPos[i]));
            yield return new WaitForSecondsRealtime(waitDelay);
        }
    }
    private IEnumerator SubCoroutine(GameObject slime, Vector3 startPos)
    {
        float timer = jumpTime;
        while (timer >= 0)
        {
            timer -= Time.unscaledDeltaTime;
            slime.transform.localPosition = Vector3.Lerp(slime.transform.localPosition, slime.transform.localPosition + new Vector3(0, jumpHeight), Time.unscaledDeltaTime);
            yield return null;
        }
        timer = jumpTime;
        while (timer >= 0)
        {
            timer -= Time.unscaledDeltaTime;
            slime.transform.localPosition = Vector3.Lerp(slime.transform.localPosition, slime.transform.localPosition + new Vector3(0, -jumpHeight), Time.unscaledDeltaTime);
            yield return null;
        }
        slime.transform.localPosition = startPos;
    }
    private IEnumerator CanJump()
    {
        canJump = false;
        yield return new WaitForSecondsRealtime(jumpTime * 2);
        canJump = true;
    }
    private void OnEnable()
    {
        startPos = new List<Vector3>(new Vector3[slimes.Count]);
        for (var i = 0; i < slimes.Count; i++) startPos[i] = slimes[i].transform.localPosition;
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        for (var i = 0; i < slimes.Count; i++) slimes[i].transform.localPosition = startPos[i];
        canJump = true;
    }
}