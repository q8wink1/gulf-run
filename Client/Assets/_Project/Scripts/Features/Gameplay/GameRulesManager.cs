using System;
using GulfRun.Core;
using GulfRun.Domain;
using UnityEngine;
using UnityEngine.Events;

namespace GulfRun.Features.Gameplay
{
    /// <summary>
    /// Sprint 23.11 — race rules config + event hub.
    /// Defines settings and win/lose vocabulary; exposes events for subscribers.
    /// Optionally bridges <see cref="RaceManager"/> start/pause/finish without
    /// duplicating finish / elimination / distance gameplay.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class GameRulesManager : SceneSingleton<GameRulesManager>
    {
        [Header("References")]
        [Tooltip("Optional flow coordinator — bridged for RaceStarted/Paused/Finished.")]
        [SerializeField] private RaceManager raceManager;

        [Tooltip("Optional Inspector preset. When set, overrides inline settings on Awake.")]
        [SerializeField] private GameRulesConfig rulesConfig;

        [Header("Race Settings")]
        [Tooltip("Maximum players allowed in a race session.")]
        [SerializeField] private int maximumPlayers = 4;

        [Tooltip("Target finish distance (world units). Reserved — no auto-finish.")]
        [SerializeField] private float raceDistance = 1000f;

        [Tooltip("Race time limit in seconds. 0 = no time limit.")]
        [SerializeField] private float timeLimitSeconds;

        [SerializeField] private bool eliminationEnabled;
        [SerializeField] private bool respawnEnabled;

        [Header("Conditions")]
        [SerializeField] private WinCondition winCondition = WinCondition.FinishLine;

        [Header("Inspector Events (optional)")]
        [SerializeField] private UnityEvent onRaceStarted;
        [SerializeField] private UnityEvent onRacePaused;
        [SerializeField] private UnityEvent onRaceFinished;

        private RaceManager _subscribedRaceManager;

        public GameRulesConfig RulesConfig => rulesConfig;
        public RaceManager RaceManager => raceManager;

        public int MaximumPlayers => maximumPlayers < 1 ? 1 : maximumPlayers;
        public float RaceDistance => raceDistance < 0f ? 0f : raceDistance;
        public float TimeLimitSeconds => timeLimitSeconds < 0f ? 0f : timeLimitSeconds;
        public bool HasTimeLimit => TimeLimitSeconds > 0f;
        public bool EliminationEnabled => eliminationEnabled;
        public bool RespawnEnabled => respawnEnabled;
        public WinCondition WinCondition => winCondition;

        /// <summary>C# event for Features subscribers (preferred over UnityEvent).</summary>
        public event Action RaceStarted;

        public event Action RacePaused;
        public event Action RaceFinished;
        public event Action<string> PlayerFinished;
        public event Action<string, LoseCondition> PlayerEliminated;

        protected override void Awake()
        {
            base.Awake();
            ApplyConfigAsset();
        }

        private void OnEnable()
        {
            SubscribeRaceManager(ResolveRaceManager());
        }

        private void Start()
        {
            SubscribeRaceManager(ResolveRaceManager());
        }

        private void OnDisable()
        {
            UnsubscribeRaceManager();
        }

        /// <summary>Swap or clear the optional config asset and re-apply fields.</summary>
        public void SetRulesConfig(GameRulesConfig config)
        {
            rulesConfig = config;
            ApplyConfigAsset();
        }

        /// <summary>Assign RaceManager reference (bridges events when enabled).</summary>
        public void SetRaceManager(RaceManager manager)
        {
            UnsubscribeRaceManager();
            raceManager = manager;
            SubscribeRaceManager(ResolveRaceManager());
        }

        /// <summary>
        /// Stub win evaluation — no distance / standings checks this sprint.
        /// Always returns false; <paramref name="winnerPlayerId"/> is null.
        /// </summary>
        public bool EvaluateWin(out string winnerPlayerId)
        {
            winnerPlayerId = null;
            return false;
        }

        /// <summary>
        /// Stub: report that a player finished. Fires <see cref="PlayerFinished"/>
        /// only — does not call <see cref="RaceManager.FinishRace"/> or check distance.
        /// </summary>
        public void ReportPlayerFinished(string playerId)
        {
            if (string.IsNullOrEmpty(playerId))
            {
                return;
            }

            RaisePlayerFinished(playerId);
        }

        /// <summary>
        /// Stub: report elimination / lose. Fires <see cref="PlayerEliminated"/>.
        /// Honors EliminationEnabled for <see cref="LoseCondition.Elimination"/> only
        /// as a soft gate (still no gameplay consequences).
        /// </summary>
        public void ReportPlayerEliminated(string playerId, LoseCondition reason)
        {
            if (string.IsNullOrEmpty(playerId))
            {
                return;
            }

            if (reason == LoseCondition.Elimination && !eliminationEnabled)
            {
                return;
            }

            RaisePlayerEliminated(playerId, reason);
        }

        /// <summary>
        /// Stub lose report for disconnect / timeout (and elimination if enabled).
        /// Routes to <see cref="ReportPlayerEliminated"/>.
        /// </summary>
        public void ReportLose(string playerId, LoseCondition reason)
        {
            ReportPlayerEliminated(playerId, reason);
        }

        /// <summary>
        /// Architecture notify: raise <see cref="RaceStarted"/> without starting
        /// the race. Prefer RaceManager.StartRace + bridge in normal flow.
        /// </summary>
        public void NotifyRaceStarted()
        {
            RaiseRaceStarted();
        }

        /// <summary>Architecture notify: raise <see cref="RacePaused"/>.</summary>
        public void NotifyRacePaused()
        {
            RaiseRacePaused();
        }

        /// <summary>
        /// Architecture notify: raise <see cref="RaceFinished"/> without finish
        /// detection or calling RaceManager.FinishRace.
        /// </summary>
        public void NotifyRaceFinished()
        {
            RaiseRaceFinished();
        }

        /// <summary>True when <paramref name="condition"/> is considered active by config.</summary>
        public bool IsLoseConditionActive(LoseCondition condition)
        {
            switch (condition)
            {
                case LoseCondition.Disconnect:
                    return true;
                case LoseCondition.Elimination:
                    return eliminationEnabled;
                case LoseCondition.Timeout:
                    return HasTimeLimit;
                default:
                    return false;
            }
        }

        private void ApplyConfigAsset()
        {
            if (rulesConfig == null)
            {
                return;
            }

            maximumPlayers = rulesConfig.MaximumPlayers;
            raceDistance = rulesConfig.RaceDistance;
            timeLimitSeconds = rulesConfig.TimeLimitSeconds;
            eliminationEnabled = rulesConfig.EliminationEnabled;
            respawnEnabled = rulesConfig.RespawnEnabled;
            winCondition = rulesConfig.WinCondition;
        }

        private RaceManager ResolveRaceManager()
        {
            if (raceManager != null)
            {
                return raceManager;
            }

            raceManager = RaceManager.Instance;
            return raceManager;
        }

        private void SubscribeRaceManager(RaceManager manager)
        {
            if (manager == null || manager == _subscribedRaceManager)
            {
                return;
            }

            UnsubscribeRaceManager();
            _subscribedRaceManager = manager;
            manager.OnRaceStart += OnRaceManagerStarted;
            manager.OnRacePause += OnRaceManagerPaused;
            manager.OnRaceFinish += OnRaceManagerFinished;
        }

        private void UnsubscribeRaceManager()
        {
            if (_subscribedRaceManager == null)
            {
                return;
            }

            _subscribedRaceManager.OnRaceStart -= OnRaceManagerStarted;
            _subscribedRaceManager.OnRacePause -= OnRaceManagerPaused;
            _subscribedRaceManager.OnRaceFinish -= OnRaceManagerFinished;
            _subscribedRaceManager = null;
        }

        private void OnRaceManagerStarted()
        {
            RaiseRaceStarted();
        }

        private void OnRaceManagerPaused()
        {
            RaiseRacePaused();
        }

        private void OnRaceManagerFinished()
        {
            RaiseRaceFinished();
        }

        private void RaiseRaceStarted()
        {
            RaceStarted?.Invoke();
            if (onRaceStarted != null)
            {
                onRaceStarted.Invoke();
            }
        }

        private void RaiseRacePaused()
        {
            RacePaused?.Invoke();
            if (onRacePaused != null)
            {
                onRacePaused.Invoke();
            }
        }

        private void RaiseRaceFinished()
        {
            RaceFinished?.Invoke();
            if (onRaceFinished != null)
            {
                onRaceFinished.Invoke();
            }
        }

        private void RaisePlayerFinished(string playerId)
        {
            PlayerFinished?.Invoke(playerId);
        }

        private void RaisePlayerEliminated(string playerId, LoseCondition reason)
        {
            PlayerEliminated?.Invoke(playerId, reason);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            maximumPlayers = Mathf.Max(1, maximumPlayers);
            raceDistance = Mathf.Max(0f, raceDistance);
            timeLimitSeconds = Mathf.Max(0f, timeLimitSeconds);
        }
#endif
    }
}
