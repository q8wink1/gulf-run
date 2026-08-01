using System;
using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Backend;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Online.HallOfFame
{
    /// <summary>
    /// Thin, cached-list read wrapper over
    /// <see cref="IOnlineBackendService"/>'s permanent Hall of Fame ledger —
    /// "a player's achievement remains permanently recorded even after
    /// losing Rank #1" (Sprint 9 brief) is guaranteed entirely by
    /// <c>Core.Backend.LocalOnlineBackendService</c> only ever appending
    /// entries, never deleting/overwriting them; this manager just exposes
    /// that list plus a convenience recording call for
    /// <c>Championships.ChampionshipManager</c> to use when a championship ends.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class HallOfFameManager : Singleton<HallOfFameManager>
    {
        public event Action Changed;

        private IOnlineBackendService _backend;

        protected override void OnInitialize()
        {
        }

        private void OnEnable()
        {
            _backend = OnlineBackendService.Current;
            _backend.HallOfFameChanged += HandleChanged;
        }

        private void OnDisable()
        {
            if (_backend != null)
            {
                _backend.HallOfFameChanged -= HandleChanged;
            }
        }

        public IReadOnlyList<HallOfFameEntry> GetEntries() => OnlineBackendService.Current.GetHallOfFame();

        public void RecordEntry(HallOfFameCategory category, GulfCountry? country, PlayerId player, string nickname, int score, string achievedLabel) =>
            OnlineBackendService.Current.RecordHallOfFameEntry(new HallOfFameEntry(category, country, player, nickname, score, achievedLabel));

        private void HandleChanged() => Changed?.Invoke();
    }
}
