using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class Pause : Unique<Pause>
{
    public LayerSwitch mainLayerSwitch;
    [SerializeField] private float secondsToPress;
    [SerializeField] private LayerSwitch subLayerSwitch;
    [SerializeField] private GameObject mainLayer;
    [SerializeField] private bool isPaused;
    private void Start() => mainLayer.SetActive(false);
    public void PauseGame()
    {
        if (!MapManager.instance.isBusy)
        {
            isPaused ^= true;
            Time.timeScale = isPaused ? 0 : 1;
            mainLayer.SetActive(isPaused);
        }
    }
    public void Help() => StartCoroutine(PressCoroutine(() => mainLayerSwitch.ChangeLayer("Layer_Help")));
    public void Options() => StartCoroutine(PressCoroutine(() => mainLayerSwitch.ChangeLayer("Layer_Options")));
    public void Menu()
    {
        Destroy(FindObjectOfType<MultiplayerManager>().gameObject);
        Destroy(FindObjectOfType<MapManager>().gameObject);
        Destroy(FindObjectOfType<MainCamera>().gameObject);
        Destroy(FindObjectOfType<Sun>().gameObject);
        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var item in players) Destroy(item);
        StartCoroutine(PressCoroutine(() => SceneManager.LoadScene("0_MainMenu")));
    }
    public IEnumerator PressCoroutine(Action func)
    {
        yield return new WaitForSecondsRealtime(secondsToPress);
        func?.Invoke();
    }
    public void OnReturn(InputAction.CallbackContext context)
    {
        if (context.performed && isPaused)
        {
            if (mainLayer.activeSelf)
            {
                foreach (var item in FindObjectsOfType<PlayerInput>()) item.enabled = false;
                foreach (var item in GameObject.FindGameObjectsWithTag("Player")) item.GetComponent<PlayerInput>().enabled = true;
                PauseGame();
            }
            else
            {
                mainLayerSwitch.ChangeLayer("Layer_Main");
                subLayerSwitch.ChangeLayer("Layer_Video");
            }
        }
    }
    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (MapManager.instance.curMod == "MusicalChairs" && FindObjectOfType<MusicalChairManager>().inTransition)
                for (int i = 0; i < Gamepad.all.Count; i++)
                    Gamepad.all[i].SetMotorSpeeds(.1f, .1f);
            foreach (var item in FindObjectsOfType<PlayerInput>()) item.enabled = false;
            foreach (var item in GameObject.FindGameObjectsWithTag("Player")) item.GetComponent<PlayerInput>().enabled = true;
            mainLayerSwitch.ChangeLayer("Layer_Main");
            PauseGame();
        }
    }
    public void SoundMove(InputAction.CallbackContext context)
    { if (context.performed) AkSoundEngine.PostEvent("Play_SFX_UI_Move", gameObject); }
    public void SoundSubmit(InputAction.CallbackContext context)
    { if (context.performed) AkSoundEngine.PostEvent("Play_SFX_UI_Submit", gameObject); }
    public void SoundReturn(InputAction.CallbackContext context)
    { if (context.performed) AkSoundEngine.PostEvent("Play_SFX_UI_Return", gameObject); }
    public void SoundY(InputAction.CallbackContext context)
    { if (context.performed && mainLayer.activeSelf) AkSoundEngine.PostEvent("Play_SFX_UI_Submit", gameObject); }
}