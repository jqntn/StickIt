using UnityEngine;
using UnityEditor;

public class TweakWindow : EditorWindow
{
    public bool showPlayers = true;
    public GUIStyle caca;
    [MenuItem("StickIt/TweakWindow")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        TweakWindow window = (TweakWindow)EditorWindow.GetWindow(typeof(TweakWindow));
        window.Show();

    }

    private void OnGUI()
    {

        showPlayers = EditorGUI.Foldout(new Rect(3, 3, position.width - 6, 15), showPlayers, "Player");
        /*if (showPlayers)
        {

        }*/
    }
    
}
