#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

#endif
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class ForcedStartFromLobbyScene
{
//    private const string STARTSCENEPATH = "Assets/Scenes/LobbyScene.unity";
//    private const string EDITORPREFKEY = "ForceStartScene_PreviousScenePath";

//    static ForcedStartFromLobbyScene()
//    {
//#if UNITY_EDITOR
//        EditorApplication.playModeStateChanged += OnPlayModeChanged;
//#endif
//    }

//#if UNITY_EDITOR
//    private static bool _isSceneSwitching  = false; 

//    private static void OnPlayModeChanged(PlayModeStateChange state)
//    {
//        if (state == PlayModeStateChange.ExitingEditMode)
//        {
//            string currentScene = SceneManager.GetActiveScene().path;

//            if (currentScene != STARTSCENEPATH)
//            {
//                EditorPrefs.SetString(EDITORPREFKEY, currentScene);
//                _isSceneSwitching = true;

//                EditorApplication.isPlaying = false;
//                EditorApplication.delayCall += () =>
//                {
//                    EditorSceneManager.OpenScene(STARTSCENEPATH);
//                    EditorApplication.isPlaying = true;
//                };
//            }
//        }
//        else if (state == PlayModeStateChange.EnteredEditMode)
//        {
//            if (_isSceneSwitching)
//            {
//                _isSceneSwitching = false;
//                return; // 씬 전환 재시작으로 인한 호출은 무시
//            }

//            // 플레이 종료 → 원래 씬 복구
//            if (EditorPrefs.HasKey(EDITORPREFKEY))
//            {
//                string prevScene = EditorPrefs.GetString(EDITORPREFKEY);
//                if (!string.IsNullOrEmpty(prevScene) && prevScene != STARTSCENEPATH)
//                {
//                    EditorSceneManager.OpenScene(prevScene);
//                }
//                EditorPrefs.DeleteKey(EDITORPREFKEY);
//            }
//        }
//    }
//#endif
}
