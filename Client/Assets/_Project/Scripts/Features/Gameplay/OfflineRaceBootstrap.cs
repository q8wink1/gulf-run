using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.CameraSystem;
using GulfRun.Features.GameplayHud;
using UnityEngine;

namespace GulfRun.Features.Gameplay
{
    /// <summary>
    /// Sprint 23.13 — Gameplay enter bootstrap for the offline Quick Play
    /// prototype. Ensures RunnerPlayer / camera / spawn pools are ready,
    /// calls <see cref="RaceManager.StartRace"/>, publishes race progress
    /// for finish-line systems, and marks the local stub match Running.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class OfflineRaceBootstrap : MonoBehaviour, IRaceProgressProvider
    {
        [Header("System References")]
        [SerializeField] private RaceManager raceManager;
        [SerializeField] private RunnerPlayerController playerController;
        [SerializeField] private RunnerCameraFollow cameraFollow;
        [SerializeField] private SpawnManager spawnManager;
        [SerializeField] private EndlessTrackGenerator trackGenerator;

        [Header("Behaviour")]
        [Tooltip("Always start the race when Gameplay loads while Waiting.")]
        [SerializeField] private bool autoStartRace = true;

        private float _startZ;
        private bool _progressRegistered;

        double IRaceProgressProvider.DistanceMeters
        {
            get
            {
                if (playerController == null)
                {
                    return 0d;
                }

                return Mathf.Max(0f, playerController.transform.position.z - _startZ);
            }
        }

        int IRaceProgressProvider.CoinsCollected
        {
            get
            {
                GameplayHudController hud = GameplayHudController.Instance;
                return hud != null ? hud.Coins : 0;
            }
        }

        private void Awake()
        {
            ResolveReferences();
        }

        private void Start()
        {
            ResolveReferences();
            EnsurePlayerReady();
            EnsureCameraTarget();
            WarmSpawnPools();

            if (playerController != null)
            {
                _startZ = playerController.transform.position.z;
            }

            RegisterProgressProvider();

            if (OfflineRaceEntryService.IsActive)
            {
                MatchLobbySummaryService.Current?.MarkOfflineRaceRunning();
            }

            if (autoStartRace && raceManager != null &&
                (raceManager.CurrentState == RaceState.Waiting ||
                 raceManager.CurrentState == RaceState.Countdown))
            {
                raceManager.StartRace();
            }
        }

        private void OnDestroy()
        {
            if (_progressRegistered && ReferenceEquals(RaceProgressService.Current, this))
            {
                RaceProgressService.Current = null;
            }
        }

        private void ResolveReferences()
        {
            if (raceManager == null)
            {
                raceManager = RaceManager.Instance;
            }

            if (raceManager != null)
            {
                if (playerController == null)
                {
                    playerController = raceManager.PlayerController;
                }

                if (cameraFollow == null)
                {
                    cameraFollow = raceManager.CameraController;
                }

                if (spawnManager == null)
                {
                    spawnManager = raceManager.SpawnManager;
                }

                if (trackGenerator == null)
                {
                    trackGenerator = raceManager.TrackGenerator;
                }
            }

            if (playerController == null)
            {
                GameObject playerGo = GameObject.FindGameObjectWithTag("Player");
                if (playerGo != null)
                {
                    playerController = playerGo.GetComponent<RunnerPlayerController>();
                }
            }

            if (spawnManager == null)
            {
                spawnManager = SpawnManager.Instance;
            }

            if (trackGenerator == null && raceManager != null)
            {
                trackGenerator = raceManager.TrackGenerator;
            }
        }

        private void EnsurePlayerReady()
        {
            if (playerController == null)
            {
                return;
            }

            if (!playerController.gameObject.activeInHierarchy)
            {
                playerController.gameObject.SetActive(true);
            }

            if (!playerController.enabled)
            {
                playerController.enabled = true;
            }
        }

        private void EnsureCameraTarget()
        {
            if (cameraFollow == null || playerController == null)
            {
                return;
            }

            if (!cameraFollow.enabled)
            {
                cameraFollow.enabled = true;
            }

            // RunnerCameraFollow already binds target in its own Start; ensure
            // follow target is set when the player exists.
            Transform follow = playerController.FollowTarget;
            if (follow != null)
            {
                cameraFollow.Target = follow;
            }
        }

        private void WarmSpawnPools()
        {
            if (spawnManager == null)
            {
                return;
            }

            spawnManager.WarmPools(spawnManager.transform);
        }

        private void RegisterProgressProvider()
        {
            RaceProgressService.Current = this;
            _progressRegistered = true;
        }
    }
}
