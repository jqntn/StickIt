using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class UI : MonoBehaviour
{
    [SerializeField] private float secondsToPress;
    [SerializeField] private LayerSwitch mainLayerSwitch;
    [SerializeField] private LayerSwitch subLayerSwitch;
    public void Play() => StartCoroutine(PressCoroutine(() => SceneManager.LoadScene("1_MenuSelection")));
    public void Help() => StartCoroutine(PressCoroutine(() => mainLayerSwitch.ChangeLayer("Layer_Help")));
    public void Options() => StartCoroutine(PressCoroutine(() => mainLayerSwitch.ChangeLayer("Layer_Options")));
    public void Quit() => StartCoroutine(PressCoroutine(() => Application.Quit()));
    public void Website() => Application.OpenURL("https://olivier-de-benoist.itch.io/stick-it");
    public IEnumerator PressCoroutine(Action func)
    {
        yield return new WaitForSeconds(secondsToPress);
        func?.Invoke();
    }
    public void OnReturn(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            subLayerSwitch.ChangeLayer("Layer_Video");
            mainLayerSwitch.ChangeLayer("Layer_Main");
        }
    }
    public void OnY(InputAction.CallbackContext context)
    { if (context.performed) Website(); }
    public void OnPause(InputAction.CallbackContext context)
    { }
}