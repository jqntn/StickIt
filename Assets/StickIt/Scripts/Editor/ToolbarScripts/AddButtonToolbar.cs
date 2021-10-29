using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UnityToolbarExtender.Examples
{
	static class ToolbarStyles
	{
		public static readonly GUIStyle commandButtonStyle;

		static ToolbarStyles()
		{
			commandButtonStyle = new GUIStyle("Command")
			{
				fontSize = 16,
				alignment = TextAnchor.MiddleCenter,
				imagePosition = ImagePosition.ImageAbove,
				fontStyle = FontStyle.Bold
			};
		}
	}




	[InitializeOnLoad]
	public class SceneSwitchLeftButton
	{
		 

		static SceneSwitchLeftButton()
		{
			ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
		}

		static void OnToolbarGUI()
		{
			GUILayout.FlexibleSpace();

			if(GUILayout.Button(new GUIContent("2", "Launch Game with 2 players"), ToolbarStyles.commandButtonStyle))
			{
				LaunchGame(2);
			}

			if(GUILayout.Button(new GUIContent("3", "Launch Game with 3 players"), ToolbarStyles.commandButtonStyle))
			{
				LaunchGame(3);
			}

			if (GUILayout.Button(new GUIContent("4", "Launch Game with 4 players"), ToolbarStyles.commandButtonStyle))
			{
				LaunchGame(4);
				
			}

		}

		static void LaunchGame(int nbrOfPlayers)
		{
			MultiplayerManager multiplayerManager;
			multiplayerManager = GameObject.FindObjectOfType<MultiplayerManager>();
			multiplayerManager.nbrOfPlayer = nbrOfPlayers;
			PrefabUtility.RecordPrefabInstancePropertyModifications(multiplayerManager);
			EditorApplication.EnterPlaymode();
		}

	}

	//static class SceneHelper
	//{
	//	static string sceneToOpen;

	//	public static void StartScene(string sceneName)
	//	{
	//		if(EditorApplication.isPlaying)
	//		{
	//			EditorApplication.isPlaying = false;
	//		}

	//		sceneToOpen = sceneName;
	//		EditorApplication.update += OnUpdate;
	//	}

	//	static void OnUpdate()
	//	{
	//		if (sceneToOpen == null ||
	//		    EditorApplication.isPlaying || EditorApplication.isPaused ||
	//		    EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
	//		{
	//			return;
	//		}

	//		EditorApplication.update -= OnUpdate;

	//		if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
	//		{
	//			// need to get scene via search because the path to the scene
	//			// file contains the package version so it'll change over time
	//			string[] guids = AssetDatabase.FindAssets("t:scene " + sceneToOpen, null);
	//			if (guids.Length == 0)
	//			{
	//				Debug.LogWarning("Couldn't find scene file");
	//			}
	//			else
	//			{
	//				string scenePath = AssetDatabase.GUIDToAssetPath(guids[0]);
	//				EditorSceneManager.OpenScene(scenePath);
	//				EditorApplication.isPlaying = true;
	//			}
	//		}
	//		sceneToOpen = null;
	//	}
	//}
}