using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
public class TweakWindow : EditorWindow
{
    public Texture textureFoldout;
    bool showPlayers = false;
    bool showMultiplayerManager = false;
    [MenuItem("StickIt/TweakWindow")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        TweakWindow window = (TweakWindow)EditorWindow.GetWindow(typeof(TweakWindow));
        window.Show();
    }

    private void OnGUI()
    {
        //var serializedObject = new SerializedObject(caca);
        GUIContent m_Content = new GUIContent();
        m_Content.image = textureFoldout;
        GUIStyle myFoldoutStyle = new GUIStyle(EditorStyles.foldout);
        myFoldoutStyle.fontStyle = FontStyle.Bold;
        myFoldoutStyle.fontSize = 14;
        Color myStyleColor = Color.red;
        Color m_ColorFoldout = Color.white;
        myFoldoutStyle.normal.textColor = m_ColorFoldout;
        myFoldoutStyle.onNormal.textColor = myStyleColor;
        myFoldoutStyle.hover.textColor = myStyleColor;
        myFoldoutStyle.onHover.textColor = myStyleColor;
        myFoldoutStyle.focused.textColor = myStyleColor;
        myFoldoutStyle.onFocused.textColor = myStyleColor;
        myFoldoutStyle.active.textColor = myStyleColor;
        myFoldoutStyle.onActive.textColor = myStyleColor;
        if (FindObjectOfType<MultiplayerManager>())
        {
            showMultiplayerManager = EditorGUILayout.Foldout(showMultiplayerManager, "Multiplayer Variables", true, myFoldoutStyle);
            if (showMultiplayerManager)
            {
                GUILayout.Label("\nSoftBody settings");
            }
            showPlayers = EditorGUILayout.Foldout(showPlayers,"Player", true ,myFoldoutStyle );
            if (showPlayers)
            {
                GUILayout.Label("\nSoftBody settings");
                /*SerializedProperty sp = serializedObject.GetIterator();
                while (sp.Next(true))
                {
                    GUILayout.Label("\n" + sp.name);
                }*/
            }
        }
    }
    
}
