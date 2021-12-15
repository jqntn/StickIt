using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Pause : Unique<Pause>
{
    public bool isPaused;
    public LayerSwitch mainLayerSwitch;
    public LayerSwitch hLayerSwitch;
    public LayerSwitch oLayerSwitch;
    public EasterEgg easterEgg;
    public GameObject mainLayer;
    [SerializeField] private float secondsToPress;
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
        StartCoroutine(PressCoroutine(() =>
        {
            Time.timeScale = 1;
            AkSoundEngine.PostEvent("Play_SFX_UI_Submit", gameObject);
            SceneManager.LoadScene("0_MainMenu");
            Destroy(MultiplayerManager.instance.gameObject);
            MultiplayerManager.instance = null;
            Destroy(MainCamera.instance.gameObject);
            MainCamera.instance = null;
            Destroy(MapManager.instance.gameObject);
            MapManager.instance = null;
            Destroy(Sun.instance.gameObject);
            Sun.instance = null;
            foreach (var item in GameObject.FindGameObjectsWithTag("Player")) Destroy(item);
            Pause.instance = null;
            Destroy(gameObject);
        }));
    }
    public IEnumerator PressCoroutine(Action func)
    {
        yield return new WaitForSecondsRealtime(secondsToPress);
        func?.Invoke();
    }
}