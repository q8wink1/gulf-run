using System;
using System.Collections.Generic;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Progression.Configuration
{
    /// <summary>
    /// One Login Streak reward calendar — either the always-active standard
    /// calendar (<see cref="EventLabel"/> empty) or one named Special Login
    /// Event calendar (brief "SPECIAL LOGIN EVENTS: Ramadan/Eid/National
    /// Days/Summer Events/Winter Events/Future Events" — authoring a new
    /// asset with a new <see cref="EventLabel"/> is how a "Future Event" is
    /// added, never a code change). <c>Login.LoginRewardManager</c> decides
    /// which single calendar is currently active; there is no live
    /// scheduler yet to auto-activate one by real-world date (same category
    /// of "no calendar/scheduler exists yet" TODO Sprint 9/10 already
    /// flagged for Tournaments/Special Offers), so activation today is a
    /// manual override a real LiveOps/calendar service would drive later.
    /// </summary>
    [CreateAssetMenu(fileName = "LoginRewardCalendarConfig", menuName = "GulfRun/Progression/Login Reward Calendar Config")]
    public sealed class LoginRewardCalendarConfig : ScriptableObject
    {
        [Serializable]
        public sealed class MysteryRewardOption
        {
            [SerializeField] private string displayName = string.Empty;
            [SerializeField] private RewardType rewardType;
            [SerializeField] private int rewardAmount;
            [SerializeField] private string rewardCosmeticId;
            [SerializeField] private bool isTemporaryCosmeticReward;
            [SerializeField] private TemporaryCosmeticDuration temporaryDuration = TemporaryCosmeticDuration.SevenDays;
            [SerializeField] private int fallbackCoinsAmount = 200;
            [SerializeField] private float weight = 1f;

            public string DisplayName => displayName;
            public RewardType RewardType => rewardType;
            public int RewardAmount => rewardAmount;
            public CosmeticId RewardCosmeticId => new CosmeticId(rewardCosmeticId);
            public bool IsTemporaryCosmeticReward => isTemporaryCosmeticReward;
            public TemporaryCosmeticDuration TemporaryDuration => temporaryDuration;
            public int FallbackCoinsAmount => fallbackCoinsAmount;
            public float Weight => weight;
        }

        [Serializable]
        public sealed class LoginRewardEntry
        {
            [SerializeField] private int day = 1;
            [SerializeField] private string displayName = string.Empty;
            [SerializeField] private RewardType rewardType;
            [SerializeField] private int rewardAmount;
            [SerializeField] private string rewardCosmeticId;
            [SerializeField] private bool isTemporaryCosmeticReward;
            [SerializeField] private TemporaryCosmeticDuration temporaryDuration = TemporaryCosmeticDuration.ThreeDays;
            [SerializeField] private int fallbackCoinsAmount = 100;

            [Tooltip("If true, this day grants a random reward from mysteryOptions (weighted) instead of the fixed rewardType/rewardAmount/rewardCosmeticId fields above — brief 'Day 7: Large Mystery Reward'.")]
            [SerializeField] private bool isMysteryReward;
            [SerializeField] private List<MysteryRewardOption> mysteryOptions = new List<MysteryRewardOption>();

            [Header("Optional Bonus (brief: 'Day 6: Coins + Gems')")]
            [SerializeField] private bool hasBonusReward;
            [SerializeField] private RewardType bonusRewardType;
            [SerializeField] private int bonusRewardAmount;

            public int Day => day;
            public string DisplayName => string.IsNullOrEmpty(displayName) ? "Day " + day : displayName;
            public RewardType RewardType => rewardType;
            public int RewardAmount => rewardAmount;
            public CosmeticId RewardCosmeticId => new CosmeticId(rewardCosmeticId);
            public bool IsTemporaryCosmeticReward => isTemporaryCosmeticReward;
            public TemporaryCosmeticDuration TemporaryDuration => temporaryDuration;
            public int FallbackCoinsAmount => fallbackCoinsAmount;
            public bool IsMysteryReward => isMysteryReward;
            public IReadOnlyList<MysteryRewardOption> MysteryOptions => mysteryOptions;
            public bool HasBonusReward => hasBonusReward;
            public RewardType BonusRewardType => bonusRewardType;
            public int BonusRewardAmount => bonusRewardAmount;
        }

        [Tooltip("Empty = the always-active standard calendar. Non-empty = a Special Login Event calendar, matched by Login.LoginRewardManager.SetActiveSpecialEvent(string).")]
        [SerializeField] private string eventLabel = string.Empty;

        [SerializeField] private List<LoginRewardEntry> days = new List<LoginRewardEntry>();

        public string EventLabel => eventLabel;

        /// <summary>Streak length before it wraps back to Day 1 — the number of authored days (7 for the standard calendar, but a Special Event calendar is free to author a different length).</summary>
        public int CycleLength => days.Count > 0 ? days.Count : 1;

        public IReadOnlyList<LoginRewardEntry> Days => days;

        public LoginRewardEntry GetDay(int dayNumber)
        {
            for (int i = 0; i < days.Count; i++)
            {
                if (days[i] != null && days[i].Day == dayNumber)
                {
                    return days[i];
                }
            }

            return null;
        }
    }
}
