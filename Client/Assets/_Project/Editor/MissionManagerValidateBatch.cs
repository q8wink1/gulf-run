using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GulfRun.Domain;
using GulfRun.Features.Progression.Configuration;
using GulfRun.Features.Progression.Missions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GulfRun.Editor
{
    /// <summary>
    /// Batchmode validator for MissionManager daily-mission generation:
    /// - Boot scene pool wiring
    /// - runtime default catalog when data is missing
    /// - generation succeeds without NullReferenceException
    /// </summary>
    public static class MissionManagerValidateBatch
    {
        private const string BootScenePath = "Assets/_Project/Scenes/Boot.unity";
        private const string ReportPath = "MissionManagerValidateReport.txt";

        [MenuItem("GulfRun/Validate Mission Manager")]
        public static void ValidateFromMenu()
        {
            int code = Run(logToConsole: true);
            if (code != 0)
            {
                Debug.LogError("MissionManager validation FAILED. See " + ReportPath);
            }
            else
            {
                Debug.Log("MissionManager validation PASSED. See " + ReportPath);
            }
        }

        // Unity -batchmode -quit -projectPath ... -executeMethod GulfRun.Editor.MissionManagerValidateBatch.RunBatch
        public static void RunBatch()
        {
            int code = Run(logToConsole: true);
            EditorApplication.Exit(code);
        }

        private static int Run(bool logToConsole)
        {
            var log = new StringBuilder();
            int failures = 0;
            int errors = 0;

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

            Application.LogCallback logCallback = (condition, stackTrace, type) =>
            {
                if (type != LogType.Error && type != LogType.Exception)
                {
                    return;
                }

                // Ignore this validator's own Fail() LogErrors.
                if (!string.IsNullOrEmpty(condition) && condition.StartsWith("FAIL:", StringComparison.Ordinal))
                {
                    return;
                }

                errors++;
                log.AppendLine("CONSOLE ERROR: " + condition);
                if (!string.IsNullOrEmpty(stackTrace))
                {
                    log.AppendLine(stackTrace);
                }
            };

            Info("=== MissionManager Validation ===");
            Info("Time: " + DateTime.UtcNow.ToString("o"));

            Application.logMessageReceived += logCallback;
            try
            {
                // Empty scene first so Singleton.Awake is not fighting Boot's MissionManager.
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

                MissionPoolCatalogConfig defaults = MissionPoolCatalogConfig.CreateDefault();
                if (defaults == null || defaults.Missions.Count == 0)
                {
                    Fail("CreateDefault() produced an empty mission pool.");
                }
                else
                {
                    Info("CreateDefault mission count: " + defaults.Missions.Count);
                }

                // Simulate the GenerateDailyMissions path that previously NRE'd on null _random.
                IRandomSource random = SeededRandom.FromTime();
                var source = new List<MissionPoolCatalogConfig.MissionPoolEntry>(defaults.Missions);
                int generated = 0;
                int count = Math.Min(3, source.Count);
                for (int i = 0; i < count; i++)
                {
                    int pick = random.NextInt(0, source.Count);
                    MissionPoolCatalogConfig.MissionPoolEntry entry = source[pick];
                    source.RemoveAt(pick);
                    if (entry == null)
                    {
                        Fail("Default pool contained a null MissionPoolEntry.");
                        break;
                    }

                    generated++;
                    Info("Generated sample: " + entry.DisplayName);
                }

                if (generated != 3)
                {
                    Fail("Expected 3 sample missions, got " + generated);
                }

                var host = new GameObject("MissionManagerValidateHost");
                try
                {
                    MissionManager runtime = host.AddComponent<MissionManager>();
                    SerializedObject runtimeSo = new SerializedObject(runtime);
                    runtimeSo.FindProperty("pool").objectReferenceValue = null;
                    runtimeSo.ApplyModifiedPropertiesWithoutUndo();

                    // Clear non-serialized RNG to reproduce domain-reload nulling, then ensure regeneration.
                    var randomField = typeof(MissionManager).GetField("_random", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    if (randomField == null)
                    {
                        Fail("Could not find MissionManager._random via reflection.");
                    }
                    else
                    {
                        randomField.SetValue(runtime, null);
                    }

                    var ensure = typeof(MissionManager).GetMethod("EnsureMissionsFresh", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    if (ensure == null)
                    {
                        Fail("Could not find EnsureMissionsFresh via reflection.");
                    }
                    else
                    {
                        ensure.Invoke(runtime, null);
                        if (runtime.Pool == null || runtime.Pool.Missions.Count == 0)
                        {
                            Fail("EnsureMissionsFresh did not install a default pool.");
                        }
                        else if (runtime.ActiveMissions == null || runtime.ActiveMissions.Count == 0)
                        {
                            Fail("EnsureMissionsFresh did not generate active missions.");
                        }
                        else
                        {
                            Info("Runtime default generation produced " + runtime.ActiveMissions.Count + " active missions after null _random.");
                        }
                    }
                }
                finally
                {
                    UnityEngine.Object.DestroyImmediate(host);
                }

                Scene boot = EditorSceneManager.OpenScene(BootScenePath, OpenSceneMode.Single);
                if (!boot.IsValid())
                {
                    Fail("Could not open " + BootScenePath);
                }
                else
                {
                    MissionManager manager = UnityEngine.Object.FindObjectOfType<MissionManager>(true);
                    if (manager == null)
                    {
                        Fail("Boot scene has no MissionManager.");
                    }
                    else
                    {
                        SerializedObject so = new SerializedObject(manager);
                        SerializedProperty poolProp = so.FindProperty("pool");
                        if (poolProp == null || poolProp.objectReferenceValue == null)
                        {
                            Fail("MissionManager.pool is not assigned in Boot.unity.");
                        }
                        else
                        {
                            var assigned = poolProp.objectReferenceValue as MissionPoolCatalogConfig;
                            Info("Boot MissionManager.pool: " + assigned.name + " entries=" + assigned.Missions.Count);
                            if (assigned.Missions.Count == 0)
                            {
                                Fail("Assigned MissionPoolCatalogConfig has zero entries.");
                            }
                        }
                    }
                }

                if (errors > 0)
                {
                    Fail("Captured " + errors + " console error(s)/exception(s) during validation.");
                }
            }
            catch (Exception ex)
            {
                Fail("Unhandled exception: " + ex);
            }
            finally
            {
                Application.logMessageReceived -= logCallback;
            }

            Info("Failures: " + failures);
            Info(failures == 0 ? "RESULT: PASS" : "RESULT: FAIL");
            WriteReport(log);
            return failures == 0 ? 0 : 1;
        }

        private static void WriteReport(StringBuilder log)
        {
            string path = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ReportPath));
            File.WriteAllText(path, log.ToString());
            Debug.Log("Wrote " + path);
        }
    }
}
