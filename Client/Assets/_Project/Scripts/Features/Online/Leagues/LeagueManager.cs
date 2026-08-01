using System;
using GulfRun.Core;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.Online.Configuration;
using GulfRun.Features.Online.Notifications;
using UnityEngine;

namespace GulfRun.Features.Online.Leagues
{
    /// <summary>
    /// Owns the local player's <see cref="SeasonProgress"/> (League tier +
    /// trophy count for the current season): updates it from
    /// <see cref="PlayerStatEventService.LocalMatchCompleted"/> using the
    /// pure <see cref="LeagueRules"/> math against <see cref="LeagueCatalogConfig"/>'s
    /// authored thresholds, and raises a Promotion/Relegation
    /// <see cref="NotificationManager"/> entry the instant the resolved
    /// tier actually changes. In-memory only today (no season-rollover
    /// scheduler yet) — see Sprint 9 report Remaining TODOs.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class LeagueManager : Singleton<LeagueManager>
    {
        [SerializeField] private LeagueCatalogConfig catalog;
        [SerializeField] private int startingSeasonNumber = 1;

        private SeasonProgress _progress;

        public SeasonProgress Progress => _progress;

        public LeagueCatalogConfig Catalog => catalog;

        public event Action<SeasonProgress> ProgressChanged;

        protected override void OnInitialize()
        {
            _progress = SeasonProgress.Initial(startingSeasonNumber);
        }

        private void OnEnable() => PlayerStatEventService.LocalMatchCompleted += HandleLocalMatchCompleted;

        private void OnDisable() => PlayerStatEventService.LocalMatchCompleted -= HandleLocalMatchCompleted;

        private void HandleLocalMatchCompleted(PlayerMatchOutcome outcome)
        {
            League previousLeague = _progress.CurrentLeague;

            int delta = LeagueRules.ComputeTrophyDelta(outcome.FinishPosition);
            int newTrophyCount = _progress.TrophyCount + delta;
            if (newTrophyCount < 0)
            {
                newTrophyCount = 0;
            }

            League resolvedLeague = LeagueRules.ResolveLeague(newTrophyCount, catalog != null ? catalog.Thresholds : null);
            _progress = new SeasonProgress(_progress.SeasonNumber, resolvedLeague, newTrophyCount);
            ProgressChanged?.Invoke(_progress);

            if (resolvedLeague > previousLeague)
            {
                NotificationManager.Instance?.Raise(NotificationType.Promotion, "Promoted to " + LeagueDisplayName(resolvedLeague) + "!");
            }
            else if (resolvedLeague < previousLeague)
            {
                NotificationManager.Instance?.Raise(NotificationType.Relegation, "Relegated to " + LeagueDisplayName(resolvedLeague) + ".");
            }
        }

        public string LeagueDisplayName(League league) => catalog != null ? catalog.GetDisplayName(league) : league.ToString();
    }
}
