using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    GameObject alterMenu;
    GameObject mainMenu;
    GameObject precedingMenu;
    private void Start()
    {
        alterMenu = GameObject.Find("AlterMenu");
        mainMenu = GameObject.Find("MainMenu");
    }
    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void Options(GameObject go)
    {
        mainMenu.SetActive(false);
        alterMenu.SetActive(true);
        if (precedingMenu)
            precedingMenu.SetActive(false);
        go.SetActive(true);
        precedingMenu = go;
    }
    public void Return()
    {
        alterMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
