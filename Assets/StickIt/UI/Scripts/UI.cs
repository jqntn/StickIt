using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UI : MonoBehaviour
{
    [SerializeField]
    private float secondsToPress;
    [SerializeField] private List<GameObject> layers;
    private Dictionary<string, GameObject> layersDictionary;
    private void Start()
    {
        layersDictionary = layers.ToDictionary(x => x.name, x => x);
        ChangeLayer("Layer_Main");
    }
    public void ChangeLayer(string layer)
    {
        foreach (var item in layers)
        {
            if (item.name == layer)
            {
                if (!item.activeSelf) item.SetActive(true);
                continue;
            }
            item.SetActive(false);
        }
    }
    public void Play() => StartCoroutine(PressCoroutine(() => SceneManager.LoadScene("1_MenuSelection")));
    public void Help() => StartCoroutine(PressCoroutine(() => ChangeLayer("Layer_Help")));
    public void Options() => StartCoroutine(PressCoroutine(() => ChangeLayer("Layer_Options")));
    public void Quit() => StartCoroutine(PressCoroutine(() => Application.Quit()));
    public IEnumerator PressCoroutine(Action func)
    {
        yield return new WaitForSeconds(secondsToPress);
        func?.Invoke();
    }
}