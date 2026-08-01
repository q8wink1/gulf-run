using GulfRun.Core;
using GulfRun.Core.Managers;
using GulfRun.Core.Networking;
using GulfRun.Domain;
using GulfRun.Features.Multiplayer.Configuration;
using GulfRun.Features.Multiplayer.Identification;
using GulfRun.Features.Multiplayer.Lobby;
using GulfRun.Features.Multiplayer.Match;
using UnityEngine;

namespace GulfRun.Features.Multiplayer.Session
{
    /// <summary>
    /// Top-level entry point for the Match Flow: Create Match, Join Match,
    /// Leave Match, Cancel Matchmaking. Everything else (Lobby roster, Ready
    /// System, shared countdown, connection health) reacts to the
    /// <see cref="IMatchTransport"/> events these calls trigger — this class
    /// is the composition root for the Multiplayer feature, the same role
    /// <c>GameLoopController</c> plays for single-player.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SessionManager : Singleton<SessionManager>
    {
        [SerializeField] private NetworkSyncConfig config;

        [Tooltip("Fallback nationality used only if a match is created/joined before Account Creation has run (should not normally happen — Account Creation gates entry into any match). Sprint 8 supersedes the old freely-settable per-session country field: the real value always comes from the permanently-locked SaveManager account once one exists.")]
        [SerializeField] private GulfCountry fallbackPlayerCountry = GulfCountry.SaudiArabia;

        private LobbyManager _lobby;
        private MatchManager _match;

        public bool IsInMatch { get; private set; }
        public bool IsHost { get; private set; }
        public bool IsMatchmaking { get; private set; }
        public PlayerIdentity LocalIdentity { get; private set; }

        /// <summary>
        /// The local player's permanent nationality (Sprint 8: "Country
        /// becomes permanently linked to the account... cannot be changed
        /// later"). Sourced from the one-time-created <see cref="SaveManager"/>
        /// account; falls back to <see cref="fallbackPlayerCountry"/> only if
        /// no account exists yet, which should never happen once Account
        /// Creation (<c>Features.Character.Account.AccountCreationView</c>)
        /// gates the flow.
        /// </summary>
        public GulfCountry LocalPlayerCountry =>
            SaveManager.Instance != null && SaveManager.Instance.HasAccount
                ? SaveManager.Instance.GetAccount().Country
                : fallbackPlayerCountry;

        protected override void OnInitialize()
        {
            _lobby = GetComponent<LobbyManager>();
            _match = GetComponent<MatchManager>();
        }

        /// <summary>Creates a brand-new match and becomes its host.</summary>
        public void CreateMatch(string displayName)
        {
            if (IsInMatch || IsMatchmaking)
            {
                return;
            }

            IsMatchmaking = true;
            LocalIdentity = LocalPlayerIdentity.CreateLocal(displayName, LocalPlayerCountry);

            int maxPlayers = config != null ? config.MaxPlayers : 4;
            MatchTransportService.Current.StartHost(LocalIdentity, maxPlayers);

            IsInMatch = true;
            IsHost = true;
            IsMatchmaking = false;
        }

        /// <summary>
        /// Joins an existing match as a client. Under the default
        /// <see cref="LocalLoopbackTransport"/> there is no real remote host
        /// to reach (see its JoinAsClient doc comment) — this call is wired
        /// end-to-end and ready for a real transport, but will not complete
        /// a join until one is registered via <see cref="MatchTransportService"/>.
        /// </summary>
        public void JoinMatch(string joinCode, string displayName)
        {
            if (IsInMatch || IsMatchmaking)
            {
                return;
            }

            IsMatchmaking = true;
            LocalIdentity = LocalPlayerIdentity.CreateLocal(displayName, LocalPlayerCountry);

            MatchTransportService.Current.JoinAsClient(LocalIdentity, joinCode);

            IsMatchmaking = false;
        }

        /// <summary>Cancels a pending Create/Join before a connection has been established.</summary>
        public void CancelMatchmaking()
        {
            IsMatchmaking = false;
        }

        /// <summary>Leaves the current match. Works identically whether the local player is the host or not.</summary>
        public void LeaveMatch()
        {
            if (!IsInMatch)
            {
                return;
            }

            DisconnectReason reason = IsHost ? DisconnectReason.HostLeft : DisconnectReason.PlayerLeft;
            MatchTransportService.Current.Disconnect(reason);

            _lobby?.Clear();
            _match?.ResetMatch();

            IsInMatch = false;
            IsHost = false;
        }

        /// <summary>Sets the local participant's Ready System state; the host's MatchManager decides when to start the countdown.</summary>
        public void SetLocalReady(PlayerReadyState state)
        {
            if (!IsInMatch)
            {
                return;
            }

            MatchTransportService.Current.SetLocalReadyState(state);
        }
    }
}
