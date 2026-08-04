using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace GulfRun.Editor
{
    /// <summary>
    /// Official Editor Play Mode always starts at Boot (Build Settings index 0),
    /// regardless of which scene tab is open. Prevents accidental starts in
    /// QuickPlay / LoadingScreen / Gameplay left open from prior work.
    /// Dedicated test scenes remain reachable via GulfRun/Play Flow verify menus
    /// (those open a scene then EnterPlaymode explicitly).
    /// </summary>
    [InitializeOnLoad]
    public static class EditorPlayModeStartScene
    {
        private const string BootScenePath = "Assets/_Project/Scenes/Boot.unity";

        static EditorPlayModeStartScene()
        {
            EditorApplication.delayCall += ApplyBootPlayModeStartScene;
        }

        [MenuItem("GulfRun/Play Flow/Use Boot as Play Mode Start Scene")]
        public static void ApplyBootPlayModeStartScene()
        {
            SceneAsset boot = AssetDatabase.LoadAssetAtPath<SceneAsset>(BootScenePath);
            if (boot == null)
            {
                Debug.LogError("[EditorPlayModeStartScene] Boot scene missing at " + BootScenePath);
                return;
            }

            if (EditorSceneManager.playModeStartScene != boot)
            {
                EditorSceneManager.playModeStartScene = boot;
                Debug.Log("[EditorPlayModeStartScene] Play Mode Start Scene set to Boot.");
            }
        }

        [MenuItem("GulfRun/Play Flow/Clear Play Mode Start Scene (use open scene)")]
        public static void ClearPlayModeStartScene()
        {
            EditorSceneManager.playModeStartScene = null;
            Debug.Log("[EditorPlayModeStartScene] Play Mode Start Scene cleared — Editor will play the open scene (dev only).");
        }
    }
}
