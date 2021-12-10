using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class Menu : MonoBehaviour
{
    [SerializeField] private float secondsToPress;
    [SerializeField] private LayerSwitch mainLayerSwitch;
    [SerializeField] private LayerSwitch subLayerSwitch;
    [SerializeField] private GameObject mainLayer;
    public void Play() => StartCoroutine(PressCoroutine(() => { foreach (var item in FindObjectsOfType<PlayerInput>()) item.enabled = false; SceneManager.LoadScene("1_MenuSelection"); }));
    public void Help() => StartCoroutine(PressCoroutine(() => mainLayerSwitch.ChangeLayer("Layer_Help")));
    public void Options() => StartCoroutine(PressCoroutine(() => mainLayerSwitch.ChangeLayer("Layer_Options")));
    public void Quit() => StartCoroutine(PressCoroutine(() => Application.Quit()));
    public void Website() => Application.OpenURL("https://olivier-de-benoist.itch.io/stick-it");
    public IEnumerator PressCoroutine(Action func)
    {
        yield return new WaitForSecondsRealtime(secondsToPress);
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
    { if (context.performed && mainLayer.activeSelf) Website(); }
    public void OnPause(InputAction.CallbackContext context)
    { }
    public void SoundMove(InputAction.CallbackContext context)
    { if (context.performed) AkSoundEngine.PostEvent("Play_SFX_UI_Move", gameObject); }
    public void SoundSubmit(InputAction.CallbackContext context)
    { if (context.performed) AkSoundEngine.PostEvent("Play_SFX_UI_Submit", gameObject); }
    public void SoundReturn(InputAction.CallbackContext context)
    { if (context.performed) AkSoundEngine.PostEvent("Play_SFX_UI_Return", gameObject); }
    public void SoundY(InputAction.CallbackContext context)
    { if (context.performed && mainLayer.activeSelf) AkSoundEngine.PostEvent("Play_SFX_UI_Submit", gameObject); }
}