using System;
using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Backend;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.Progression.Configuration;
using GulfRun.Features.Progression.Missions;
using UnityEngine;

namespace GulfRun.Features.Progression.Login
{
    /// <summary>
    /// Composition root for the Login Streak / Daily Login Rewards system.
    /// Owns the always-active standard 7-day calendar plus every authored
    /// Special Login Event calendar (brief "SPECIAL LOGIN EVENTS"), and
    /// resolves/claims each day's reward via <see cref="LoginStreakCalculator"/>
    /// + <see cref="RewardApplication"/>. Persistent (Boot-scene).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class LoginRewardManager : Singleton<LoginRewardManager>, ILoginRewardStatusProvider, IEventBannerSource
    {
        [SerializeField] private LoginRewardCalendarConfig standardCalendar;
        [SerializeField] private List<LoginRewardCalendarConfig> specialEventCalendars = new List<LoginRewardCalendarConfig>();

        private IRandomSource _random;
        private LoginRewardCalendarConfig _activeSpecialEvent;
        private bool _reportedLoginThisSession;

        public LoginStreakStatus Status => ProgressionBackendService.Current.GetLoginStreakStatus();

        /// <summary>The calendar the next claim will use — the active Special Event override if one is set, otherwise the standard calendar.</summary>
        public LoginRewardCalendarConfig ActiveCalendar => _activeSpecialEvent != null ? _activeSpecialEvent : standardCalendar;

        public IReadOnlyList<LoginRewardCalendarConfig> SpecialEventCalendars => specialEventCalendars;

        public string ActiveSpecialEventLabel => _activeSpecialEvent != null ? _activeSpecialEvent.EventLabel : string.Empty;

        protected override void OnInitialize()
        {
            _random = SeededRandom.FromTime();
            _activeSpecialEvent = null;
            LoginRewardStatusService.Current = this;
            EventBannerRegistry.Register(this);
        }

        private void OnDisable()
        {
            if (ReferenceEquals(LoginRewardStatusService.Current, this))
            {
                LoginRewardStatusService.Current = null;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            EventBannerRegistry.Unregister(this);
        }

        /// <summary>Sprint 13 (Main Menu Event Banner): the active Special Login Event calendar (e.g. Ramadan/National Day), if one is set.</summary>
        public IReadOnlyList<string> GetActiveBannerMessages() =>
            _activeSpecialEvent != null ? new[] { ActiveSpecialEventLabel + " Login Rewards are live!" } : System.Array.Empty<string>();

        private void Update()
        {
            // "Login today" (both the Daily Mission type and the
            // once-per-session notification) only needs to fire once per
            // app session — the actual Login Streak claim is a deliberate
            // player action (TryClaimDailyLogin), not automatic.
            if (_reportedLoginThisSession)
            {
                return;
            }

            _reportedLoginThisSession = true;
            MissionManager.Instance?.ReportLogin();

            if (!HasClaimedToday())
            {
                ProgressionNotificationBridge.Raise(NotificationType.DailyRewardAvailable, "Your Daily Login Reward is ready to claim!");
            }
        }

        public bool HasClaimedToday() => ProgressionBackendService.Current.HasClaimedLoginToday(NowSeconds());

        /// <summary>
        /// Sprint 11 "SPECIAL LOGIN EVENTS" manual activation switch — no
        /// live calendar/scheduler exists yet to auto-activate one by
        /// real-world date (same category of TODO Sprint 9/10 already
        /// flagged for Tournaments/Special Offers), so a real LiveOps
        /// service would call this instead of a debug/manual trigger later.
        /// Pass null/empty to return to the standard calendar.
        /// </summary>
        public void SetActiveSpecialEvent(string eventLabelOrNull)
        {
            if (string.IsNullOrEmpty(eventLabelOrNull))
            {
                _activeSpecialEvent = null;
                return;
            }

            for (int i = 0; i < specialEventCalendars.Count; i++)
            {
                if (specialEventCalendars[i] != null && string.Equals(specialEventCalendars[i].EventLabel, eventLabelOrNull, StringComparison.OrdinalIgnoreCase))
                {
                    _activeSpecialEvent = specialEventCalendars[i];
                    return;
                }
            }
        }

        /// <summary>Claims today's Login Streak reward. Returns false if already claimed today or no calendar is configured.</summary>
        public bool TryClaimDailyLogin()
        {
            LoginRewardCalendarConfig calendar = ActiveCalendar;
            double now = NowSeconds();
            if (calendar == null || ProgressionBackendService.Current.HasClaimedLoginToday(now))
            {
                return false;
            }

            LoginStreakStatus status = Status;
            int nextDay = LoginStreakCalculator.ResolveNextStreakDay(status.LastClaimAtSeconds, now, status.CurrentStreakDay, calendar.CycleLength);

            LoginRewardCalendarConfig.LoginRewardEntry entry = calendar.GetDay(nextDay);
            if (entry != null)
            {
                ApplyEntry(entry);
            }

            ProgressionBackendService.Current.RecordLoginClaim(nextDay, now);
            ProgressionNotificationBridge.Raise(NotificationType.DailyRewardAvailable, "Day " + nextDay + " Login Reward claimed!");
            return true;
        }

        private void ApplyEntry(LoginRewardCalendarConfig.LoginRewardEntry entry)
        {
            if (!entry.IsMysteryReward)
            {
                RewardApplication.Apply(entry.RewardType, entry.RewardAmount, entry.RewardCosmeticId, entry.IsTemporaryCosmeticReward, entry.TemporaryDuration, entry.FallbackCoinsAmount, "login_day_" + entry.Day);

                // Brief "Day 6: Coins + Gems" — an optional flat second grant on top of the primary reward above.
                if (entry.HasBonusReward)
                {
                    RewardApplication.Apply(entry.BonusRewardType, entry.BonusRewardAmount, CosmeticId.None, false, entry.TemporaryDuration, 0, "login_day_" + entry.Day + "_bonus");
                }

                return;
            }

            if (entry.MysteryOptions.Count == 0)
            {
                return;
            }

            var weighted = new List<WeightedOption<LoginRewardCalendarConfig.MysteryRewardOption>>(entry.MysteryOptions.Count);
            for (int i = 0; i < entry.MysteryOptions.Count; i++)
            {
                weighted.Add(new WeightedOption<LoginRewardCalendarConfig.MysteryRewardOption>(entry.MysteryOptions[i], entry.MysteryOptions[i].Weight));
            }

            if (WeightedSelector.TrySelect(weighted, _random, out LoginRewardCalendarConfig.MysteryRewardOption picked))
            {
                RewardApplication.Apply(picked.RewardType, picked.RewardAmount, picked.RewardCosmeticId, picked.IsTemporaryCosmeticReward, picked.TemporaryDuration, picked.FallbackCoinsAmount, "login_mystery_day" + entry.Day + "_" + (long)NowSeconds());
            }
        }

        private static double NowSeconds() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}
