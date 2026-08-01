using System;
using GulfRun.Core;
using GulfRun.Core.Backend;
using GulfRun.Core.Managers;
using GulfRun.Core.Networking;
using GulfRun.Core.Services;
using GulfRun.Domain;
using GulfRun.Features.Online.Championships;
using GulfRun.Features.Online.Leaderboard;
using GulfRun.Features.Online.Leagues;
using GulfRun.Features.Online.Statistics;
using UnityEngine;

namespace GulfRun.Features.Online.Profile
{
    /// <summary>
    /// Composition root for the Sprint 9 Player Profile screen: rebuilds
    /// the local player's single <see cref="PlayerProfileSummary"/> from
    /// every other system (Account/Country from <see cref="SaveManager"/>,
    /// Character/Outfit from <see cref="ILocalLoadoutProvider"/>, League/
    /// Season from <see cref="LeagueManager"/>, Ranks from
    /// <see cref="LeaderboardManager"/>, stats from
    /// <see cref="PlayerStatisticsTracker"/>, Coins/Gems from
    /// <see cref="EconomyManager"/>) on a throttled timer (never every
    /// frame — see <see cref="refreshIntervalSeconds"/>) and publishes it
    /// to <see cref="OnlineBackendService"/> so Search/Leaderboard/Friends
    /// screens all see up-to-date data for this player too.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ProfileManager : Singleton<ProfileManager>
    {
        [SerializeField] private float refreshIntervalSeconds = 1f;

        private double _lastRefreshTimeSeconds = double.NegativeInfinity;
        private bool _accountReady;
        private MatchState _lastKnownMatchState = MatchState.Waiting;

        public PlayerProfileSummary LocalProfile { get; private set; }

        public event Action LocalProfileChanged;

        public PlayerId LocalPlayerId =>
            SaveManager.Instance != null && SaveManager.Instance.HasAccount ? SaveManager.Instance.GetAccount().PlayerId : PlayerId.None;

        protected override void OnInitialize()
        {
        }

        private void OnEnable()
        {
            IMatchTransport transport = MatchTransportService.Current;
            if (transport != null)
            {
                transport.MatchStateChanged += HandleMatchStateChanged;
            }
        }

        private void OnDisable()
        {
            IMatchTransport transport = MatchTransportService.Current;
            if (transport != null)
            {
                transport.MatchStateChanged -= HandleMatchStateChanged;
            }
        }

        private void Update()
        {
            if (SaveManager.Instance == null || !SaveManager.Instance.HasAccount)
            {
                return;
            }

            if (!_accountReady)
            {
                _accountReady = true;
                RefreshLocalProfile();
                return;
            }

            if (Time.timeAsDouble - _lastRefreshTimeSeconds >= refreshIntervalSeconds)
            {
                RefreshLocalProfile();
            }
        }

        /// <summary>Forces an immediate rebuild — call after an action that should be reflected right away (e.g. right after Account Creation, right after a race ends).</summary>
        public void RefreshLocalProfile()
        {
            if (SaveManager.Instance == null || !SaveManager.Instance.HasAccount)
            {
                return;
            }

            PlayerAccount account = SaveManager.Instance.GetAccount();
            PlayerId localId = account.PlayerId;
            ILocalLoadoutProvider loadout = LocalLoadoutProviderService.Current;
            PlayerMatchStatistics stats = PlayerStatisticsTracker.Instance != null ? PlayerStatisticsTracker.Instance.Statistics : null;
            SeasonProgress season = LeagueManager.Instance != null ? LeagueManager.Instance.Progress : SeasonProgress.Initial(1);

            var profile = new PlayerProfileSummary
            {
                PlayerId = localId,
                Nickname = account.DisplayName,
                Country = account.Country,
                CurrentCharacterDisplayName = loadout != null ? loadout.CurrentCharacterDisplayName : string.Empty,
                CurrentOutfitDisplayName = loadout != null ? loadout.CurrentOutfitDisplayName : string.Empty,
                Season = season,
                WorldRank = ResolveRank(RankingScope.World, null, localId),
                GulfRank = ResolveRank(RankingScope.Gulf, null, localId),
                CountryRank = ResolveRank(RankingScope.Country, account.Country, localId),
                TotalWins = stats != null ? stats.Wins : 0,
                Top3Finishes = stats != null ? stats.Top3Finishes : 0,
                WinRate = stats != null ? stats.WinRate : 0f,
                BestFinishTimeSeconds = stats != null ? stats.BestFinishTimeSeconds : -1f,
                Coins = EconomyManager.Instance != null ? EconomyManager.Instance.Coins : 0,
                Gems = EconomyManager.Instance != null ? EconomyManager.Instance.Gems : 0,
                FavouriteCharacterDisplayName = ResolveFavouriteCharacterDisplayName(stats, loadout),
                Status = ResolveOnlineStatus()
            };

            LocalProfile = profile;
            OnlineBackendService.Current.UpsertProfile(profile);
            _lastRefreshTimeSeconds = Time.timeAsDouble;
            LocalProfileChanged?.Invoke();
        }

        public bool TryGetProfile(PlayerId playerId, out PlayerProfileSummary profile)
        {
            if (playerId == LocalPlayerId && LocalProfile != null)
            {
                profile = LocalProfile;
                return true;
            }

            return OnlineBackendService.Current.TryGetProfile(playerId, out profile);
        }

        private static int ResolveRank(RankingScope scope, GulfCountry? country, PlayerId localId) =>
            LeaderboardManager.Instance != null ? LeaderboardManager.Instance.GetPlayerRank(scope, country, localId) : -1;

        /// <summary>
        /// Favourite = the character played in the most matches
        /// (<see cref="PlayerMatchStatistics.ResolveFavouriteCharacter"/>).
        /// Its display name can only be resolved here when it happens to be
        /// the currently-equipped one (via <see cref="ILocalLoadoutProvider"/>) —
        /// Features.Online never references Features.Character's catalog
        /// directly, so a historical favourite that differs from today's
        /// equipped character falls back to its raw id (see Sprint 9 report
        /// Remaining TODOs).
        /// </summary>
        private static string ResolveFavouriteCharacterDisplayName(PlayerMatchStatistics stats, ILocalLoadoutProvider loadout)
        {
            if (stats != null)
            {
                CharacterId favourite = stats.ResolveFavouriteCharacter();
                if (!favourite.IsNone)
                {
                    if (loadout != null && loadout.CurrentCharacterId == favourite)
                    {
                        return loadout.CurrentCharacterDisplayName;
                    }

                    return favourite.Value;
                }
            }

            return loadout != null ? loadout.CurrentCharacterDisplayName : string.Empty;
        }

        private OnlineStatus ResolveOnlineStatus()
        {
            IMatchTransport transport = MatchTransportService.Current;
            bool transportActive = transport != null && transport.IsActive;
            bool tournamentContext = transportActive && ChampionshipManager.Instance != null && ChampionshipManager.Instance.HasActiveChampionship;
            bool hasAccount = SaveManager.Instance != null && SaveManager.Instance.HasAccount;
            return OnlineStatusResolver.Resolve(hasAccount, transportActive, _lastKnownMatchState, tournamentContext);
        }

        private void HandleMatchStateChanged(MatchState state) => _lastKnownMatchState = state;
    }
}
