using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Gameplay
{
    /// <summary>
    /// Sprint 23.11 — Inspector preset for race rule settings.
    /// Pure configuration; <see cref="GameRulesManager"/> applies and exposes it.
    /// </summary>
    [CreateAssetMenu(
        fileName = "GameRulesConfig",
        menuName = "GulfRun/Gameplay/Game Rules Config")]
    public sealed class GameRulesConfig : ScriptableObject
    {
        [Header("Lobby / Match")]
        [Tooltip("Maximum players allowed in a race session.")]
        [SerializeField] private int maximumPlayers = 4;

        [Header("Race")]
        [Tooltip("Target finish distance (world units). Unused by auto-finish this sprint.")]
        [SerializeField] private float raceDistance = 1000f;

        [Tooltip("Race time limit in seconds. 0 = no time limit.")]
        [SerializeField] private float timeLimitSeconds;

        [Header("Rules Flags")]
        [SerializeField] private bool eliminationEnabled;
        [SerializeField] private bool respawnEnabled;

        [Header("Win")]
        [SerializeField] private WinCondition winCondition = WinCondition.FinishLine;

        public int MaximumPlayers => maximumPlayers < 1 ? 1 : maximumPlayers;
        public float RaceDistance => raceDistance < 0f ? 0f : raceDistance;
        public float TimeLimitSeconds => timeLimitSeconds < 0f ? 0f : timeLimitSeconds;
        public bool HasTimeLimit => TimeLimitSeconds > 0f;
        public bool EliminationEnabled => eliminationEnabled;
        public bool RespawnEnabled => respawnEnabled;
        public WinCondition WinCondition => winCondition;

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
