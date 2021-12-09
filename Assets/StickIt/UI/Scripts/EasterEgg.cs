using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class EasterEgg : MonoBehaviour
{
    [SerializeField] private GameObject layer;
    [SerializeField] private List<GameObject> slimes;
    [SerializeField] private float waitDelay;
    [SerializeField] private float jumpTime;
    [SerializeField] private float jumpHeight;
    private bool canJump = true;
    public void Jump(InputAction.CallbackContext context)
    { if (context.performed && layer.activeSelf && canJump) StartCoroutine(MainCoroutine()); }
    private IEnumerator MainCoroutine()
    {
        StartCoroutine(CanJump());
        foreach (var i in slimes)
        {
            StartCoroutine(SubCoroutine(i));
            yield return new WaitForSeconds(waitDelay);
        }
    }
    private IEnumerator SubCoroutine(GameObject obj)
    {
        var startPos = obj.transform.position;
        float timer = jumpTime;
        while (timer >= 0)
        {
            timer -= Time.unscaledDeltaTime;
            obj.transform.position = Vector3.Lerp(obj.transform.position, obj.transform.position + new Vector3(0, jumpHeight), Time.unscaledDeltaTime);
            yield return null;
        }
        timer = jumpTime;
        while (timer >= 0)
        {
            timer -= Time.unscaledDeltaTime;
            obj.transform.position = Vector3.Lerp(obj.transform.position, obj.transform.position + new Vector3(0, -jumpHeight), Time.unscaledDeltaTime);
            yield return null;
        }
        obj.transform.position = startPos;
    }
    private IEnumerator CanJump()
    {
        canJump = false;
        yield return new WaitForSeconds(jumpTime * 2);
        canJump = true;
    }
}