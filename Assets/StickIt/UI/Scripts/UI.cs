using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UI : MonoBehaviour
{
    private void Start()
    { }
    private void Update()
    { }
    public void Play() => SceneManager.LoadScene("1_MenuSelection");
    public void Quit() => Application.Quit();
}