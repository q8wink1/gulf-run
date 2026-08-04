using System;
using System.Collections.Generic;
using GulfRun.Domain;

namespace GulfRun.Core.Services
{
    /// <summary>
    /// Read-only + action seam onto the whole Match Flow: Sprint 13's PLAY
    /// button/bottom bar Room Code/Matchmaking ETA/"SOCIAL: Room Code"/
    /// "Invite button", and Sprint 15's full Matchmaking, Private Room &amp;
    /// Pre-Race Lobby surface (roster, Ready System, Bot Fill, Kick Player,
    /// host-authoritative Start Match, Quick Chat, connection quality).
    /// Implemented entirely by
    /// <c>Features.Multiplayer.Session.SessionManager</c> — the Match
    /// Flow's composition root — so both Features.MainMenu (Play/Private
    /// Room entry points) and the new Features.Matchmaking (the Pre-Race
    /// Lobby scene) can drive/observe the whole flow without either ever
    /// referencing Features.Multiplayer directly, the same shape as
    /// <see cref="ILocalLoadoutProvider"/>.
    /// </summary>
    public interface IMatchLobbySummaryProvider
    {
        bool IsInMatch { get; }
        bool IsMatchmaking { get; }

        /// <summary>True while the local participant's own roster entry has <c>MatchParticipant.IsHost</c> set — always up to date across a Sprint 15 host migration, never a manually-tracked flag.</summary>
        bool IsHost { get; }

        /// <summary>Sprint 15: true once the current match was created/joined as an invitation-only Private Room rather than public Quick Play matchmaking (see <see cref="MatchCreationMode"/>).</summary>
        bool IsPrivateRoom { get; }

        /// <see cref="RoomCode.None"/> when not currently hosting.
        RoomCode LocalRoomCode { get; }

        /// <summary>Sprint 15 (Debug "Match ID"): a fresh identifier rolled every time a match is created; empty string when not in a match.</summary>
        string MatchId { get; }

        int LobbyPlayerCount { get; }
        int RequiredPlayerCount { get; }

        /// <summary>Sprint 15 (Bot Settings "Minimum players required: 2"). The configured minimum real+bot player count the Ready System/Auto Start require.</summary>
        int MinimumPlayerCount { get; }

        /// <summary>Sprint 15: the local transport connection id, or -1 when not in a match. Lets Pre-Race Lobby UI tell "this is me" apart from other <see cref="Participants"/> rows.</summary>
        int LocalConnectionId { get; }

        /// <summary>Sprint 15 (Ready System). The local participant's own current Ready/Not Ready state.</summary>
        PlayerReadyState LocalReadyState { get; }

        /// <summary>Sprint 15 (Bot Settings toggle). Whether the Room Owner has Fill Empty Slots With Bots switched on for the current Private Room.</summary>
        bool BotFillEnabled { get; }

        /// <summary>Sprint 15 (Auto Start). Reuses <see cref="MatchState.Waiting"/>/<see cref="MatchState.Countdown"/>/<see cref="MatchState.Running"/> as the Pre-Race Lobby's own phase: Waiting = joining/readying, Countdown = the shared 5-4-3-2-1-GO, Running = "load Gameplay now".</summary>
        MatchState LobbyPhase { get; }

        /// <summary>Sprint 15 (Auto Start "5 4 3 2 1 GO"). Only meaningful while <see cref="LobbyPhase"/> is <see cref="MatchState.Countdown"/>.</summary>
        int AutoStartCountdownSecondsRemaining { get; }

        /// <summary>Sprint 15 (Ready System "Room Owner cannot start unless... Everyone Ready"). For Quick Play this means a full 4/4 lobby with every seat Ready; Private Room still uses the configured minimum.</summary>
        bool AllPlayersReady { get; }

        /// <summary>Mock Quick Play search status copy for the QuickPlay searching screen (empty when idle).</summary>
        string MatchmakingStatusMessage { get; }

        /// <summary>True after a Quick Play search resolved by joining an existing public room rather than creating one.</summary>
        bool JoinedExistingPublicRoom { get; }

        /// <summary>Sprint 15 (Player Cards). Live roster snapshot — empty collection when not in a match.</summary>
        IReadOnlyCollection<MatchParticipant> Participants { get; }

        /// <summary>Raised after any roster/ready/bot-fill/phase change, so Pre-Race Lobby views can refresh without polling every field every OnGUI frame if they prefer an event-driven refresh.</summary>
        event Action LobbyStateChanged;

        /// <summary>Raised whenever any participant posts a Quick Chat message (Sprint 15 "CHAT: Quick Chat").</summary>
        event Action<int, QuickChatMessage> QuickChatReceived;

        /// <summary>Starts hosting a brand-new Quick Match (the PLAY button's action; public automatic matchmaking, P017).</summary>
        void StartQuickMatch(string localDisplayName);

        /// <summary>
        /// Sprint 23.13 — skip public search / lobby fill. Hosts a local
        /// single-player stub match for the offline race prototype.
        /// </summary>
        void CreateLocalOfflinePrototype(string localDisplayName);

        /// <summary>
        /// Sprint 23.13 — set match phase to Running so Race Finish progress
        /// reporting works during the offline Gameplay prototype.
        /// </summary>
        void MarkOfflineRaceRunning();

        /// <summary>Sprint 15 (Private Room "Create Room"). Hosts a brand-new invitation-only Private Room instead of public Quick Play.</summary>
        void CreatePrivateRoom(string localDisplayName);

        /// <summary>Sprint 15 (Private Room "Join Room" by Room Code).</summary>
        void JoinPrivateRoom(string roomCode, string localDisplayName);

        /// <summary>Cancels pending matchmaking, or leaves an already-joined match — whichever applies.</summary>
        void CancelOrLeaveMatch();

        /// <summary>Sprint 15 (Ready System). Sets the local participant's Ready/Not Ready state.</summary>
        void SetLocalReady(PlayerReadyState state);

        /// <summary>Sprint 15 (Bot Settings ON/OFF toggle). Host-only; no-op for a non-host caller.</summary>
        void SetBotFillEnabled(bool enabled);

        /// <summary>Sprint 15 (Owner Feature "Kick Player"). Host-only; no-op for a non-host caller or when kicking the local player's own connection.</summary>
        void KickPlayer(int connectionId);

        /// <summary>Sprint 15 (Owner Feature "Start Match" / Play). Host-only — opens Map Voting once the lobby is fully Ready (no longer auto-starts the race countdown).</summary>
        void RequestHostStart();

        /// <summary>Sprint 15 (Player Cards "BOT" tag / Bot Settings). True if the given connection was added by Bot Fill rather than a real player.</summary>
        bool IsBot(int connectionId);

        /// <summary>Sprint 15 (Network "Latency indicator"). Always 0 under the current offline loopback transport (see <c>ConnectionManager</c> remarks).</summary>
        float GetPingMilliseconds(int connectionId);

        /// <summary>Sprint 15 (Player Cards "Connection Quality").</summary>
        ConnectionQuality GetConnectionQuality(int connectionId);

        /// <summary>Sprint 15 (Chat "Quick Chat"). Posts one of the four fixed presets to every participant.</summary>
        void SendQuickChat(QuickChatMessage message);
    }
}
