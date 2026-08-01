using System.Collections.Generic;
using UnityEngine;

namespace GulfRun.Features.RaceFinish.Configuration
{
    /// <summary>
    /// Every tunable value for the Race Finish / Ranking / Victory Ceremony /
    /// Reward flow in one ScriptableObject — no hardcoded track length,
    /// elimination gaps/timeouts, ceremony timings, or reward amounts
    /// anywhere in <c>Features.RaceFinish</c> code. Mirrors the role
    /// <c>NetworkSyncConfig</c> plays for Sprint 4 and <c>WeaponCatalogConfig</c>/
    /// <c>TrapCatalogConfig</c> play for Sprints 5/6.
    ///
    /// The reward table values below are placeholders pending a ratified
    /// Reward System specification — P011 (Post-Race Results) explicitly
    /// marks Coin/Rank Point/Experience reward amounts as "not yet defined";
    /// see the Sprint 7 report for how this is reconciled (every value here
    /// is designer-editable data, never asserted as final game-balance).
    /// </summary>
    [CreateAssetMenu(fileName = "RaceFinishConfig", menuName = "GulfRun/RaceFinish/Race Finish Config")]
    public sealed class RaceFinishConfig : ScriptableObject
    {
        [Header("Race Length")]
        [Tooltip("Distance from start to finish line. ~550m yields ~80-90s for an average player at this project's base/max run speed (see GameSpeedConfig) — configurable per-track.")]
        [SerializeField] private float trackLengthMeters = 550f;

        [Header("Elimination")]
        [Tooltip("Meters behind the current race leader (or the finish line, once someone has crossed it) that triggers an elimination warning.")]
        [SerializeField] private float eliminationWarningGapMeters = 25f;
        [Tooltip("Must close the gap to at or below this many meters to cancel an active elimination warning. Kept lower than the warning gap to avoid rapid warn/clear flicker.")]
        [SerializeField] private float eliminationRecoveryGapMeters = 15f;
        [Tooltip("Seconds of grace after a warning begins before the player is automatically eliminated if they have not recovered.")]
        [SerializeField] private float eliminationCountdownSeconds = 5f;
        [Tooltip("Safety-net timeout: any player still racing after this many seconds is automatically eliminated, guaranteeing the race always ends even if nobody triggers the gap threshold.")]
        [SerializeField] private float maxRaceDurationSeconds = 150f;

        [Header("Progress Reporting")]
        [Tooltip("How often each client reports its local distance/coins to the host for finish-line and elimination checks.")]
        [SerializeField] private float progressReportIntervalSeconds = 0.5f;

        [Header("Ceremony")]
        [Tooltip("Automatic playback duration of the Podium Ceremony before auto-advancing to the Reward Screen (sooner if any player presses Skip).")]
        [SerializeField] private float podiumCeremonySeconds = 6f;
        [Tooltip("Automatic playback duration of the Reward Screen before the match automatically returns to the lobby (sooner if any player presses Skip).")]
        [SerializeField] private float rewardScreenSeconds = 6f;
        [Tooltip("Seconds over which each reward counter animates from 0 to its final value.")]
        [SerializeField] private float rewardCounterAnimationSeconds = 1.5f;
        [Tooltip("Looping music played while the Podium Ceremony is active (optional).")]
        [SerializeField] private AudioClip victoryMusicClip;

        [Header("Champion Presentation (Sprint 7 addendum)")]
        [Tooltip("One-shot fanfare/sting played the instant the champion's Podium Ceremony begins, layered on top of the looping victory music — the \"Special victory music\" called for 1st place specifically (optional).")]
        [SerializeField] private AudioClip championFanfareClip;
        [Tooltip("Number of golden confetti particles simulated behind the champion during the Podium Ceremony.")]
        [SerializeField] private int confettiParticleCount = 40;
        [Tooltip("Downward fall speed of confetti particles, in effect-area heights per second.")]
        [SerializeField] private float confettiFallSpeed = 0.25f;
        [Tooltip("Degrees of gentle side-to-side sway applied to each national flag during the ceremony.")]
        [SerializeField] private float flagWaveAmplitudeDegrees = 12f;
        [Tooltip("Sway cycles per second for the national flag animation.")]
        [SerializeField] private float flagWaveFrequencyHz = 0.5f;

        [Header("Reward Tuning (placeholder values — see Sprint 7 report re: P011 alignment)")]
        [Tooltip("Multiplier applied to raw coins collected before crediting the reward wallet. 1 = pass-through.")]
        [SerializeField] private float coinRewardMultiplier = 1f;
        [Tooltip("Indexed by finish position (index 0 = 1st place). The last entry is reused for any position beyond the list's length.")]
        [SerializeField] private List<int> bonusCoinsByPosition = new List<int> { 100, 60, 30, 10 };
        [SerializeField] private List<int> rankPointsByPosition = new List<int> { 50, 25, 10, 0 };
        [SerializeField] private List<int> experienceByPosition = new List<int> { 200, 150, 100, 50 };
        [Tooltip("Flat Experience granted to every participant regardless of placement, on top of the placement-indexed amount above.")]
        [SerializeField] private int participationExperience = 25;

        public float TrackLengthMeters => trackLengthMeters;
        public float EliminationWarningGapMeters => eliminationWarningGapMeters;
        public float EliminationRecoveryGapMeters => eliminationRecoveryGapMeters;
        public float EliminationCountdownSeconds => eliminationCountdownSeconds;
        public float MaxRaceDurationSeconds => maxRaceDurationSeconds;
        public float ProgressReportIntervalSeconds => progressReportIntervalSeconds;
        public float PodiumCeremonySeconds => podiumCeremonySeconds;
        public float RewardScreenSeconds => rewardScreenSeconds;
        public float RewardCounterAnimationSeconds => rewardCounterAnimationSeconds;
        public AudioClip VictoryMusicClip => victoryMusicClip;
        public AudioClip ChampionFanfareClip => championFanfareClip;
        public int ConfettiParticleCount => confettiParticleCount;
        public float ConfettiFallSpeed => confettiFallSpeed;
        public float FlagWaveAmplitudeDegrees => flagWaveAmplitudeDegrees;
        public float FlagWaveFrequencyHz => flagWaveFrequencyHz;
        public float CoinRewardMultiplier => coinRewardMultiplier;
        public IReadOnlyList<int> BonusCoinsByPosition => bonusCoinsByPosition;
        public IReadOnlyList<int> RankPointsByPosition => rankPointsByPosition;
        public IReadOnlyList<int> ExperienceByPosition => experienceByPosition;
        public int ParticipationExperience => participationExperience;
    }
}
