using System;
using GulfRun.Core;
using GulfRun.Domain;
using GulfRun.Features.CameraSystem;
using GulfRun.Features.GameplayHud;
using UnityEngine;
using UnityEngine.Events;

namespace GulfRun.Features.Gameplay
{
    /// <summary>
    /// Sprint 23.8 — central race flow coordinator. Owns session state,
    /// speed targets, and system references so Features can subscribe without
    /// owning finish / obstacle / coin / multiplayer logic yet.
    /// Starts in <see cref="RaceState.Waiting"/>; callers invoke
    /// <see cref="BeginCountdown"/> / <see cref="StartRace"/> later.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class RaceManager : SceneSingleton<RaceManager>
    {
        [Header("System References")]
        [SerializeField] private RunnerPlayerController playerController;
        [SerializeField] private RunnerCameraFollow cameraController;
        [SerializeField] private EndlessTrackGenerator trackGenerator;
        [SerializeField] private SpawnManager spawnManager;
        [SerializeField] private GameplayHudController hud;

        [Header("Speed (future progressive run)")]
        [Tooltip("Baseline race speed used for TargetSpeed reset and player scale.")]
        [SerializeField] private float initialSpeed = 12f;
        [SerializeField] private float maximumSpeed = 28f;
        [Tooltip("Units per second CurrentSpeed rises while Running (uncapped by distance).")]
        [SerializeField] private float speedIncreaseRate = 0.35f;
        [Tooltip("Target finish distance — reserved; no auto-finish this sprint.")]
        [SerializeField] private float raceDistance = 1000f;
        [Tooltip("When true, Running speed syncs to RunnerPlayerController.SetSpeedScale.")]
        [SerializeField] private bool applySpeedToPlayer;

        [Header("Inspector Events (optional)")]
        [SerializeField] private UnityEvent onRaceStart;
        [SerializeField] private UnityEvent onRacePause;
        [SerializeField] private UnityEvent onRaceResume;
        [SerializeField] private UnityEvent onRaceFinish;

        private RaceState _state = RaceState.Waiting;
        private bool _isPaused;
        private float _currentSpeed;
        private float _targetSpeed;
        private float _savedTimeScale = 1f;

        public RaceState CurrentState => _state;
        public bool IsPaused => _isPaused;
        public bool IsRacing => _state == RaceState.Running && !_isPaused;

        public float InitialSpeed => initialSpeed;
        public float MaximumSpeed => maximumSpeed;
        public float SpeedIncreaseRate => speedIncreaseRate;
        public float RaceDistance => raceDistance;

        /// <summary>Desired run speed (clamped). Updated by ramp / <see cref="SetRunningSpeed"/>.</summary>
        public float TargetSpeed => _targetSpeed;

        /// <summary>Live speed used by future systems; ramps toward <see cref="TargetSpeed"/>.</summary>
        public float CurrentSpeed => _currentSpeed;

        public RunnerPlayerController PlayerController => playerController;
        public RunnerCameraFollow CameraController => cameraController;
        public EndlessTrackGenerator TrackGenerator => trackGenerator;
        public SpawnManager SpawnManager => spawnManager;
        public GameplayHudController Hud => hud;

        /// <summary>C# event for Features subscribers (preferred over UnityEvent).</summary>
        public event Action OnRaceStart;

        public event Action OnRacePause;
        public event Action OnRaceResume;
        public event Action OnRaceFinish;

        /// <summary>Raised after every successful state change (Waiting/Countdown/Running/Finished).</summary>
        public event Action<RaceState> StateChanged;

        protected override void Awake()
        {
            base.Awake();
            _targetSpeed = Mathf.Max(0f, initialSpeed);
            _currentSpeed = 0f;
            _state = RaceState.Waiting;
            _isPaused = false;
        }

        private void Update()
        {
            if (_state != RaceState.Running || _isPaused)
            {
                return;
            }

            TickSpeed(Time.deltaTime);
        }

        /// <summary>Waiting → Countdown. Does not start the race.</summary>
        public void BeginCountdown()
        {
            if (_state != RaceState.Waiting)
            {
                return;
            }

            SetState(RaceState.Countdown);
        }

        /// <summary>
        /// Countdown → Running (also accepts Waiting for skip / tools).
        /// Fires <see cref="OnRaceStart"/>. Does not auto-finish.
        /// </summary>
        public void StartRace()
        {
            if (_state != RaceState.Countdown && _state != RaceState.Waiting)
            {
                return;
            }

            _isPaused = false;
            _targetSpeed = Mathf.Clamp(initialSpeed, 0f, maximumSpeed);
            _currentSpeed = _targetSpeed;
            EnsureTimeScaleRunning();
            SetState(RaceState.Running);
            TryApplySpeedToPlayer();
            RaiseStart();
        }

        /// <summary>Running → paused flag + timeScale 0. State stays Running.</summary>
        public void PauseRace()
        {
            if (_state != RaceState.Running || _isPaused)
            {
                return;
            }

            _isPaused = true;
            _savedTimeScale = Time.timeScale > 0f ? Time.timeScale : 1f;
            Time.timeScale = 0f;
            RaisePause();
        }

        /// <summary>Clears pause and restores timeScale.</summary>
        public void ResumeRace()
        {
            if (_state != RaceState.Running || !_isPaused)
            {
                return;
            }

            _isPaused = false;
            Time.timeScale = _savedTimeScale > 0f ? _savedTimeScale : 1f;
            RaiseResume();
        }

        /// <summary>
        /// Running → Finished. Caller-driven only — no distance / obstacle finish.
        /// </summary>
        public void FinishRace()
        {
            if (_state != RaceState.Running)
            {
                return;
            }

            if (_isPaused)
            {
                _isPaused = false;
                EnsureTimeScaleRunning();
            }

            SetState(RaceState.Finished);
            RaiseFinish();
        }

        /// <summary>
        /// Sets the desired run speed while preparing / racing.
        /// Clamped to [0, MaximumSpeed]. Does not finish the race.
        /// </summary>
        public void SetRunningSpeed(float speed)
        {
            _targetSpeed = Mathf.Clamp(speed, 0f, Mathf.Max(0f, maximumSpeed));
            if (_state == RaceState.Running && !_isPaused)
            {
                _currentSpeed = _targetSpeed;
                TryApplySpeedToPlayer();
            }
        }

        /// <summary>
        /// Maps <see cref="CurrentSpeed"/> onto <see cref="RunnerPlayerController.SetSpeedScale"/>
        /// using <see cref="InitialSpeed"/> as 1.0. No-op when player unset or initial is 0.
        /// </summary>
        public void ApplySpeedToPlayer()
        {
            if (playerController == null || initialSpeed <= 0.0001f)
            {
                return;
            }

            playerController.SetSpeedScale(_currentSpeed / initialSpeed);
        }

        private void TickSpeed(float deltaTime)
        {
            if (speedIncreaseRate > 0f && _targetSpeed < maximumSpeed)
            {
                _targetSpeed = Mathf.Min(maximumSpeed, _targetSpeed + (speedIncreaseRate * deltaTime));
            }

            _currentSpeed = _targetSpeed;
            TryApplySpeedToPlayer();
        }

        private void TryApplySpeedToPlayer()
        {
            if (!applySpeedToPlayer)
            {
                return;
            }

            ApplySpeedToPlayer();
        }

        private void EnsureTimeScaleRunning()
        {
            if (Time.timeScale <= 0f)
            {
                Time.timeScale = _savedTimeScale > 0f ? _savedTimeScale : 1f;
            }
        }

        private void SetState(RaceState newState)
        {
            if (newState == _state)
            {
                return;
            }

            _state = newState;
            StateChanged?.Invoke(newState);
        }

        private void RaiseStart()
        {
            OnRaceStart?.Invoke();
            if (onRaceStart != null)
            {
                onRaceStart.Invoke();
            }
        }

        private void RaisePause()
        {
            OnRacePause?.Invoke();
            if (onRacePause != null)
            {
                onRacePause.Invoke();
            }
        }

        private void RaiseResume()
        {
            OnRaceResume?.Invoke();
            if (onRaceResume != null)
            {
                onRaceResume.Invoke();
            }
        }

        private void RaiseFinish()
        {
            OnRaceFinish?.Invoke();
            if (onRaceFinish != null)
            {
                onRaceFinish.Invoke();
            }
        }
    }
}
