using UnityEngine;
using UnityEditor;

public class TweakWindow : EditorWindow
{
    [MenuItem("StickIt/TweakWindow")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        TweakWindow window = (TweakWindow)EditorWindow.GetWindow(typeof(TweakWindow));
        window.Show();
    }
}
