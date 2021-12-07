using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UI : MonoBehaviour
{
    [SerializeField]
    private float secondsToPress;
    private void Start()
    { }
    private void Update()
    { }
    public void Play() => StartCoroutine(PressCoroutine(PlayFinal));
    public void Quit() => StartCoroutine(PressCoroutine(QuitFinal));
    public void QuitFinal() => Application.Quit();
    public void PlayFinal() => SceneManager.LoadScene("1_MenuSelection");
    public IEnumerator PressCoroutine(Action func)
    {
        yield return new WaitForSeconds(secondsToPress);
        func?.Invoke();
    }
}