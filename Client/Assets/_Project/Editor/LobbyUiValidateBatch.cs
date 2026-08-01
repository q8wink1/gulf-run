using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GulfRun.Features.Matchmaking.Lobby;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GulfRun.Editor
{
    /// <summary>
    /// Batchmode validator for Lobby UI repair:
    /// - inventories active Canvases / OnGUI drawers
    /// - asserts LobbyCanvas CanvasScaler (1920×1080, match 0.5)
    /// - checks production Lobby OnGUI rects fit 1080p / 1440p / 4K
    /// </summary>
    public static class LobbyUiValidateBatch
    {
        private const string LobbyScenePath = "Assets/_Project/Scenes/Lobby.unity";
        private const string ReportPath = "LobbyUiValidateReport.txt";

        private static readonly Vector2Int[] Resolutions =
        {
            new Vector2Int(1920, 1080),
            new Vector2Int(2560, 1440),
            new Vector2Int(3840, 2160)
        };

        [MenuItem("GulfRun/Validate Lobby UI")]
        public static void ValidateFromMenu()
        {
            int code = Run(logToConsole: true);
            if (code != 0)
            {
                Debug.LogError("Lobby UI validation FAILED. See " + ReportPath);
            }
            else
            {
                Debug.Log("Lobby UI validation PASSED. See " + ReportPath);
            }
        }

        // Unity -batchmode -quit -projectPath ... -executeMethod GulfRun.Editor.LobbyUiValidateBatch.RunBatch
        public static void RunBatch()
        {
            int code = Run(logToConsole: true);
            EditorApplication.Exit(code);
        }

        private static int Run(bool logToConsole)
        {
            var log = new StringBuilder();
            int failures = 0;

            void Info(string msg)
            {
                log.AppendLine(msg);
                if (logToConsole)
                {
                    Debug.Log(msg);
                }
            }

            void Fail(string msg)
            {
                failures++;
                log.AppendLine("FAIL: " + msg);
                if (logToConsole)
                {
                    Debug.LogError("FAIL: " + msg);
                }
            }

            Info("=== Lobby UI Validation ===");
            Info("Time: " + DateTime.UtcNow.ToString("o"));

            Scene scene = EditorSceneManager.OpenScene(LobbyScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                Fail("Could not open " + LobbyScenePath);
                WriteReport(log);
                return 2;
            }

            Info("Opened scene: " + scene.path);

            var canvases = UnityEngine.Object.FindObjectsOfType<Canvas>(true);
            Info("Canvas count (incl. inactive): " + canvases.Length);
            foreach (Canvas canvas in canvases)
            {
                Info("  Canvas: " + GetPath(canvas.gameObject) + " active=" + canvas.gameObject.activeInHierarchy +
                     " enabled=" + canvas.enabled + " mode=" + canvas.renderMode);
            }

            if (canvases.Length == 0)
            {
                Fail("Expected at least one LobbyCanvas.");
            }

            LobbyCanvasRoot root = UnityEngine.Object.FindObjectOfType<LobbyCanvasRoot>(true);
            if (root == null)
            {
                Fail("LobbyCanvasRoot missing.");
            }
            else
            {
                root.EnsureLayout();
                CanvasScaler scaler = root.GetComponent<CanvasScaler>();
                if (scaler == null)
                {
                    Fail("CanvasScaler missing on LobbyCanvasRoot.");
                }
                else
                {
                    if (scaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
                    {
                        Fail("CanvasScaler.uiScaleMode expected ScaleWithScreenSize, got " + scaler.uiScaleMode);
                    }

                    if (!Approximately(scaler.referenceResolution.x, LobbyCanvasRoot.ReferenceWidth) ||
                        !Approximately(scaler.referenceResolution.y, LobbyCanvasRoot.ReferenceHeight))
                    {
                        Fail("CanvasScaler.referenceResolution expected 1920x1080, got " + scaler.referenceResolution);
                    }

                    if (!Approximately(scaler.matchWidthOrHeight, LobbyCanvasRoot.Match))
                    {
                        Fail("CanvasScaler.matchWidthOrHeight expected 0.5, got " + scaler.matchWidthOrHeight);
                    }

                    Info("CanvasScaler OK: ScaleWithScreenSize 1920x1080 match=0.5");
                }

                RectTransform rt = root.transform as RectTransform;
                if (rt != null)
                {
                    if (rt.anchorMin != Vector2.zero || rt.anchorMax != Vector2.one)
                    {
                        Fail("LobbyCanvas RectTransform anchors expected stretch (0,0)-(1,1).");
                    }
                    else
                    {
                        Info("LobbyCanvas RectTransform anchors OK (stretch).");
                    }
                }
            }

            MatchmakingDebugViewState(log, Fail, Info);

            Info("--- Active root GameObjects ---");
            foreach (GameObject go in scene.GetRootGameObjects())
            {
                Info("  root: " + go.name + " active=" + go.activeSelf);
                foreach (MonoBehaviour mb in go.GetComponents<MonoBehaviour>())
                {
                    if (mb == null)
                    {
                        continue;
                    }

                    Info("    component: " + mb.GetType().Name + " enabled=" + mb.enabled);
                }
            }

            Info("--- Production Lobby OnGUI bounds check ---");
            foreach (Vector2Int res in Resolutions)
            {
                failures += ValidateLobbyRectsAt(res.x, res.y, Info, Fail);
            }

            Info("=== Result: " + (failures == 0 ? "PASS" : "FAIL (" + failures + ")") + " ===");
            WriteReport(log);
            return failures == 0 ? 0 : 1;
        }

        private static void MatchmakingDebugViewState(StringBuilder log, Action<string> fail, Action<string> info)
        {
            // Avoid hard asm dependency on MatchmakingDebugView type from optional ifdef.
            MonoBehaviour[] behaviours = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>(true);
            bool found = false;
            foreach (MonoBehaviour mb in behaviours)
            {
                if (mb == null || mb.GetType().Name != "MatchmakingDebugView")
                {
                    continue;
                }

                found = true;
                info("MatchmakingDebugView present enabled=" + mb.enabled + " goActive=" + mb.gameObject.activeInHierarchy);
                if (mb.enabled && mb.gameObject.activeInHierarchy)
                {
                    fail("MatchmakingDebugView should be disabled in Lobby.unity for production.");
                }

                SerializedObject so = new SerializedObject(mb);
                SerializedProperty show = so.FindProperty("showOnScreenDebug");
                if (show != null && show.boolValue)
                {
                    fail("MatchmakingDebugView.showOnScreenDebug should be false in Lobby.");
                }
            }

            if (!found)
            {
                info("MatchmakingDebugView not found (acceptable if removed).");
            }

            _ = log;
        }

        private static int ValidateLobbyRectsAt(int width, int height, Action<string> info, Action<string> fail)
        {
            int failures = 0;
            var rects = new List<(string name, Rect rect)>
            {
                ("OwnerControls", new Rect((width - Mathf.Clamp(width - 48f, 320f, 520f)) * 0.5f, 40f, Mathf.Clamp(width - 48f, 320f, 520f), 110f)),
                ("PlayerCards", new Rect(24f, height * 0.30f, width - 48f, 210f)),
                ("QuickChat", new Rect(16f, height - 210f, 250f, 100f)),
                ("QuickChatFeed", new Rect(16f, height - 288f, 250f, 72f)),
                ("ReadyControls", new Rect((width - 280f) * 0.5f, height - 96f, 280f, 40f)),
                ("VoiceWidget", new Rect(width - 150f - 16f, height - 36f - 16f, 150f, 36f)),
                ("MatchFoundPopup", new Rect((width - Mathf.Min(420f, width - 40f)) * 0.5f, height * 0.18f, Mathf.Min(420f, width - 40f), 120f))
            };

            info("Resolution " + width + "x" + height + ":");
            foreach ((string name, Rect rect) in rects)
            {
                bool inside = rect.xMin >= -0.5f && rect.yMin >= -0.5f &&
                              rect.xMax <= width + 0.5f && rect.yMax <= height + 0.5f;
                info("  " + name + " " + RectToString(rect) + (inside ? " OK" : " OUT OF BOUNDS"));
                if (!inside)
                {
                    fail(name + " outside " + width + "x" + height + ": " + RectToString(rect));
                    failures++;
                }
            }

            return failures;
        }

        private static string RectToString(Rect r) =>
            string.Format("[x={0:0.#} y={1:0.#} w={2:0.#} h={3:0.#}]", r.x, r.y, r.width, r.height);

        private static bool Approximately(float a, float b) => Mathf.Abs(a - b) < 0.001f;

        private static string GetPath(GameObject go)
        {
            string path = go.name;
            Transform t = go.transform.parent;
            while (t != null)
            {
                path = t.name + "/" + path;
                t = t.parent;
            }

            return path;
        }

        private static void WriteReport(StringBuilder log)
        {
            string path = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ReportPath));
            File.WriteAllText(path, log.ToString());
            Debug.Log("Wrote " + path);
        }
    }
}
