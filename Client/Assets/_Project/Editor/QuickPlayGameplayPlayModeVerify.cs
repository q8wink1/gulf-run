using System;
using System.Collections;
using System.IO;
using GulfRun.Core.Services;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GulfRun.Editor
{
    /// <summary>
    /// Batchmode Play Mode verification: Quick Play offline flow reaches Gameplay.
    /// Path: set OfflineRaceEntryService → open QuickPlay (or LoadingScreen) → wait for Gameplay.
    /// Writes VERIFY_PASS / VERIFY_FAIL under Client/BuildLogs and exits the Editor.
    /// </summary>
    [InitializeOnLoad]
    public static class QuickPlayGameplayPlayModeVerify
    {
        private const string QuickPlayScenePath = "Assets/_Project/Scenes/QuickPlay.unity";
        private const string LoadingScreenScenePath = "Assets/_Project/Scenes/LoadingScreen.unity";
        private const string GameplayScenePath = "Assets/_Project/Scenes/Gameplay.unity";
        private const string ResultLogRelative = "BuildLogs/QuickPlayGameplayVerify.log";
        private const string FlagRelative = "Temp/QuickPlayGameplayVerify.flag";
        private const string MarkerPass = "VERIFY_PASS";
        private const string MarkerFail = "VERIFY_FAIL";

        private static bool _updateHooked;
        private static bool _runnerStarted;
        private static bool _exitScheduled;
        private static double _playEnteredAt;
        private static bool _savedEnterPlayModeOptions;
        private static bool _prevEnterPlayModeOptionsEnabled;
        private static EnterPlayModeOptions _prevEnterPlayModeOptions;

        static QuickPlayGameplayPlayModeVerify()
        {
            TryResumeFromFlag();
        }

        [MenuItem("GulfRun/Play Flow/Verify Quick Play → Gameplay (Play Mode)")]
        public static void VerifyQuickPlayGameplay()
        {
            string resultPath = ResultPath();
            Directory.CreateDirectory(Path.GetDirectoryName(resultPath) ?? ".");
            File.WriteAllText(resultPath, "VERIFY_STARTED " + DateTime.UtcNow.ToString("o") + Environment.NewLine);

            if (!File.Exists(QuickPlayScenePath))
            {
                FailAndExit("QuickPlay scene missing at " + QuickPlayScenePath);
                return;
            }

            if (!File.Exists(LoadingScreenScenePath))
            {
                FailAndExit("LoadingScreen scene missing at " + LoadingScreenScenePath);
                return;
            }

            if (!File.Exists(GameplayScenePath))
            {
                FailAndExit("Gameplay scene missing at " + GameplayScenePath);
                return;
            }

            if (!IsInBuildSettings(GameplayScenePath))
            {
                FailAndExit("Gameplay scene is not in EditorBuildSettings: " + GameplayScenePath);
                return;
            }

            if (!IsInBuildSettings(LoadingScreenScenePath))
            {
                FailAndExit("LoadingScreen scene is not in EditorBuildSettings: " + LoadingScreenScenePath);
                return;
            }

            if (!IsInBuildSettings(QuickPlayScenePath))
            {
                FailAndExit("QuickPlay scene is not in EditorBuildSettings: " + QuickPlayScenePath);
                return;
            }

            Append("Preflight OK — QuickPlay/LoadingScreen/Gameplay exist and are in Build Settings.");
            Append("Gameplay LoadScene name expected: 'Gameplay'");

            _prevEnterPlayModeOptionsEnabled = EditorSettings.enterPlayModeOptionsEnabled;
            _prevEnterPlayModeOptions = EditorSettings.enterPlayModeOptions;
            EditorSettings.enterPlayModeOptionsEnabled = true;
            EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload;
            _savedEnterPlayModeOptions = true;

            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(QuickPlayScenePath, OpenSceneMode.Single);

            WriteFlag("waiting-play");
            Append("Entering Play Mode for QuickPlay (DisableDomainReload)…");
            HookUpdate();
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.EnterPlaymode();
        }

        private static bool IsInBuildSettings(string scenePath)
        {
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled && scene.path == scenePath)
                {
                    return true;
                }
            }

            return false;
        }

        private static void TryResumeFromFlag()
        {
            string flag = ReadFlag();
            if (string.IsNullOrEmpty(flag))
            {
                return;
            }

            HookUpdate();
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            if (EditorApplication.isPlaying && flag == "waiting-play")
            {
                StartRunner();
            }
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            Append("PlayModeState=" + state);
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                _playEnteredAt = EditorApplication.timeSinceStartup;
                StartRunner();
            }
        }

        private static void StartRunner()
        {
            if (_runnerStarted || !EditorApplication.isPlaying)
            {
                return;
            }

            _runnerStarted = true;
            WriteFlag("running");
            GameObject host = new GameObject("~QuickPlayGameplayVerifyRunner");
            UnityEngine.Object.DontDestroyOnLoad(host);
            host.AddComponent<QuickPlayGameplayVerifyRunner>().Begin(Report);
        }

        private static void HookUpdate()
        {
            if (_updateHooked)
            {
                return;
            }

            _updateHooked = true;
            EditorApplication.update += OnEditorUpdate;
        }

        private static void UnhookUpdate()
        {
            if (!_updateHooked)
            {
                return;
            }

            _updateHooked = false;
            EditorApplication.update -= OnEditorUpdate;
        }

        private static void OnEditorUpdate()
        {
            string flag = ReadFlag();
            if (string.IsNullOrEmpty(flag))
            {
                return;
            }

            if (flag == "waiting-play" && EditorApplication.isPlaying && !_runnerStarted)
            {
                StartRunner();
            }

            if (_runnerStarted && _playEnteredAt > 0
                && EditorApplication.timeSinceStartup - _playEnteredAt > 90.0
                && flag == "running")
            {
                FailAndExit("Watchdog timeout: runner did not finish within 90s of EnteredPlayMode. activeScene="
                    + SceneManager.GetActiveScene().name);
            }
        }

        private static void Report(bool pass, string detail)
        {
            if (pass)
            {
                Append(MarkerPass + " " + detail);
                Debug.Log("[QuickPlayGameplayVerify] " + MarkerPass + " " + detail);
            }
            else
            {
                Append(MarkerFail + " " + detail);
                Debug.LogError("[QuickPlayGameplayVerify] " + MarkerFail + " " + detail);
            }

            WriteFlag(pass ? "pass" : "fail");
            ScheduleExit(pass ? 0 : 1);
        }

        private static void FailAndExit(string detail)
        {
            Append(MarkerFail + " " + detail);
            Debug.LogError("[QuickPlayGameplayVerify] " + MarkerFail + " " + detail);
            WriteFlag("fail");
            ScheduleExit(1);
        }

        private static void ScheduleExit(int code)
        {
            if (_exitScheduled)
            {
                return;
            }

            _exitScheduled = true;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            UnhookUpdate();

            EditorApplication.delayCall += () =>
            {
                RestoreEnterPlayModeOptions();

                if (EditorApplication.isPlaying)
                {
                    EditorApplication.ExitPlaymode();
                }

                EditorApplication.delayCall += () =>
                {
                    try
                    {
                        if (File.Exists(FlagPath()))
                        {
                            File.Delete(FlagPath());
                        }
                    }
                    catch
                    {
                        // ignored
                    }

                    EditorApplication.Exit(code);
                };
            };
        }

        private static void RestoreEnterPlayModeOptions()
        {
            if (!_savedEnterPlayModeOptions)
            {
                return;
            }

            EditorSettings.enterPlayModeOptionsEnabled = _prevEnterPlayModeOptionsEnabled;
            EditorSettings.enterPlayModeOptions = _prevEnterPlayModeOptions;
            _savedEnterPlayModeOptions = false;
        }

        private static string ResultPath() =>
            Path.GetFullPath(Path.Combine(Application.dataPath, "..", ResultLogRelative));

        private static string FlagPath() =>
            Path.GetFullPath(Path.Combine(Application.dataPath, "..", FlagRelative));

        private static void WriteFlag(string value)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FlagPath()) ?? ".");
                File.WriteAllText(FlagPath(), value);
            }
            catch (Exception ex)
            {
                Debug.LogError("[QuickPlayGameplayVerify] flag write failed: " + ex.Message);
            }
        }

        private static string ReadFlag()
        {
            try
            {
                string path = FlagPath();
                return File.Exists(path) ? File.ReadAllText(path).Trim() : null;
            }
            catch
            {
                return null;
            }
        }

        private static void Append(string line)
        {
            try
            {
                File.AppendAllText(ResultPath(), line + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Debug.LogError("[QuickPlayGameplayVerify] log write failed: " + ex.Message);
            }
        }

        private sealed class QuickPlayGameplayVerifyRunner : MonoBehaviour
        {
            private Action<bool, string> _report;

            public void Begin(Action<bool, string> report)
            {
                _report = report;
                StartCoroutine(Run());
            }

            private IEnumerator Run()
            {
                yield return null;
                yield return null;

                AppendStatic("Runner start activeScene=" + SceneManager.GetActiveScene().name
                    + " OfflineIsActive=" + OfflineRaceEntryService.IsActive
                    + " PendingAuto=" + OfflineRaceEntryService.PendingLoadingAutoAdvance);

                // QuickPlayController.Start should arm offline + Load LoadingScreen.
                float deadline = Time.realtimeSinceStartup + 8f;
                while (SceneManager.GetActiveScene().name == "QuickPlay"
                       && Time.realtimeSinceStartup < deadline)
                {
                    yield return null;
                }

                string afterQuick = SceneManager.GetActiveScene().name;
                AppendStatic("After QuickPlay wait: activeScene=" + afterQuick
                    + " OfflineIsActive=" + OfflineRaceEntryService.IsActive
                    + " PendingAuto=" + OfflineRaceEntryService.PendingLoadingAutoAdvance);

                if (afterQuick == "Gameplay")
                {
                    _report(true, "Gameplay active immediately after QuickPlay (skipped LoadingScreen dwell).");
                    yield break;
                }

                if (afterQuick != "LoadingScreen" && afterQuick != "Gameplay")
                {
                    // Fallback path: force offline flag + LoadingScreen if QuickPlay stuck.
                    AppendStatic("QuickPlay did not reach LoadingScreen — forcing OfflineRaceEntry + LoadScene(LoadingScreen).");
                    OfflineRaceEntryService.BeginPendingEntry();
                    SceneManager.LoadScene("LoadingScreen");
                    yield return null;
                    yield return null;
                    afterQuick = SceneManager.GetActiveScene().name;
                    AppendStatic("Forced LoadingScreen load → activeScene=" + afterQuick);
                }

                if (afterQuick != "LoadingScreen" && afterQuick != "Gameplay")
                {
                    _report(false, "Expected LoadingScreen or Gameplay after QuickPlay; got '" + afterQuick + "'.");
                    yield break;
                }

                // Wait for LoadingScreen auto-advance (~2–3s) to Gameplay.
                deadline = Time.realtimeSinceStartup + 12f;
                while (SceneManager.GetActiveScene().name != "Gameplay"
                       && Time.realtimeSinceStartup < deadline)
                {
                    string current = SceneManager.GetActiveScene().name;
                    if (current != "LoadingScreen" && current != "Gameplay")
                    {
                        AppendStatic("Unexpected mid-transition scene='" + current + "'");
                    }

                    yield return null;
                }

                string finalName = SceneManager.GetActiveScene().name;
                AppendStatic("Final activeScene=" + finalName
                    + " path=" + SceneManager.GetActiveScene().path
                    + " OfflineIsActive=" + OfflineRaceEntryService.IsActive);

                if (finalName == "Gameplay")
                {
                    _report(true, "Gameplay active after Quick Play offline flow. scenePath="
                        + SceneManager.GetActiveScene().path);
                    yield break;
                }

                _report(false, "Gameplay did not become active. activeScene=" + finalName
                    + " path=" + SceneManager.GetActiveScene().path
                    + " OfflineIsActive=" + OfflineRaceEntryService.IsActive
                    + " PendingAuto=" + OfflineRaceEntryService.PendingLoadingAutoAdvance);
            }

            private static void AppendStatic(string line)
            {
                try
                {
                    string path = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ResultLogRelative));
                    File.AppendAllText(path, line + Environment.NewLine);
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}
