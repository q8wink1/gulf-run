using GulfRun.Core.Services;
using UnityEngine;

namespace GulfRun.Core.Managers
{
    /// <summary>
    /// Top-level orchestrator for Project GulfRun. Owns the application lifecycle
    /// and coordinates the other core managers. No gameplay logic is implemented
    /// in this sprint; this is a production-ready empty foundation.
    /// References: P001 (Game Vision), P002 (Core Gameplay Loop),
    /// P003 (Core Gameplay Design), P050 (Master Design Bible).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class GameManager : Singleton<GameManager>
    {
        [Tooltip("Target frame rate applied on startup (Sprint 1: 60 FPS target).")]
        [SerializeField] private int _targetFrameRate = 60;

        protected override void OnInitialize()
        {
            ApplyPerformanceTargets();

            // Boot only initializes services — never Quick Play / Loading / Gameplay.
            // Clear any leftover offline race flag (e.g. DisableDomainReload verify runs).
            OfflineRaceEntryService.Clear();

            // TODO(Sprint 2+): Bootstrap game state machine and coordinate
            // manager startup order once gameplay systems are specified.
        }

        private void Start()
        {
            // Deferred to Start() (rather than OnInitialize/Awake) so every other
            // CoreSystems singleton on this GameObject has finished its own Awake
            // before SceneManager.Instance is dereferenced. Only ever runs once:
            // duplicate GameManager instances self-destroy in Awake before Start.
            // Official flow: Boot → Intro → Main Menu (Intro loads MainMenu).
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == BootSceneName)
            {
                if (SceneManager.Instance != null)
                {
                    SceneManager.Instance.LoadIntro();
                }
                else
                {
                    Debug.LogError("[GameManager] SceneManager.Instance missing on Boot — loading Intro directly.");
                    UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.IntroSceneName);
                }
            }
        }

        private const string BootSceneName = "Boot";

        private void ApplyPerformanceTargets()
        {
            // Uncapped by vSync so Application.targetFrameRate can govern pacing;
            // see P046 Performance Optimization Specification (60 FPS target, 30 FPS minimum).
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = _targetFrameRate;
        }
    }
}
