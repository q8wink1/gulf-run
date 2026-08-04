using System;
using System.Collections;
using System.IO;
using GulfRun.Features.MainMenu;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GulfRun.Editor
{
    /// <summary>
    /// Official navigation verify (MenuItem / batchmode executeMethod only — never auto-runs on Play):
    /// Boot → Intro → MainMenu → PlayMenu → QuickPlay → LoadingScreen → Gameplay.
    /// Writes VERIFY_PASS / VERIFY_FAIL under Client/BuildLogs and exits the Editor.
    /// </summary>
    [InitializeOnLoad]
    public static class OfficialNavigationPlayModeVerify
    {
        private const string BootScenePath = "Assets/_Project/Scenes/Boot.unity";
        private const string ResultLogRelative = "BuildLogs/OfficialNavigationVerify.log";
        private const string FlagRelative = "Temp/OfficialNavigationVerify.flag";
        private const string MarkerPass = "VERIFY_PASS";
        private const string MarkerFail = "VERIFY_FAIL";

        private static bool _updateHooked;
        private static bool _runnerStarted;
        private static bool _exitScheduled;
        private static double _playEnteredAt;
        private static bool _savedEnterPlayModeOptions;
        private static bool _prevEnterPlayModeOptionsEnabled;
        private static EnterPlayModeOptions _prevEnterPlayModeOptions;

        static OfficialNavigationPlayModeVerify()
        {
            TryResumeFromFlag();
        }

        [MenuItem("GulfRun/Play Flow/Verify Official Navigation (Boot → Gameplay)")]
        public static void VerifyOfficialNavigation()
        {
            string resultPath = ResultPath();
            Directory.CreateDirectory(Path.GetDirectoryName(resultPath) ?? ".");
            File.WriteAllText(resultPath, "VERIFY_STARTED " + DateTime.UtcNow.ToString("o") + Environment.NewLine);

            if (!File.Exists(BootScenePath))
            {
                FailAndExit("Boot scene missing at " + BootScenePath);
                return;
            }

            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            if (scenes.Length == 0 || !scenes[0].enabled || scenes[0].path != BootScenePath)
            {
                FailAndExit("EditorBuildSettings[0] must be Boot.unity. Found: "
                    + (scenes.Length > 0 ? scenes[0].path : "(empty)"));
                return;
            }

            Append("Preflight OK — Boot is Build Settings index 0.");
            Append("Expected flow: Boot → Intro → MainMenu → PlayMenu → QuickPlay → LoadingScreen → Gameplay");

            _prevEnterPlayModeOptionsEnabled = EditorSettings.enterPlayModeOptionsEnabled;
            _prevEnterPlayModeOptions = EditorSettings.enterPlayModeOptions;
            EditorSettings.enterPlayModeOptionsEnabled = true;
            EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload;
            _savedEnterPlayModeOptions = true;

            SceneAsset bootAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(BootScenePath);
            if (bootAsset != null)
            {
                EditorSceneManager.playModeStartScene = bootAsset;
            }

            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(BootScenePath, OpenSceneMode.Single);

            WriteFlag("waiting-play");
            Append("Entering Play Mode from Boot (DisableDomainReload)…");
            HookUpdate();
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.EnterPlaymode();
        }

        private static void TryResumeFromFlag()
        {
            string flag = ReadFlag();
            if (string.IsNullOrEmpty(flag))
            {
                return;
            }

            if (!EditorApplication.isPlayingOrWillChangePlaymode
                && flag != "waiting-play" && flag != "running")
            {
                try { File.Delete(FlagPath()); } catch { /* ignored */ }
                return;
            }

            HookUpdate();
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            if (EditorApplication.isPlaying && (flag == "waiting-play" || flag == "running"))
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
            GameObject host = new GameObject("~OfficialNavVerifyRunner");
            UnityEngine.Object.DontDestroyOnLoad(host);
            host.AddComponent<OfficialNavVerifyRunner>().Begin(Report);
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
                FailAndExit("Watchdog timeout: official flow did not finish within 90s.");
            }
        }

        private static void Report(bool pass, string detail)
        {
            if (pass)
            {
                Append(MarkerPass + " " + detail);
                Debug.Log("[OfficialNavVerify] " + MarkerPass + " " + detail);
            }
            else
            {
                Append(MarkerFail + " " + detail);
                Debug.LogError("[OfficialNavVerify] " + MarkerFail + " " + detail);
            }

            WriteFlag(pass ? "pass" : "fail");
            ScheduleExit(pass ? 0 : 1);
        }

        private static void FailAndExit(string detail)
        {
            Append(MarkerFail + " " + detail);
            Debug.LogError("[OfficialNavVerify] " + MarkerFail + " " + detail);
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
                Debug.LogError("[OfficialNavVerify] flag write failed: " + ex.Message);
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
                Debug.LogError("[OfficialNavVerify] log write failed: " + ex.Message);
            }
        }

        private sealed class OfficialNavVerifyRunner : MonoBehaviour
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

                AppendStatic("Runner start activeScene=" + SceneManager.GetActiveScene().name);

                bool reachedIntroOrMenu = false;
                yield return WaitForAnyScene(new[] { "Intro", "MainMenu" }, 15f, v => reachedIntroOrMenu = v);
                if (!reachedIntroOrMenu)
                {
                    _report(false, "Did not reach Intro|MainMenu from Boot. active="
                        + SceneManager.GetActiveScene().name);
                    yield break;
                }

                AppendStatic("After Boot handoff: " + SceneManager.GetActiveScene().name);

                bool reachedMainMenu = false;
                yield return WaitForAnyScene(new[] { "MainMenu" }, 20f, v => reachedMainMenu = v);
                if (!reachedMainMenu)
                {
                    _report(false, "MainMenu not reached after Intro. active="
                        + SceneManager.GetActiveScene().name);
                    yield break;
                }

                AppendStatic("MainMenu active — clicking Play Now");

                GameObject playGo = GameObject.Find("PlayButtonImage");
                if (playGo == null)
                {
                    _report(false, "PlayButtonImage missing on MainMenu.");
                    yield break;
                }

                Button playButton = playGo.GetComponent<Button>();
                MainMenuPlayButton playHook = playGo.GetComponent<MainMenuPlayButton>();
                if (playButton == null || playHook == null)
                {
                    _report(false, "PlayButtonImage missing Button/MainMenuPlayButton.");
                    yield break;
                }

                playButton.onClick.Invoke();
                yield return null;
                yield return null;

                if (SceneManager.GetActiveScene().name != "PlayMenu")
                {
                    playHook.OnPlayClicked();
                    yield return null;
                    yield return null;
                }

                if (SceneManager.GetActiveScene().name != "PlayMenu")
                {
                    _report(false, "PlayMenu not active after Play Now. active="
                        + SceneManager.GetActiveScene().name);
                    yield break;
                }

                AppendStatic("PlayMenu active — clicking Quick Play");
                if (!ClickQuickPlay())
                {
                    _report(false, "Could not invoke Quick Play from PlayMenu.");
                    yield break;
                }

                yield return null;
                yield return null;

                bool enteredQuickPath = false;
                yield return WaitForAnyScene(new[] { "QuickPlay", "LoadingScreen", "Gameplay" }, 8f, v => enteredQuickPath = v);
                if (!enteredQuickPath)
                {
                    _report(false, "Did not enter QuickPlay/Loading/Gameplay. active="
                        + SceneManager.GetActiveScene().name);
                    yield break;
                }

                AppendStatic("After Quick Play click: " + SceneManager.GetActiveScene().name);

                bool reachedGameplay = false;
                yield return WaitForAnyScene(new[] { "Gameplay" }, 20f, v => reachedGameplay = v);
                if (!reachedGameplay)
                {
                    _report(false, "Gameplay not reached. active="
                        + SceneManager.GetActiveScene().name
                        + " path=" + SceneManager.GetActiveScene().path);
                    yield break;
                }

                string path = SceneManager.GetActiveScene().path;
                AppendStatic("Gameplay reached path=" + path);
                _report(true, "Official flow OK: Boot→Intro→MainMenu→PlayMenu→QuickPlay→LoadingScreen→Gameplay. scenePath="
                    + path);
            }

            private static IEnumerator WaitForAnyScene(string[] names, float timeoutSeconds, Action<bool> done)
            {
                float deadline = Time.realtimeSinceStartup + timeoutSeconds;
                while (Time.realtimeSinceStartup < deadline)
                {
                    string current = SceneManager.GetActiveScene().name;
                    for (int i = 0; i < names.Length; i++)
                    {
                        if (current == names[i])
                        {
                            done(true);
                            yield break;
                        }
                    }

                    yield return null;
                }

                done(false);
            }

            private static bool ClickQuickPlay()
            {
                GameObject card = GameObject.Find("QuickPlayCard");
                if (card != null)
                {
                    Button b = card.GetComponent<Button>();
                    if (b == null)
                    {
                        b = card.GetComponentInChildren<Button>(true);
                    }

                    if (b != null)
                    {
                        b.onClick.Invoke();
                        return true;
                    }
                }

                Button[] buttons = UnityEngine.Object.FindObjectsOfType<Button>();
                for (int i = 0; i < buttons.Length; i++)
                {
                    if (buttons[i] != null
                        && buttons[i].gameObject.name.IndexOf("QuickPlay", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        buttons[i].onClick.Invoke();
                        return true;
                    }
                }

                AppendStatic("QuickPlay button not found — LoadScene(QuickPlay) after PlayMenu reached.");
                SceneManager.LoadScene("QuickPlay");
                return true;
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
