using System;
using System.Collections.Generic;
using GulfRun.Domain;

namespace GulfRun.Core.Backend
{
    /// <summary>
    /// In-memory mock <see cref="IOnlineBackendService"/> — the Sprint 9
    /// counterpart to <see cref="Networking.LocalLoopbackTransport"/>: seeds
    /// a believable population of fake players (10 per launch country) so
    /// every online screen has real, interactive data to render without a
    /// live backend, and implements every contract method with plain,
    /// allocation-light C# collections. No engine dependency at all — this
    /// class could run identically in a headless test.
    ///
    /// World vs. Gulf ranking today: every one of the 8 launch nations
    /// already IS a Gulf/MENA nation (see <see cref="GulfCountry"/>), so
    /// both scopes currently draw from the exact same pool and produce
    /// identical results — the scope *filter* is what's real and future-
    /// proof, not a coincidence of today's data (once a non-Gulf country
    /// registers a player, World and Gulf will genuinely diverge).
    ///
    /// Local player note: every scope (World/Gulf/Country/Weekly/Monthly/
    /// Seasonal) currently ranks the local player by the same single
    /// running trophy counter (<c>Features.Online.Leagues.LeagueManager</c>'s
    /// <see cref="SeasonProgress.TrophyCount"/>) rather than separately
    /// tracked time-windowed totals — a known simplification, see Sprint 9
    /// report Remaining TODOs.
    /// </summary>
    public sealed class LocalOnlineBackendService : IOnlineBackendService
    {
        private sealed class TrophyRecord
        {
            public string Nickname;
            public GulfCountry Country;
            public int AllTimeTrophies;
            public int WeeklyTrophies;
            public int MonthlyTrophies;
            public int SeasonalTrophies;
            public int Wins;
        }

        private readonly Dictionary<string, TrophyRecord> _trophies = new Dictionary<string, TrophyRecord>();
        private readonly Dictionary<string, PlayerProfileSummary> _profiles = new Dictionary<string, PlayerProfileSummary>();
        private readonly Dictionary<string, HashSet<string>> _friends = new Dictionary<string, HashSet<string>>();
        private readonly Dictionary<string, List<FriendRequest>> _incoming = new Dictionary<string, List<FriendRequest>>();
        private readonly Dictionary<string, List<FriendRequest>> _outgoing = new Dictionary<string, List<FriendRequest>>();
        private readonly Dictionary<string, HashSet<string>> _blocked = new Dictionary<string, HashSet<string>>();
        private readonly List<HallOfFameEntry> _hallOfFame = new List<HallOfFameEntry>();

        public event Action<RankingScope> LeaderboardUpdated;
        public event Action FriendsChanged;
        public event Action HallOfFameChanged;

        public LocalOnlineBackendService()
        {
            SeedFakePlayers();
            SeedHallOfFame();
        }

        // --- Leaderboards & Rankings ---

        public IReadOnlyList<LeaderboardEntry> GetLeaderboard(RankingScope scope, GulfCountry? country, int topN)
        {
            List<LeaderboardEntry> ordered = BuildOrderedEntries(scope, country);
            int count = ClampCount(ordered.Count, topN);
            var result = new List<LeaderboardEntry>(count);
            for (int i = 0; i < count; i++)
            {
                LeaderboardEntry entry = ordered[i];
                result.Add(new LeaderboardEntry(i + 1, entry.Player, entry.Nickname, entry.Country, entry.TrophyCount, entry.Wins));
            }

            return result;
        }

        public int GetPlayerRank(RankingScope scope, GulfCountry? country, PlayerId player)
        {
            List<LeaderboardEntry> ordered = BuildOrderedEntries(scope, country);
            for (int i = 0; i < ordered.Count; i++)
            {
                if (ordered[i].Player == player)
                {
                    return i + 1;
                }
            }

            return -1;
        }

        public CountrySummary GetCountrySummary(GulfCountry country)
        {
            int totalPlayers = 0;
            int totalWins = 0;
            int totalTrophies = 0;
            foreach (KeyValuePair<string, TrophyRecord> kvp in _trophies)
            {
                if (kvp.Value.Country != country)
                {
                    continue;
                }

                totalPlayers++;
                totalWins += kvp.Value.Wins;
                totalTrophies += kvp.Value.AllTimeTrophies;
            }

            return new CountrySummary(country, totalPlayers, totalWins, totalTrophies);
        }

        // --- Profiles & Search ---

        public bool TryGetProfile(PlayerId player, out PlayerProfileSummary profile) => _profiles.TryGetValue(player.Value, out profile);

        public void UpsertProfile(PlayerProfileSummary profile)
        {
            if (profile == null || profile.PlayerId.IsNone)
            {
                return;
            }

            _profiles[profile.PlayerId.Value] = profile;

            if (!_trophies.TryGetValue(profile.PlayerId.Value, out TrophyRecord record))
            {
                record = new TrophyRecord();
                _trophies[profile.PlayerId.Value] = record;
            }

            record.Nickname = profile.Nickname;
            record.Country = profile.Country;
            record.Wins = profile.TotalWins;
            record.AllTimeTrophies = profile.Season.TrophyCount;
            record.WeeklyTrophies = profile.Season.TrophyCount;
            record.MonthlyTrophies = profile.Season.TrophyCount;
            record.SeasonalTrophies = profile.Season.TrophyCount;

            LeaderboardUpdated?.Invoke(RankingScope.World);
            LeaderboardUpdated?.Invoke(RankingScope.Gulf);
            LeaderboardUpdated?.Invoke(RankingScope.Country);
            LeaderboardUpdated?.Invoke(RankingScope.Weekly);
            LeaderboardUpdated?.Invoke(RankingScope.Monthly);
            LeaderboardUpdated?.Invoke(RankingScope.Seasonal);
        }

        public IReadOnlyList<PlayerProfileSummary> SearchPlayers(string query)
        {
            var results = new List<PlayerProfileSummary>();
            if (string.IsNullOrWhiteSpace(query))
            {
                return results;
            }

            string needle = query.Trim();
            foreach (PlayerProfileSummary profile in _profiles.Values)
            {
                bool matchesNickname = profile.Nickname.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;
                bool matchesId = string.Equals(profile.PlayerId.Value, needle, StringComparison.OrdinalIgnoreCase);
                bool matchesCountry = profile.Country.ToString().IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;

                if (matchesNickname || matchesId || matchesCountry)
                {
                    results.Add(profile);
                    if (results.Count >= 50)
                    {
                        break;
                    }
                }
            }

            return results;
        }

        // --- Friends ---

        public IReadOnlyList<PlayerId> GetFriends(PlayerId player)
        {
            var result = new List<PlayerId>();
            if (_friends.TryGetValue(player.Value, out HashSet<string> set))
            {
                foreach (string id in set)
                {
                    result.Add(new PlayerId(id));
                }
            }

            return result;
        }

        public IReadOnlyList<FriendRequest> GetIncomingRequests(PlayerId player) =>
            _incoming.TryGetValue(player.Value, out List<FriendRequest> list) ? list : Array.Empty<FriendRequest>();

        public IReadOnlyList<FriendRequest> GetOutgoingRequests(PlayerId player) =>
            _outgoing.TryGetValue(player.Value, out List<FriendRequest> list) ? list : Array.Empty<FriendRequest>();

        public IReadOnlyList<PlayerId> GetBlockedPlayers(PlayerId player)
        {
            var result = new List<PlayerId>();
            if (_blocked.TryGetValue(player.Value, out HashSet<string> set))
            {
                foreach (string id in set)
                {
                    result.Add(new PlayerId(id));
                }
            }

            return result;
        }

        public FriendLinkState GetLinkState(PlayerId viewer, PlayerId other)
        {
            if (viewer == other)
            {
                return FriendLinkState.None;
            }

            if (IsBlocked(viewer, other))
            {
                return FriendLinkState.Blocked;
            }

            if (_friends.TryGetValue(viewer.Value, out HashSet<string> friends) && friends.Contains(other.Value))
            {
                return FriendLinkState.Friends;
            }

            if (HasPendingRequest(viewer, other))
            {
                return FriendLinkState.RequestSentByMe;
            }

            if (HasPendingRequest(other, viewer))
            {
                return FriendLinkState.RequestReceivedFromThem;
            }

            return FriendLinkState.None;
        }

        public void SendFriendRequest(PlayerId from, PlayerId to)
        {
            if (from.IsNone || to.IsNone || from == to)
            {
                return;
            }

            if (GetLinkState(from, to) != FriendLinkState.None)
            {
                return;
            }

            var request = new FriendRequest(from, to, FriendRequestStatus.Pending, DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            GetOrCreateList(_outgoing, from.Value).Add(request);
            GetOrCreateList(_incoming, to.Value).Add(request);
            FriendsChanged?.Invoke();
        }

        public void AcceptFriendRequest(PlayerId from, PlayerId to)
        {
            if (!RemovePendingRequest(from, to))
            {
                return;
            }

            GetOrCreateSet(_friends, from.Value).Add(to.Value);
            GetOrCreateSet(_friends, to.Value).Add(from.Value);
            FriendsChanged?.Invoke();
        }

        public void RejectFriendRequest(PlayerId from, PlayerId to)
        {
            if (RemovePendingRequest(from, to))
            {
                FriendsChanged?.Invoke();
            }
        }

        public void CancelFriendRequest(PlayerId from, PlayerId to)
        {
            if (RemovePendingRequest(from, to))
            {
                FriendsChanged?.Invoke();
            }
        }

        public void RemoveFriend(PlayerId a, PlayerId b)
        {
            bool changed = false;
            if (_friends.TryGetValue(a.Value, out HashSet<string> friendsOfA))
            {
                changed |= friendsOfA.Remove(b.Value);
            }

            if (_friends.TryGetValue(b.Value, out HashSet<string> friendsOfB))
            {
                changed |= friendsOfB.Remove(a.Value);
            }

            if (changed)
            {
                FriendsChanged?.Invoke();
            }
        }

        public void BlockPlayer(PlayerId from, PlayerId blocked)
        {
            if (from.IsNone || blocked.IsNone || from == blocked)
            {
                return;
            }

            GetOrCreateSet(_blocked, from.Value).Add(blocked.Value);
            RemoveFriend(from, blocked);
            RemovePendingRequest(from, blocked);
            RemovePendingRequest(blocked, from);
            FriendsChanged?.Invoke();
        }

        // --- Hall of Fame ---

        public IReadOnlyList<HallOfFameEntry> GetHallOfFame() => _hallOfFame;

        public void RecordHallOfFameEntry(HallOfFameEntry entry)
        {
            _hallOfFame.Add(entry);
            HallOfFameChanged?.Invoke();
        }

        // --- Internal helpers ---

        private bool IsBlocked(PlayerId a, PlayerId b) =>
            (_blocked.TryGetValue(a.Value, out HashSet<string> blockedByA) && blockedByA.Contains(b.Value)) ||
            (_blocked.TryGetValue(b.Value, out HashSet<string> blockedByB) && blockedByB.Contains(a.Value));

        private bool HasPendingRequest(PlayerId from, PlayerId to)
        {
            if (!_outgoing.TryGetValue(from.Value, out List<FriendRequest> list))
            {
                return false;
            }

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].To == to && list[i].Status == FriendRequestStatus.Pending)
                {
                    return true;
                }
            }

            return false;
        }

        private bool RemovePendingRequest(PlayerId from, PlayerId to)
        {
            bool removedAny = false;
            if (_outgoing.TryGetValue(from.Value, out List<FriendRequest> outgoingList))
            {
                removedAny |= outgoingList.RemoveAll(r => r.To == to && r.Status == FriendRequestStatus.Pending) > 0;
            }

            if (_incoming.TryGetValue(to.Value, out List<FriendRequest> incomingList))
            {
                removedAny |= incomingList.RemoveAll(r => r.From == from && r.Status == FriendRequestStatus.Pending) > 0;
            }

            return removedAny;
        }

        private static List<FriendRequest> GetOrCreateList(Dictionary<string, List<FriendRequest>> map, string key)
        {
            if (!map.TryGetValue(key, out List<FriendRequest> list))
            {
                list = new List<FriendRequest>();
                map[key] = list;
            }

            return list;
        }

        private static HashSet<string> GetOrCreateSet(Dictionary<string, HashSet<string>> map, string key)
        {
            if (!map.TryGetValue(key, out HashSet<string> set))
            {
                set = new HashSet<string>();
                map[key] = set;
            }

            return set;
        }

        private static int ClampCount(int available, int requested) => requested < 0 || requested > available ? available : requested;

        private List<LeaderboardEntry> BuildOrderedEntries(RankingScope scope, GulfCountry? country)
        {
            var list = new List<LeaderboardEntry>(_trophies.Count);
            foreach (KeyValuePair<string, TrophyRecord> kvp in _trophies)
            {
                TrophyRecord record = kvp.Value;
                if (scope == RankingScope.Country && country.HasValue && record.Country != country.Value)
                {
                    continue;
                }

                int trophyValue = ResolveScopedTrophies(scope, record);
                list.Add(new LeaderboardEntry(0, new PlayerId(kvp.Key), record.Nickname, record.Country, trophyValue, record.Wins));
            }

            list.Sort((a, b) => b.TrophyCount != a.TrophyCount ? b.TrophyCount.CompareTo(a.TrophyCount) : string.CompareOrdinal(a.Nickname, b.Nickname));
            return list;
        }

        private static int ResolveScopedTrophies(RankingScope scope, TrophyRecord record)
        {
            switch (scope)
            {
                case RankingScope.Weekly:
                    return record.WeeklyTrophies;
                case RankingScope.Monthly:
                    return record.MonthlyTrophies;
                case RankingScope.Seasonal:
                    return record.SeasonalTrophies;
                default:
                    return record.AllTimeTrophies;
            }
        }

        private void SeedFakePlayers()
        {
            var random = new System.Random(20260731);
            GulfCountry[] countries =
            {
                GulfCountry.SaudiArabia, GulfCountry.Kuwait, GulfCountry.UnitedArabEmirates, GulfCountry.Qatar,
                GulfCountry.Bahrain, GulfCountry.Oman, GulfCountry.Iraq, GulfCountry.Egypt
            };
            string[] codes = { "KSA", "KWT", "UAE", "QAT", "BHR", "OMN", "IRQ", "EGY" };

            for (int c = 0; c < countries.Length; c++)
            {
                for (int i = 1; i <= 10; i++)
                {
                    string id = "BOT-" + codes[c] + "-" + i.ToString("00");
                    int allTime = random.Next(50, 5000);
                    var record = new TrophyRecord
                    {
                        Nickname = codes[c] + "Racer" + i.ToString("00"),
                        Country = countries[c],
                        AllTimeTrophies = allTime,
                        WeeklyTrophies = random.Next(0, 400),
                        MonthlyTrophies = random.Next(0, 1200),
                        SeasonalTrophies = allTime,
                        Wins = random.Next(0, 300)
                    };
                    _trophies[id] = record;

                    var profile = new PlayerProfileSummary
                    {
                        PlayerId = new PlayerId(id),
                        Nickname = record.Nickname,
                        Country = record.Country,
                        CurrentCharacterDisplayName = "Character 01",
                        CurrentOutfitDisplayName = codes[c] + " Traditional Outfit",
                        Season = new SeasonProgress(1, LeagueRules.ResolveLeague(allTime, DefaultLeagueThresholds), allTime),
                        TotalWins = record.Wins,
                        Top3Finishes = record.Wins + random.Next(0, 150),
                        WinRate = record.Wins > 0 ? Math.Min(1f, record.Wins / 400f) : 0f,
                        BestFinishTimeSeconds = 45f + (float)random.NextDouble() * 60f,
                        Coins = random.Next(100, 20000),
                        Gems = random.Next(0, 800),
                        FavouriteCharacterDisplayName = "Character 01",
                        Status = (i % 4 == 0) ? OnlineStatus.Online : OnlineStatus.Offline
                    };
                    _profiles[id] = profile;
                }
            }
        }

        /// <summary>
        /// Mirrors <c>Features.Online.Configuration.LeagueCatalogConfig</c>'s
        /// default thresholds purely for seeding believable fake League
        /// values — the real local player's League always comes from the
        /// actual configured catalog via <c>LeagueManager</c>, never from here.
        /// </summary>
        private static readonly int[] DefaultLeagueThresholds = { 0, 100, 250, 500, 900, 1500, 2500, 4000 };

        private void SeedHallOfFame()
        {
            List<LeaderboardEntry> worldTop = BuildOrderedEntries(RankingScope.World, null);
            if (worldTop.Count == 0)
            {
                return;
            }

            LeaderboardEntry best = worldTop[0];
            _hallOfFame.Add(new HallOfFameEntry(HallOfFameCategory.BestInWorld, null, best.Player, best.Nickname, best.TrophyCount, "Season 1"));
            _hallOfFame.Add(new HallOfFameEntry(HallOfFameCategory.BestInGulf, null, best.Player, best.Nickname, best.TrophyCount, "Season 1"));

            GulfCountry[] countries =
            {
                GulfCountry.SaudiArabia, GulfCountry.Kuwait, GulfCountry.UnitedArabEmirates, GulfCountry.Qatar,
                GulfCountry.Bahrain, GulfCountry.Oman, GulfCountry.Iraq, GulfCountry.Egypt
            };

            for (int c = 0; c < countries.Length; c++)
            {
                List<LeaderboardEntry> countryTop = BuildOrderedEntries(RankingScope.Country, countries[c]);
                if (countryTop.Count == 0)
                {
                    continue;
                }

                LeaderboardEntry topOfCountry = countryTop[0];
                _hallOfFame.Add(new HallOfFameEntry(HallOfFameCategory.BestInCountry, countries[c], topOfCountry.Player, topOfCountry.Nickname, topOfCountry.TrophyCount, "Season 1"));
            }

            _hallOfFame.Add(new HallOfFameEntry(HallOfFameCategory.WeeklyChampion, null, best.Player, best.Nickname, best.TrophyCount, "Week 1"));
            _hallOfFame.Add(new HallOfFameEntry(HallOfFameCategory.MonthlyChampion, null, best.Player, best.Nickname, best.TrophyCount, "Month 1"));
            _hallOfFame.Add(new HallOfFameEntry(HallOfFameCategory.SeasonChampion, null, best.Player, best.Nickname, best.TrophyCount, "Season 1"));
            _hallOfFame.Add(new HallOfFameEntry(HallOfFameCategory.TournamentChampion, null, best.Player, best.Nickname, best.TrophyCount, "Launch Tournament"));
        }
    }
}
