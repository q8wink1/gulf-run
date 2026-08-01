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

            // TODO(Sprint 2+): Bootstrap game state machine and coordinate
            // manager startup order once gameplay systems are specified.
        }

        private void Start()
        {
            // Deferred to Start() (rather than OnInitialize/Awake) so every other
            // CoreSystems singleton on this GameObject has finished its own Awake
            // before SceneManager.Instance is dereferenced. Only ever runs once:
            // duplicate GameManager instances self-destroy in Awake before Start.
            // Sprint 14: Boot now always hands off to the Brand Intro first —
            // the Intro scene itself is responsible for loading MainMenu once
            // it finishes or is skipped (see IntroSequenceController).
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == BootSceneName)
            {
                SceneManager.Instance?.LoadIntro();
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
