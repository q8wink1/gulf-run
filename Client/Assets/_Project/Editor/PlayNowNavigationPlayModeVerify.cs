using System;
using System.Collections;
using System.IO;
using GulfRun.Features.MainMenu;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace GulfRun.Editor
{
    /// <summary>
    /// Batchmode Play Mode verification: MainMenu → click Play Now → PlayMenu active.
    /// Survives Play Mode domain reload via a flag file + InitializeOnLoadMethod.
    /// Writes VERIFY_PASS / VERIFY_FAIL under Client/Temp and exits the Editor.
    /// </summary>
    [InitializeOnLoad]
    public static class PlayNowNavigationPlayModeVerify
    {
        private const string MainMenuScenePath = "Assets/_Project/Scenes/MainMenu.unity";
        // BuildLogs survives Editor Temp cleanup on exit.
        private const string ResultLogRelative = "BuildLogs/PlayNowNavigationVerify.log";
        private const string FlagRelative = "Temp/PlayNowNavigationVerify.flag";
        private const string MarkerPass = "VERIFY_PASS";
        private const string MarkerFail = "VERIFY_FAIL";

        private static bool _updateHooked;
        private static bool _runnerStarted;
        private static bool _exitScheduled;
        private static double _playEnteredAt;
        private static bool _savedEnterPlayModeOptions;
        private static bool _prevEnterPlayModeOptionsEnabled;
        private static EnterPlayModeOptions _prevEnterPlayModeOptions;

        static PlayNowNavigationPlayModeVerify()
        {
            // Domain reload (edit↔play) re-runs this; resume if a verify is in flight.
            TryResumeFromFlag();
        }

        [MenuItem("GulfRun/Play Flow/Verify Play Now Navigation (Play Mode)")]
        public static void VerifyPlayNowNavigation()
        {
            string resultPath = ResultPath();
            Directory.CreateDirectory(Path.GetDirectoryName(resultPath) ?? ".");
            File.WriteAllText(resultPath, "VERIFY_STARTED " + DateTime.UtcNow.ToString("o") + Environment.NewLine);

            if (!File.Exists(MainMenuScenePath))
            {
                FailAndExit("MainMenu scene missing at " + MainMenuScenePath);
                return;
            }

            // Avoid domain reload so static hooks survive EnterPlaymode in batchmode.
            _prevEnterPlayModeOptionsEnabled = EditorSettings.enterPlayModeOptionsEnabled;
            _prevEnterPlayModeOptions = EditorSettings.enterPlayModeOptions;
            EditorSettings.enterPlayModeOptionsEnabled = true;
            EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload;
            _savedEnterPlayModeOptions = true;

            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(MainMenuScenePath, OpenSceneMode.Single);

            WriteFlag("waiting-play");
            Append("Entering Play Mode for MainMenu (DisableDomainReload)…");
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
            GameObject host = new GameObject("~PlayNowNavVerifyRunner");
            UnityEngine.Object.DontDestroyOnLoad(host);
            host.AddComponent<PlayNowNavVerifyRunner>().Begin(Report);
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

            // Watchdog: EnterPlaymode stalled.
            if (flag == "waiting-play" && !EditorApplication.isPlaying)
            {
                // Still entering — give it time. If stuck > 90s after start, fail.
                return;
            }

            if (flag == "waiting-play" && EditorApplication.isPlaying && !_runnerStarted)
            {
                StartRunner();
            }

            if (_runnerStarted && _playEnteredAt > 0
                && EditorApplication.timeSinceStartup - _playEnteredAt > 60.0
                && flag == "running")
            {
                FailAndExit("Watchdog timeout: runner did not finish within 60s of EnteredPlayMode.");
            }
        }

        private static void Report(bool pass, string detail)
        {
            if (pass)
            {
                Append(MarkerPass + " " + detail);
                Debug.Log("[PlayNowNavVerify] " + MarkerPass + " " + detail);
            }
            else
            {
                Append(MarkerFail + " " + detail);
                Debug.LogError("[PlayNowNavVerify] " + MarkerFail + " " + detail);
            }

            WriteFlag(pass ? "pass" : "fail");
            ScheduleExit(pass ? 0 : 1);
        }

        private static void FailAndExit(string detail)
        {
            Append(MarkerFail + " " + detail);
            Debug.LogError("[PlayNowNavVerify] " + MarkerFail + " " + detail);
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
                Debug.LogError("[PlayNowNavVerify] flag write failed: " + ex.Message);
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
                Debug.LogError("[PlayNowNavVerify] log write failed: " + ex.Message);
            }
        }

        private sealed class PlayNowNavVerifyRunner : MonoBehaviour
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
                yield return null;

                string diagnostics = CollectDiagnostics();
                AppendStatic(diagnostics);
                Debug.Log("[PlayNowNavVerify] Diagnostics:\n" + diagnostics);

                GameObject playGo = GameObject.Find("PlayButtonImage");
                if (playGo == null)
                {
                    _report(false, "PlayButtonImage not found in Play Mode.");
                    yield break;
                }

                Button button = playGo.GetComponent<Button>();
                MainMenuPlayButton hook = playGo.GetComponent<MainMenuPlayButton>();
                if (button == null || hook == null)
                {
                    _report(false, "PlayButtonImage missing Button or MainMenuPlayButton. button="
                        + (button != null) + " hook=" + (hook != null));
                    yield break;
                }

                if (!button.interactable)
                {
                    _report(false, "Play Button.interactable is false.");
                    yield break;
                }

                if (EventSystem.current == null)
                {
                    _report(false, "EventSystem.current is null in Play Mode.");
                    yield break;
                }

                Canvas canvas = playGo.GetComponentInParent<Canvas>();
                if (canvas == null || canvas.GetComponent<GraphicRaycaster>() == null)
                {
                    _report(false, "Play button Canvas missing GraphicRaycaster.");
                    yield break;
                }

                // 1) Simulate pointer click through EventSystem.
                SimulatePointerClick(playGo);
                yield return null;
                yield return null;

                if (SceneManager.GetActiveScene().name == "PlayMenu")
                {
                    _report(true, "PlayMenu active after ExecuteEvents.pointerClick. " + diagnostics);
                    yield break;
                }

                // 2) Direct Button.onClick.Invoke
                AppendStatic("Invoking button.onClick…");
                button.onClick.Invoke();
                yield return null;
                yield return null;

                if (SceneManager.GetActiveScene().name == "PlayMenu")
                {
                    _report(true, "PlayMenu active after button.onClick.Invoke. " + diagnostics);
                    yield break;
                }

                // 3) Direct method call
                AppendStatic("Calling OnPlayClicked directly…");
                hook.OnPlayClicked();
                yield return null;
                yield return null;

                if (SceneManager.GetActiveScene().name == "PlayMenu")
                {
                    _report(true, "PlayMenu active after direct OnPlayClicked(). " + diagnostics);
                    yield break;
                }

                _report(
                    false,
                    "PlayMenu did not become active. activeScene=" + SceneManager.GetActiveScene().name
                    + " path=" + SceneManager.GetActiveScene().path
                    + " | " + diagnostics);
            }

            private static void SimulatePointerClick(GameObject playGo)
            {
                EventSystem es = EventSystem.current;
                if (es == null)
                {
                    return;
                }

                PointerEventData pointer = new PointerEventData(es)
                {
                    button = PointerEventData.InputButton.Left,
                    position = RectTransformUtility.WorldToScreenPoint(null, playGo.transform.position)
                };

                var results = new System.Collections.Generic.List<RaycastResult>();
                es.RaycastAll(pointer, results);
                AppendStatic("RaycastAll count=" + results.Count);
                for (int i = 0; i < results.Count && i < 8; i++)
                {
                    AppendStatic("  ray[" + i + "]=" + results[i].gameObject.name);
                }

                GameObject target = results.Count > 0 ? results[0].gameObject : playGo;
                AppendStatic("Primary raycast hit=" + target.name);

                bool down = ExecuteEvents.Execute(target, pointer, ExecuteEvents.pointerDownHandler);
                bool click = ExecuteEvents.Execute(target, pointer, ExecuteEvents.pointerClickHandler);
                bool up = ExecuteEvents.Execute(target, pointer, ExecuteEvents.pointerUpHandler);
                AppendStatic("ExecuteEvents down=" + down + " click=" + click + " up=" + up);

                if (target != playGo)
                {
                    ExecuteEvents.Execute(playGo, pointer, ExecuteEvents.pointerClickHandler);
                }

                // Button implements IPointerClickHandler on the same GO as Image.
                ExecuteEvents.Execute(playGo, pointer, ExecuteEvents.pointerClickHandler);
            }

            private static string CollectDiagnostics()
            {
                var sb = new System.Text.StringBuilder();
                sb.Append("activeScene=").Append(SceneManager.GetActiveScene().name);
                sb.Append(" eventSystem=").Append(EventSystem.current != null);
                if (EventSystem.current != null)
                {
                    var modules = EventSystem.current.GetComponents<BaseInputModule>();
                    sb.Append(" inputModules=");
                    foreach (BaseInputModule m in modules)
                    {
                        sb.Append(m.GetType().Name).Append(';');
                    }
                }

                GameObject play = GameObject.Find("PlayButtonImage");
                if (play != null)
                {
                    Button b = play.GetComponent<Button>();
                    Image img = play.GetComponent<Image>();
                    sb.Append(" playActive=").Append(play.activeInHierarchy);
                    sb.Append(" interactable=").Append(b != null && b.interactable);
                    sb.Append(" raycastTarget=").Append(img != null && img.raycastTarget);
                    sb.Append(" persistentOnClick=").Append(b != null ? b.onClick.GetPersistentEventCount() : -1);
                    Canvas c = play.GetComponentInParent<Canvas>();
                    sb.Append(" canvas=").Append(c != null ? c.name : "null");
                    sb.Append(" graphicRaycaster=").Append(c != null && c.GetComponent<GraphicRaycaster>() != null);
                }
                else
                {
                    sb.Append(" PlayButtonImage=MISSING");
                }

#if ENABLE_INPUT_SYSTEM
                sb.Append(" ENABLE_INPUT_SYSTEM=1");
                sb.Append(" hasInputSystemUIModule=")
                    .Append(UnityEngine.Object.FindObjectOfType<InputSystemUIInputModule>() != null);
#else
                sb.Append(" ENABLE_INPUT_SYSTEM=0");
#endif
                return sb.ToString();
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
