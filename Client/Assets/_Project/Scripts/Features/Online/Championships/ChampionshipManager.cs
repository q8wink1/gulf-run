using System;
using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Managers;
using GulfRun.Domain;
using GulfRun.Features.Online.Configuration;
using GulfRun.Features.Online.HallOfFame;
using GulfRun.Features.Online.Leaderboard;
using GulfRun.Features.Online.Notifications;
using UnityEngine;

namespace GulfRun.Features.Online.Championships
{
    /// <summary>
    /// Tracks which single Championship (Weekly/Monthly/Season/Weekend/
    /// Special Event) and which single Country Event (National Day/
    /// Ramadan/Eid/Summer/Winter/regional) are currently "Active" out of
    /// their respective catalogs. No real calendar/scheduler exists yet
    /// (see Sprint 9 report Remaining TODOs) — starts the first entry of
    /// each catalog automatically so there is always something live to
    /// show, and exposes <see cref="AdvanceToNextChampionship"/>/
    /// <see cref="AdvanceToNextCountryEvent"/> for
    /// <c>OnlineDebugView</c>'s "Simulate Advance" button to step through
    /// the rest, raising the exact Tournament Starting/Ending and New
    /// Event notifications the brief requires along the way.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ChampionshipManager : Singleton<ChampionshipManager>
    {
        [SerializeField] private ChampionshipCatalogConfig championshipCatalog;
        [SerializeField] private CountryEventCatalogConfig countryEventCatalog;

        private int _activeChampionshipIndex = -1;
        private int _activeCountryEventIndex = -1;

        public ChampionshipCatalogConfig ChampionshipCatalog => championshipCatalog;

        public CountryEventCatalogConfig CountryEventCatalog => countryEventCatalog;

        public bool HasActiveChampionship => championshipCatalog != null && _activeChampionshipIndex >= 0 && _activeChampionshipIndex < championshipCatalog.Championships.Count;

        public ChampionshipCatalogConfig.ChampionshipEntry ActiveChampionship =>
            HasActiveChampionship ? championshipCatalog.Championships[_activeChampionshipIndex] : null;

        public bool HasActiveCountryEvent => countryEventCatalog != null && _activeCountryEventIndex >= 0 && _activeCountryEventIndex < countryEventCatalog.Events.Count;

        public CountryEventCatalogConfig.CountryEventEntry ActiveCountryEvent =>
            HasActiveCountryEvent ? countryEventCatalog.Events[_activeCountryEventIndex] : null;

        public event Action ActiveChampionshipChanged;
        public event Action ActiveCountryEventChanged;

        protected override void OnInitialize()
        {
        }

        private void Start()
        {
            if (championshipCatalog != null && championshipCatalog.Championships.Count > 0)
            {
                StartChampionship(0);
            }

            if (countryEventCatalog != null && countryEventCatalog.Events.Count > 0)
            {
                StartCountryEvent(0);
            }
        }

        public void StartChampionship(int index)
        {
            if (championshipCatalog == null || index < 0 || index >= championshipCatalog.Championships.Count)
            {
                return;
            }

            _activeChampionshipIndex = index;
            ActiveChampionshipChanged?.Invoke();
            NotificationManager.Instance?.Raise(NotificationType.TournamentStarting, ActiveChampionship.DisplayName + " has started!");
        }

        /// <summary>Ends the active championship (if any): pays out its headline reward, records a Hall of Fame Tournament Champion entry from the current World #1, and clears the active slot.</summary>
        public void EndActiveChampionship()
        {
            if (!HasActiveChampionship)
            {
                return;
            }

            ChampionshipCatalogConfig.ChampionshipEntry ended = ActiveChampionship;
            NotificationManager.Instance?.Raise(NotificationType.TournamentEnding, ended.DisplayName + " has ended!");
            ApplyHeadlineReward(ended);
            NotificationManager.Instance?.Raise(NotificationType.RewardsReady, "Rewards from " + ended.DisplayName + " are ready to collect.");

            IReadOnlyList<LeaderboardEntry> worldTop = LeaderboardManager.Instance != null
                ? LeaderboardManager.Instance.GetLeaderboard(RankingScope.World, null, 1)
                : Array.Empty<LeaderboardEntry>();

            if (worldTop.Count > 0 && HallOfFameManager.Instance != null)
            {
                LeaderboardEntry champion = worldTop[0];
                HallOfFameManager.Instance.RecordEntry(HallOfFameCategory.TournamentChampion, null, champion.Player, champion.Nickname, champion.TrophyCount, ended.DisplayName);
            }

            _activeChampionshipIndex = -1;
            ActiveChampionshipChanged?.Invoke();
        }

        /// <summary>
        /// Grants the ended championship's headline reward to the local
        /// player wallet for the two currency-shaped <see cref="RewardType"/>
        /// values (Coins/Gems) via <see cref="EconomyManager"/> — the only
        /// two reward types this project has a real wallet for today.
        /// Cosmetic/Title/Badge/Frame/Effect rewards are announced via the
        /// Rewards Ready notification above but not yet auto-granted to a
        /// persistent inventory (see Sprint 9 report Remaining TODOs).
        /// </summary>
        private static void ApplyHeadlineReward(ChampionshipCatalogConfig.ChampionshipEntry entry)
        {
            if (entry == null || entry.RewardAmount <= 0 || EconomyManager.Instance == null)
            {
                return;
            }

            switch (entry.RewardType)
            {
                case RewardType.Coins:
                    EconomyManager.Instance.AddCoins(entry.RewardAmount);
                    break;
                case RewardType.Gems:
                    EconomyManager.Instance.AddGems(entry.RewardAmount);
                    break;
            }
        }

        /// <summary>Debug/demo hook: ends the current championship (if any) and starts the next one in the catalog, wrapping around.</summary>
        public void AdvanceToNextChampionship()
        {
            if (championshipCatalog == null || championshipCatalog.Championships.Count == 0)
            {
                return;
            }

            int nextIndex = (_activeChampionshipIndex + 1) % championshipCatalog.Championships.Count;
            EndActiveChampionship();
            StartChampionship(nextIndex);
        }

        public void StartCountryEvent(int index)
        {
            if (countryEventCatalog == null || index < 0 || index >= countryEventCatalog.Events.Count)
            {
                return;
            }

            _activeCountryEventIndex = index;
            ActiveCountryEventChanged?.Invoke();
            NotificationManager.Instance?.Raise(NotificationType.NewEvent, ActiveCountryEvent.DisplayName + " is now live!");
        }

        /// <summary>Debug/demo hook: steps to the next Country Event in the catalog, wrapping around.</summary>
        public void AdvanceToNextCountryEvent()
        {
            if (countryEventCatalog == null || countryEventCatalog.Events.Count == 0)
            {
                return;
            }

            int nextIndex = (_activeCountryEventIndex + 1) % countryEventCatalog.Events.Count;
            StartCountryEvent(nextIndex);
        }
    }
}
