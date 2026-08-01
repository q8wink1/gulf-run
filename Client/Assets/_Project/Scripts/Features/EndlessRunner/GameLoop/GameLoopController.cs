using System;
using GulfRun.Core;
using GulfRun.Core.Managers;
using GulfRun.Core.Save;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.EndlessRunner.Difficulty;
using GulfRun.Features.EndlessRunner.Distance;
using GulfRun.Features.EndlessRunner.Scoring;
using GulfRun.Features.EndlessRunner.Speed;
using GulfRun.Features.EndlessRunner.WorldGeneration;
using UnityEngine;

namespace GulfRun.Features.EndlessRunner.GameLoop
{
    /// <summary>
    /// Composition root and state machine for a single endless-runner
    /// session (Ready -> Countdown -> Running -> Paused/GameOver -> Restart).
    /// Owns the single per-frame tick ordering for every gameplay-session
    /// system, so there is never any ambiguity about MonoBehaviour Update()
    /// execution order between them — and so a future server-authoritative
    /// simulation can drive the exact same Tick sequence for multiplayer
    /// sync. Publishes itself as <see cref="IGameStateProvider"/> via
    /// <see cref="GameStateService"/> so the player (a different feature)
    /// can react to Countdown/Running/GameOver without a direct reference.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class GameLoopController : SceneSingleton<GameLoopController>, IGameStateProvider
    {
        private GameSpeedController _speedController;
        private DistanceTracker _distanceTracker;
        private ScoreController _scoreController;
        private DifficultyController _difficultyController;
        private WorldGenerator _worldGenerator;
        private CountdownController _countdownController;

        public GameLoopState State { get; private set; } = GameLoopState.Ready;

        /// <summary>Raised whenever <see cref="State"/> changes.</summary>
        public event Action<GameLoopState> StateChanged;

        GameLoopState IGameStateProvider.CurrentState => State;

        /// <summary>
        /// Optional override for tests/tooling. Defaults to
        /// <see cref="SaveManager.Instance"/> — dependency inversion without
        /// requiring an Inspector-serializable interface reference.
        /// </summary>
        public IProgressRepository ProgressRepositoryOverride { get; set; }

        protected override void Awake()
        {
            base.Awake();
            _speedController = GetComponent<GameSpeedController>();
            _distanceTracker = GetComponent<DistanceTracker>();
            _scoreController = GetComponent<ScoreController>();
            _difficultyController = GetComponent<DifficultyController>();
            _worldGenerator = GetComponent<WorldGenerator>();
            _countdownController = GetComponent<CountdownController>();
        }

        private void OnEnable()
        {
            GameStateService.Current = this;
            _countdownController.Finished += HandleCountdownFinished;
        }

        private void OnDisable()
        {
            _countdownController.Finished -= HandleCountdownFinished;

            if (ReferenceEquals(GameStateService.Current, this))
            {
                GameStateService.Current = null;
            }
        }

        /// <summary>
        /// No start button: the race-start countdown begins automatically as
        /// soon as the session is ready.
        /// </summary>
        private void Start()
        {
            RequestStart();
        }

        private void Update()
        {
            if (State == GameLoopState.Countdown)
            {
                _countdownController.Tick(Time.deltaTime);
                return;
            }

            if (State != GameLoopState.Running)
            {
                return;
            }

            float deltaTime = Time.deltaTime;

            // Fixed, explicit order: speed uses last-known distance, distance
            // integrates the speed just computed, then difficulty/world/score
            // all read the freshly-updated distance.
            _speedController.Tick(deltaTime, _distanceTracker.DistanceMeters);
            _distanceTracker.Tick(deltaTime, _speedController.CurrentSpeed);
            _difficultyController.Tick(_distanceTracker.DistanceMeters);
            _worldGenerator.Tick(_distanceTracker.DistanceMeters, _difficultyController.Current01);
            _scoreController.Tick();
        }

        /// <summary>
        /// Ready -> Countdown. Begins the automatic 3-2-1-GO countdown; no
        /// button/UI trigger is required — see <see cref="Start"/>.
        /// </summary>
        public void RequestStart()
        {
            if (State != GameLoopState.Ready)
            {
                return;
            }

            _countdownController.BeginCountdown();
            SetState(GameLoopState.Countdown);
        }

        /// <summary>Countdown -> Running, the instant the countdown reaches GO. The player begins running automatically.</summary>
        private void HandleCountdownFinished()
        {
            SetState(GameLoopState.Running);
        }

        /// <summary>Running -> Paused. Freezes physics/animation via Time.timeScale in addition to halting the session tick.</summary>
        public void RequestPause()
        {
            if (State != GameLoopState.Running)
            {
                return;
            }

            Time.timeScale = 0f;
            SetState(GameLoopState.Paused);
        }

        /// <summary>Paused -> Running.</summary>
        public void RequestResume()
        {
            if (State != GameLoopState.Paused)
            {
                return;
            }

            Time.timeScale = 1f;
            SetState(GameLoopState.Running);
        }

        /// <summary>Running -> GameOver. Commits best-distance/best-score/coins via the progress repository.</summary>
        public void RequestGameOver()
        {
            if (State != GameLoopState.Running)
            {
                return;
            }

            Time.timeScale = 0f;
            CommitBestResults();
            SetState(GameLoopState.GameOver);
        }

        /// <summary>
        /// GameOver -> Restart -> Ready -> Countdown: resets every session
        /// system for a fresh attempt and immediately begins a new race-start
        /// countdown, consistent with "no button required" for starting a run.
        /// </summary>
        public void RequestRestart()
        {
            if (State != GameLoopState.GameOver)
            {
                return;
            }

            SetState(GameLoopState.Restart);

            Time.timeScale = 1f;
            _speedController.ResetSpeed();
            _distanceTracker.ResetDistance();
            _scoreController.ResetScore();
            _difficultyController.ResetDifficulty();
            _worldGenerator.ResetGenerator();

            SetState(GameLoopState.Ready);
            RequestStart();
        }

        private void CommitBestResults()
        {
            IProgressRepository repository = ProgressRepositoryOverride != null
                ? ProgressRepositoryOverride
                : SaveManager.Instance;

            if (repository == null)
            {
                return;
            }

            float finalDistance = (float)_distanceTracker.DistanceMeters;
            float finalScore = _scoreController.TotalScore;

            if (finalDistance > repository.GetBestDistance())
            {
                repository.SaveBestDistance(finalDistance);
            }

            if (finalScore > repository.GetBestScore())
            {
                repository.SaveBestScore(finalScore);
            }

            repository.AddCoinsCollected(_scoreController.CoinsCollected);
        }

        private void SetState(GameLoopState newState)
        {
            if (newState == State)
            {
                return;
            }

            State = newState;
            StateChanged?.Invoke(newState);
        }
    }
}
