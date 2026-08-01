using System;
using System.Collections.Generic;
using GulfRun.Core;
using GulfRun.Core.Backend;
using GulfRun.Domain;
using UnityEngine;

namespace GulfRun.Features.Online.Leaderboard
{
    /// <summary>
    /// The brief's Performance section ("Fast leaderboard loading, Caching,
    /// optimized database queries, minimal API calls") implemented as a
    /// thin time-based cache in front of <see cref="OnlineBackendService"/>:
    /// repeated requests for the same scope/country/topN within
    /// <see cref="cacheDurationSeconds"/> never re-hit the backend, and any
    /// backend-reported change to a scope (<see cref="IOnlineBackendService.LeaderboardUpdated"/>)
    /// proactively invalidates just that scope's cached entries rather than
    /// waiting for the whole cache to expire.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class LeaderboardManager : Singleton<LeaderboardManager>
    {
        private readonly struct CacheKey : IEquatable<CacheKey>
        {
            public readonly RankingScope Scope;
            public readonly int CountryOrdinal;
            public readonly int TopN;

            public CacheKey(RankingScope scope, GulfCountry? country, int topN)
            {
                Scope = scope;
                CountryOrdinal = country.HasValue ? (int)country.Value : -1;
                TopN = topN;
            }

            public bool Equals(CacheKey other) => Scope == other.Scope && CountryOrdinal == other.CountryOrdinal && TopN == other.TopN;

            public override bool Equals(object obj) => obj is CacheKey other && Equals(other);

            public override int GetHashCode() => ((int)Scope * 397) ^ (CountryOrdinal * 31) ^ TopN;
        }

        private sealed class CacheEntry
        {
            public IReadOnlyList<LeaderboardEntry> Entries;
            public double CachedAtSeconds;
        }

        [SerializeField] private float cacheDurationSeconds = 5f;

        private readonly Dictionary<CacheKey, CacheEntry> _cache = new Dictionary<CacheKey, CacheEntry>();
        private IOnlineBackendService _backend;

        public double LastRefreshedAtSeconds { get; private set; }

        public event Action Refreshed;

        protected override void OnInitialize()
        {
        }

        private void OnEnable()
        {
            _backend = OnlineBackendService.Current;
            _backend.LeaderboardUpdated += HandleBackendUpdated;
        }

        private void OnDisable()
        {
            if (_backend != null)
            {
                _backend.LeaderboardUpdated -= HandleBackendUpdated;
            }
        }

        public IReadOnlyList<LeaderboardEntry> GetLeaderboard(RankingScope scope, GulfCountry? country, int topN)
        {
            var key = new CacheKey(scope, country, topN);
            if (_cache.TryGetValue(key, out CacheEntry cached) && Time.timeAsDouble - cached.CachedAtSeconds < cacheDurationSeconds)
            {
                return cached.Entries;
            }

            IReadOnlyList<LeaderboardEntry> fresh = OnlineBackendService.Current.GetLeaderboard(scope, country, topN);
            _cache[key] = new CacheEntry { Entries = fresh, CachedAtSeconds = Time.timeAsDouble };
            LastRefreshedAtSeconds = Time.timeAsDouble;
            Refreshed?.Invoke();
            return fresh;
        }

        /// <summary>Not cached — a single-player point lookup is cheap and infrequent compared to a Top-N list fetch, so the added bookkeeping isn't worth it.</summary>
        public int GetPlayerRank(RankingScope scope, GulfCountry? country, PlayerId player) =>
            OnlineBackendService.Current.GetPlayerRank(scope, country, player);

        public CountrySummary GetCountrySummary(GulfCountry country) => OnlineBackendService.Current.GetCountrySummary(country);

        public void ForceRefresh(RankingScope scope)
        {
            var staleKeys = new List<CacheKey>();
            foreach (KeyValuePair<CacheKey, CacheEntry> kvp in _cache)
            {
                if (kvp.Key.Scope == scope)
                {
                    staleKeys.Add(kvp.Key);
                }
            }

            for (int i = 0; i < staleKeys.Count; i++)
            {
                _cache.Remove(staleKeys[i]);
            }
        }

        private void HandleBackendUpdated(RankingScope scope) => ForceRefresh(scope);
    }
}
