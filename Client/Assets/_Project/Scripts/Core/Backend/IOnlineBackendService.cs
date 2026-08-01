using System;
using System.Collections.Generic;
using GulfRun.Domain;

namespace GulfRun.Core.Backend
{
    /// <summary>
    /// The single seam every Sprint 9 online-ecosystem manager (Leaderboard/
    /// Friend/HallOfFame/Profile-lookup) talks to instead of any concrete
    /// backend — the same "swap the implementation, zero caller changes"
    /// contract <see cref="Networking.IMatchTransport"/> already gives
    /// realtime match sync. "Prepare scalable backend architecture ...
    /// Cloud Ready" (Sprint 9 brief) is satisfied by this abstraction
    /// itself: <see cref="OnlineBackendService.Current"/> can be pointed at
    /// a real HTTP/gRPC-backed implementation later with no change to any
    /// manager in Features.Online. <see cref="Networking.LocalLoopbackTransport"/>'s
    /// role is played today by <see cref="LocalOnlineBackendService"/>, an
    /// in-memory mock seeded with fake players so every screen (Leaderboard,
    /// Search, Hall of Fame, ...) has real data to render and interact with.
    /// </summary>
    public interface IOnlineBackendService
    {
        // --- Leaderboards & Rankings ---

        /// <summary>Top <paramref name="topN"/> entries for a scope, optionally filtered to one country (required/used only for <see cref="RankingScope.Country"/>).</summary>
        IReadOnlyList<LeaderboardEntry> GetLeaderboard(RankingScope scope, GulfCountry? country, int topN);

        /// <summary>1-based rank of a specific player within a scope, or -1 if unranked/unknown.</summary>
        int GetPlayerRank(RankingScope scope, GulfCountry? country, PlayerId player);

        CountrySummary GetCountrySummary(GulfCountry country);

        /// <summary>Raised whenever a scope's underlying standings change (drives cache invalidation — see <c>Features.Online.Leaderboard.LeaderboardManager</c>).</summary>
        event Action<RankingScope> LeaderboardUpdated;

        // --- Profiles & Search ---

        bool TryGetProfile(PlayerId player, out PlayerProfileSummary profile);

        /// <summary>Publishes/refreshes the local player's own profile snapshot so other lookups (Search, Friends, Leaderboard-linked profile views) see up-to-date data.</summary>
        void UpsertProfile(PlayerProfileSummary profile);

        /// <summary>Matches by Nickname (contains, case-insensitive), exact Player ID, or Country name — per the brief's Search section.</summary>
        IReadOnlyList<PlayerProfileSummary> SearchPlayers(string query);

        // --- Friends ---

        IReadOnlyList<PlayerId> GetFriends(PlayerId player);

        IReadOnlyList<FriendRequest> GetIncomingRequests(PlayerId player);

        IReadOnlyList<FriendRequest> GetOutgoingRequests(PlayerId player);

        IReadOnlyList<PlayerId> GetBlockedPlayers(PlayerId player);

        FriendLinkState GetLinkState(PlayerId viewer, PlayerId other);

        void SendFriendRequest(PlayerId from, PlayerId to);

        void AcceptFriendRequest(PlayerId from, PlayerId to);

        void RejectFriendRequest(PlayerId from, PlayerId to);

        void CancelFriendRequest(PlayerId from, PlayerId to);

        void RemoveFriend(PlayerId a, PlayerId b);

        void BlockPlayer(PlayerId from, PlayerId blocked);

        /// <summary>Raised after any friend/request/block mutation, for any of the players involved.</summary>
        event Action FriendsChanged;

        // --- Hall of Fame ---

        IReadOnlyList<HallOfFameEntry> GetHallOfFame();

        /// <summary>Appends a new permanent record. Never removes or overwrites an existing one — see <see cref="HallOfFameEntry"/> remarks.</summary>
        void RecordHallOfFameEntry(HallOfFameEntry entry);

        event Action HallOfFameChanged;
    }
}
