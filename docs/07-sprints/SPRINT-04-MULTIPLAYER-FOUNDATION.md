# Sprint 4 — Multiplayer Foundation — Sprint Report

**Role:** Lead Multiplayer Engineer
**Scope:** Transport-agnostic multiplayer architecture — Match Flow (Create/Join/Leave/Cancel), Lobby + Ready System, shared Countdown, player synchronization + network interpolation, Connection/Lobby/Match/Session/Spawn managers, and debug tooling. No final gameplay logic (no real remote transport, no player-avatar spawning/movement replication wiring) per the brief.
**Status:** Complete, under the same environment constraint documented in [SPRINT-01-PROJECT-FOUNDATION.md](SPRINT-01-PROJECT-FOUNDATION.md) §0 (no licensed Unity Editor on this machine — Unity Hub is installed but no Editor version is downloaded).

---

## 0. Continuation check

Sprint 1 (Project Foundation), Sprint 2 (Player Controller Foundation), and Sprint 3 (Endless Runner Core, including the Race Start/Double Jump addendum) are complete and were **not** modified except for one pre-existing bug fix (§1) required for the new code to compile correctly inside a real Unity Editor. No Sprint 1–3 system was rewritten, duplicated, or overwritten. `docs/adr/README.md` previously had **zero** ratified ADRs despite `MULTIPLAYER_ARCHITECTURE.md` §9 requiring "choose via ADR" for the Netcode approach — this sprint drafts that ADR (§13) rather than silently picking a transport.

## 1. Pre-existing bug fixed

`GulfRun.Core.asmdef` declared `"references": []` despite `Core/Services/IGameStateProvider.cs` (Sprint 3) already using `GulfRun.Domain.GameLoopState`. The offline `.compile_check` project ignores asmdef boundaries (it globs every script into one project), so this was invisible until now — but it would have failed to compile inside a real Unity Editor, where asmdef references are enforced. Sprint 4 adds substantially more Core code that depends on Domain types (`IMatchTransport`, `ILocalPlayerStateProvider`), making this the right moment to fix it: `GulfRun.Core.asmdef` now references `GulfRun.Domain`. This is a one-line, additive fix — no behavior changed, only a missing reference added — consistent with "fix bugs, don't rewrite completed work."

## 2. Multiplayer Architecture

Per [MULTIPLAYER_ARCHITECTURE.md §9](../02-architecture/MULTIPLAYER_ARCHITECTURE.md#9-netcode-approach-options-choose-via-adr), the Netcode approach (custom protocol / Unity Netcode for GameObjects / third-party) is an **open decision reserved for an ADR** — one didn't exist yet (`docs/adr/README.md` index was empty). Rather than silently hard-coding Unity Netcode for GameObjects (already present in `manifest.json` from Sprint 1's "preparation" note, but never ratified) into every gameplay manager, this sprint:

1. Introduces **`Core.Networking.IMatchTransport`** — the single seam every multiplayer manager depends on: participant join/leave, ready-state, match-state, countdown, and player-snapshot events, plus Create/Join/Disconnect/ready/broadcast/send methods. No manager anywhere references a concrete transport type.
2. Ships **`LocalLoopbackTransport`** as the default, self-registering implementation (`Core.Networking.MatchTransportService.Current` lazily creates one on first access) — a pure C# class (zero UnityEngine dependency) that runs entirely in-process. It has no real sockets; "remote" participants only exist via explicit `SimulateRemoteJoin`/`SimulateRemoteReady`/etc. calls (wired to the debug view's buttons, §10), so the full Match Flow can be exercised end-to-end today.
3. Records the decision and its rationale in **[ADR-0001](../adr/0001-multiplayer-transport-abstraction.md)** (Proposed — pending Principal Architect + Server Lead sign-off), so a real transport (dedicated-server Unity Netcode for GameObjects, a custom authoritative protocol, or a vetted third party) can be dropped in later by reassigning `MatchTransportService.Current` — zero changes to any gameplay-facing manager.

This satisfies "4 Players per Match" (`NetworkSyncConfig.MaxPlayers`, default 4, and `Domain.MatchSpawnLayout.MaxSlots`), "Client/Server architecture" and "Authoritative networking" (every state-changing broadcast — `BroadcastMatchState`, `BroadcastCountdownSeconds` — is gated to `IsHost`; non-host callers are no-ops), and "future dedicated server compatibility" concretely, not just in prose: `LocalLoopbackTransport` and every Domain type it uses (`MatchState`, `PlayerIdentity`, `MatchParticipant`, `NetworkPlayerSnapshot`, `NetVector2`) have **zero UnityEngine dependency** and could run unmodified inside a future plain-.NET dedicated server process.

## 3. Match Flow

| Action | Implementation |
|---|---|
| Create Match | `Session.SessionManager.CreateMatch(displayName)` — generates a local `PlayerIdentity`, calls `IMatchTransport.StartHost`, becomes host. |
| Join Match | `SessionManager.JoinMatch(joinCode, displayName)` — calls `IMatchTransport.JoinAsClient`; end-to-end wired, but `LocalLoopbackTransport.JoinAsClient` is an explicit no-op stub (there is no real remote host reachable in-process — see ADR-0001) until a real transport exists. |
| Leave Match | `SessionManager.LeaveMatch()` — disconnects (`DisconnectReason.HostLeft` or `.PlayerLeft` depending on host status), clears the Lobby roster, resets the Match state machine. |
| Cancel Matchmaking | `SessionManager.CancelMatchmaking()` — clears the pending `IsMatchmaking` flag before a connection completes. |
| Lobby / Waiting Room | `Lobby.LobbyManager` — roster of `Domain.MatchParticipant`, kept in sync purely by listening to `IMatchTransport` events. |
| Countdown | `Match.MatchManager` — shared 3-2-1-GO, host-authoritative (§5). |
| Race Start | `MatchManager` broadcasts `MatchState.Running` the instant the countdown reaches 0 — every listening client (and this project's own debug view) reacts identically. |
| Race End | `MatchState.Finished` is a defined, reachable state (`MatchManager`/`IMatchTransport.BroadcastMatchState`); no results/scoring UI is wired to it yet — out of scope ("no final gameplay logic"), tracked in §12. |

## 4. Lobby

`Features.Multiplayer.Lobby.LobbyManager` (persistent `Singleton<T>`) maintains `Dictionary<int, Domain.MatchParticipant>` keyed by connection ID, updated only by `IMatchTransport` events (`ParticipantJoined`/`ParticipantLeft`/`ReadyStateChanged`). Exposes:

- **Player List** — `Participants` (`IReadOnlyCollection<MatchParticipant>`).
- **Ready Status** — `MatchParticipant.Ready` (`Domain.PlayerReadyState`: NotReady/Ready).
- **Connection Status** — `MatchParticipant.Connection` (`Domain.ConnectionState`), kept fresh by `ConnectionManager` (§6) independently, then folded back onto the roster.
- **Host Status** — `MatchParticipant.IsHost`.
- **Player Count** — `PlayerCount`.

`LobbyChanged` fires only *after* the roster dictionary is updated, which is what lets `MatchManager` safely react to "is everyone ready yet" without an Awake/OnEnable ordering hazard (see §5).

## 5. Ready System & Countdown

Every participant has a `PlayerReadyState` (`NotReady`/`Ready`) set via `SessionManager.SetLocalReady(state)` → `IMatchTransport.SetLocalReadyState`. `LobbyManager.AllRequiredPlayersReady()` is a pure roster check (`Domain.ReadyCheck.AllReady`, `NetworkSyncConfig.MinimumPlayersToStart`, default 2). `MatchManager` subscribes to `LobbyManager.LobbyChanged` and calls `TryStartCountdown()` on every roster change — **no button required**, matching the single-player Sprint 3 addendum's auto-countdown design at match scope instead of single-player scope.

**Shared countdown, identical for every player:** only the host's `MatchManager.Update()` advances `_countdownElapsedSeconds` and calls `IMatchTransport.BroadcastCountdownSeconds`/`BroadcastMatchState`; every other participant (including the host's own UI) reacts purely to `CountdownSecondsChanged`/`MatchStateChanged`, which is what *guarantees* every connected player receives the identical 3-2-1-GO and flips to `Running` in the same instant — there is no per-client local countdown timer to drift out of sync. The whole-seconds arithmetic is `Domain.CountdownMath.WholeSecondsRemaining` — a new pure function deliberately kept separate from Sprint 3's `Features.EndlessRunner.GameLoop.CountdownController` (same shape of problem, but the single-player and match-level countdowns must not depend on each other across features).

## 6. Player Synchronization & Network Interpolation

| Requirement | Implementation |
|---|---|
| Player Position / Rotation | `Domain.NetworkPlayerSnapshot.Position` (`Domain.NetVector2`, engine-independent) / `RotationDegrees`. |
| Animation / Jump / Landing / Running state | `NetworkPlayerSnapshot.AnimationState` reuses **`Domain.PlayerMovementState` directly** (Sprint 2/3's enum already distinguishes Running/Jumping/DoubleJumping/Falling/Landing) — zero duplicated state machine. |
| Connection Status | Tracked separately by `ConnectionManager` (§7), not embedded per-snapshot (it changes far less often than position). |

`Features.Multiplayer.Sync.NetworkPlayerSync` reads the local player's live state via the new `Core.Services.ILocalPlayerStateProvider`/`LocalPlayerStateService` locator (same decoupling pattern as `IRunSpeedProvider`/`IGameStateProvider` — Multiplayer never references the PlayerController feature assembly). `Features.PlayerController.PlayerNetworkStateAdapter` (new, optional component, `[RequireComponent(typeof(PlayerMotor))]`) publishes it. Snapshots are sent at a configurable, sub-60Hz rate (`NetworkSyncConfig.SnapshotSendRateHz`, default 15) — directly satisfying "Minimize bandwidth."

`Features.Multiplayer.Sync.RemotePlayerSyncHub` buffers the last two snapshots per remote connection and resolves a render pose via the pure **`Domain.NetworkInterpolator.Resolve`**: linear interpolation between two known snapshots (smooth blending, "avoid visible jitter"), and a short, clamped linear **extrapolation** if render time runs ahead of the last snapshot (`NetworkSyncConfig.MaxExtrapolationSeconds`, default 0.25s) before holding at the last known pose — this is the "prepare future prediction system" requirement implemented as working, testable math today, not just a comment.

Neither `NetworkPlayerSync` nor `RemotePlayerSyncHub` spawns or moves a visual player avatar — no networked Player.prefab instance exists in any scene yet (Sprint 2/3 also deliberately stopped short of this; see §12 item 1). They are ready-to-consume building blocks for a future `PlayerSpawnController`/remote-avatar component.

## 7. Connection Management & Disconnection Handling

`Features.Multiplayer.Connection.ConnectionManager` (persistent `Singleton<T>`) tracks `Domain.ConnectionState` (Connecting/Connected/TimedOut/Reconnecting/Disconnected) per connection ID, independent of Lobby/Ready concerns:

- **Temporary Disconnect / Timeout** — any inbound transport event (join or snapshot) is a heartbeat; if none arrive within `NetworkSyncConfig.ConnectionTimeoutSeconds` (default 10s), the participant is flagged `TimedOut` (`PlayerTimedOut` event) **without** being removed from the Lobby roster — a stalled link isn't the same as leaving.
- **Reconnect** — the next snapshot received from a `TimedOut` connection flips it back to `Connected` (`PlayerReconnected` event).
- **Player Leaving / Host Leaving** — `IMatchTransport.ParticipantLeft` carries a `Domain.DisconnectReason` (`PlayerLeft`/`HostLeft`/`Timeout`/`Unknown`); `SessionManager.LeaveMatch()` reports the correct reason based on the local player's own host status.
- **Ping** — `ConnectionManager.PingSecondsFor(connectionId)` is part of the public API and rendered in the debug view now; it is honestly `0` under the in-process `LocalLoopbackTransport` (no real round trip exists to measure) and is the designated place a real transport reports measured RTT.

## 8. Network Managers

| Manager | Location | Lifetime | Responsibility |
|---|---|---|---|
| **Connection Manager** | `Features.Multiplayer.Connection.ConnectionManager` | `Singleton` (persistent) | Link health, timeout/reconnect detection, ping. |
| **Lobby Manager** | `Features.Multiplayer.Lobby.LobbyManager` | `Singleton` (persistent) | Roster, Ready System, player count. |
| **Match Manager** | `Features.Multiplayer.Match.MatchManager` | `Singleton` (persistent) | `MatchState` machine, host-authoritative shared countdown. |
| **Session Manager** | `Features.Multiplayer.Session.SessionManager` | `Singleton` (persistent) | Composition root: Create/Join/Leave/Cancel Matchmaking. |
| **Spawn Manager** | `Features.Multiplayer.Spawning.SpawnManager` | `SceneSingleton` (Gameplay scene) | Assigns unique, non-overlapping race-start positions per connection. |

All five are Persistent/Singleton or SceneSingleton per the existing convention (`Core.Singleton<T>`/`Core.SceneSingleton<T>` — the same base classes as `SaveManager`/`ObjectPoolManager` and `GameLoopController`/`WorldGenerator`, respectively); Connection/Lobby/Match/Session are boot-persistent because a real session must survive a future Lobby-scene → Gameplay-scene transition, while Spawn Manager is scoped to the race scene it positions players in.

## 9. Spawning

`Domain.MatchSpawnLayout` is a pure, deterministic 4-slot formation: fixed X-axis (run-direction) offsets from a shared start line (`{-1.125, -0.375, 0.375, 1.125}` for slots 0–3), never in Y/altitude, since Sprint 2/3's 2D physics keeps every player on one ground line with gravity along -Y — stacking spawn slots vertically would place players at different heights on the same ground, which is physically wrong for this game. By construction, no two slots ever coincide, satisfying "prevent overlapping" without any runtime overlap query. `Features.Multiplayer.Spawning.SpawnManager` assigns slots first-come-first-served and exposes `TryGetSpawnPosition(connectionId, out Vector2)`. It does not instantiate any Player.prefab — see §12 item 1.

## 10. Player Identification

`Domain.PlayerIdentity` (readonly struct): `PlayerId` (locally-generated GUID for now), `DisplayName`, `ConnectionId` (assigned by the transport on join), and `ProfileId` (reserved, empty string — no account/profile system exists yet, P041 territory). `Features.Multiplayer.Identification.LocalPlayerIdentity.CreateLocal(displayName)` generates the local client's identity — same "honest placeholder now, real system later" approach as `SaveManager`/`IProgressRepository`.

## 11. Game States

`Domain.MatchState`: `Waiting`, `Countdown`, `Running`, `Finished`, `Disconnected` — deliberately a **separate enum** from the single-player `Domain.GameLoopState` (which already has its own `Ready/Countdown/Running/Paused/GameOver/Restart`), since a match's lifecycle and a single player's session lifecycle are different concerns that must not be conflated once multiplayer and single-player can coexist in the same build.

## 12. Debug Tools

`Features.Multiplayer.MultiplayerDebugView` (Editor/dev-build only `OnGUI`, same convention as `RunnerDebugView`/`PlayerDebugView`) displays exactly the requested fields — **Connected Players** (full roster with name/host/ready/connection), **Ping**, **Connection State**, **Player IDs** (connection ID + display name per row), **Match State** (plus live countdown value) — and adds four buttons (Create Match, Ready Up, Simulate Remote Join [auto-ready, for exercising the Ready System], Leave Match) so the entire Match Flow can be driven and observed end-to-end today with no menu UI in the project yet.

## 13. Netcode ADR

[ADR-0001](../adr/0001-multiplayer-transport-abstraction.md) is drafted and added to the `docs/adr/README.md` index (previously empty) with **Status: Proposed** — it documents *why* a transport-agnostic seam was introduced this sprint instead of committing to Unity Netcode for GameObjects (already in `manifest.json` but never ratified) or any other option, and hands the actual vendor decision to Principal Architect + Server Lead, per the ADR process rules in `docs/adr/README.md`.

## 14. Files Added / Changed

**Domain (`Scripts/Domain/`, all pure, no UnityEngine dependency) — new:** `MatchState.cs`, `PlayerReadyState.cs`, `ConnectionState.cs`, `DisconnectReason.cs`, `PlayerIdentity.cs`, `MatchParticipant.cs`, `NetVector2.cs`, `NetworkPlayerSnapshot.cs`, `NetworkInterpolator.cs`, `ReadyCheck.cs`, `MatchSpawnLayout.cs`, `CountdownMath.cs`.

**Core — new:** `Networking/IMatchTransport.cs`, `Networking/MatchTransportService.cs`, `Networking/LocalLoopbackTransport.cs`, `Services/ILocalPlayerStateProvider.cs`, `Services/LocalPlayerStateService.cs`.

**Core — extended (bug fix + bootstrap, not rewritten):** `GulfRun.Core.asmdef` (+`GulfRun.Domain` reference, §1), `Managers/NetworkManager.cs` (`OnInitialize` now ensures a default `IMatchTransport` is registered as early as possible; the default is otherwise self-initializing regardless — see `MatchTransportService`).

**Features/PlayerController — new (optional, additive):** `PlayerNetworkStateAdapter.cs`.

**Features/Multiplayer — entirely new feature assembly (`GulfRun.Features.Multiplayer.asmdef`, references `GulfRun.Core` + `GulfRun.Domain` only, per the "Features must not reference other Features" rule):** `Configuration/NetworkSyncConfig.cs`, `Identification/LocalPlayerIdentity.cs`, `Connection/ConnectionManager.cs`, `Lobby/LobbyManager.cs`, `Match/MatchManager.cs`, `Session/SessionManager.cs`, `Spawning/SpawnManager.cs`, `Sync/NetworkPlayerSync.cs`, `Sync/RemotePlayerSyncHub.cs`, `MultiplayerDebugView.cs`.

**Assets:** `Settings/NetworkSyncConfig.asset` (new ScriptableObject instance).

**Scenes:** `Scenes/Boot.unity` (+`MultiplayerSystems` GameObject: ConnectionManager, LobbyManager, MatchManager, SessionManager, MultiplayerDebugView — the *first* manager GameObject ever wired into Boot.unity; see §16 item 2 for the pre-existing gap this does not fix), `Scenes/Gameplay.unity` (+`MultiplayerSpawning` GameObject: SpawnManager, RemotePlayerSyncHub).

**Docs:** `docs/adr/0001-multiplayer-transport-abstraction.md` (new), `docs/adr/README.md` (index updated), `docs/07-sprints/SPRINT-04-MULTIPLAYER-FOUNDATION.md` (this report), `docs/README.md`, `Client/README.md`.

**Tooling:** `.compile_check/Shims/UnityEngineShim.cs` (+`GUI.Button`, +`Time.timeAsDouble`), `.compile_check/validate_yaml_refs.py` (new fileID/guid cross-reference + duplicate-GUID validator, ad hoc replacement for the Sprint 3 script of the same purpose).

## 15. Code Quality

- **SOLID:** `IMatchTransport` is the Dependency Inversion seam for the entire feature (every manager depends on the abstraction, never `LocalLoopbackTransport` directly, except the debug view's explicit local-testing hooks which are clearly commented as such). Each manager has one responsibility (Connection ≠ Lobby ≠ Match ≠ Session ≠ Spawn); `SessionManager` is the only composition root. Open/Closed: a real transport is added by implementing `IMatchTransport`, not modifying any existing manager.
- **No hardcoded values:** every tunable (max players, minimum-to-start, countdown duration, snapshot rate, interpolation delay, max extrapolation, connection timeout) lives in `NetworkSyncConfig` (ScriptableObject).
- **No duplicated logic:** `NetworkPlayerSnapshot.AnimationState` reuses Sprint 2/3's `PlayerMovementState` instead of a parallel enum; `ReadyCheck`/`MatchSpawnLayout`/`NetworkInterpolator`/`CountdownMath` are single pure implementations shared by whichever manager needs them.
- **No gameplay logic in UI:** `MultiplayerDebugView` only calls public manager methods and reads public state; all Ready/Countdown/Spawn/Sync decision logic lives in the managers.
- **Feature isolation:** `GulfRun.Features.Multiplayer.asmdef` references only `GulfRun.Core`/`GulfRun.Domain` — never `GulfRun.Features.EndlessRunner` or `GulfRun.Features.PlayerController` — cross-feature data flow goes through `Core.Services.ILocalPlayerStateProvider`, the same pattern already used for `IRunSpeedProvider`/`IGameStateProvider`.

## 16. Performance Notes

- **Mobile-optimized sync rate:** `NetworkSyncConfig.SnapshotSendRateHz` defaults to 15Hz, not 60Hz — directly "Minimize bandwidth" while gameplay itself still targets 60+ FPS (unaffected, since sending is decoupled from `Update()`'s per-frame cadence via an accumulator timer).
- **Zero-allocation hot paths:** `NetworkPlayerSnapshot`, `MatchParticipant`, `PlayerIdentity`, `NetVector2` are all `readonly struct`s (no per-snapshot heap allocation); `ConnectionManager.Update()` reuses a single scratch `List<int>` instead of allocating one every frame.
- **No `Instantiate`/`Destroy` in the hot path:** this sprint adds no spawning of GameObjects at all (positions only, §9) — fully consistent with Sprint 3's Object Pooling discipline; when a future sprint spawns networked player avatars, `Core.Pooling.ObjectPoolManager` is the designated mechanism, not raw `Instantiate`.

## 17. Build Verification / Compiler Status

- **Offline compile:** all project `.cs` files (**93**, up from 65 after Sprint 3) recompiled together via `dotnet build` against `.compile_check/Shims/UnityEngineShim.cs`, extended with `GUI.Button` and `Time.timeAsDouble`. **Result: Build succeeded, 0 errors, 0 warnings.**
- **YAML/GUID validation:** `.compile_check/validate_yaml_refs.py` (new) checked all 103 project `.meta` GUIDs for duplicates (**none found**) and cross-referenced every `{fileID, guid}` in `Boot.unity` (15 documents, up from 8), `Gameplay.unity` (33 documents, up from 29), and the new `NetworkSyncConfig.asset` (1 document) — **0 real dangling references**. The script flagged two pre-existing references to Unity's built-in Resources (`m_SkyboxMaterial`/`m_SpotCookie` in `RenderSettings`, all-zero GUIDs with `e`/`f` suffixes) as "missing" — these predate this sprint (untouched `RenderSettings` block from the original template scenes), are not real project assets, and are expected false positives for any script that only knows about project `.meta` files.

## 18. Remaining TODOs

1. **No networked Player.prefab instance exists in any scene** — `NetworkPlayerSync`/`PlayerNetworkStateAdapter`/`RemotePlayerSyncHub`/`SpawnManager` are all implemented and ready to consume/produce data, but nothing yet instantiates a local or remote player avatar in a multiplayer session (same "first real Editor open required" caveat as Sprint 2/3's Player.prefab). This is intentional — "Do NOT implement final gameplay logic."
2. **Pre-existing gap, carried forward, not fixed this sprint:** none of Sprint 1's ten `Core.Managers.*` singletons (`SaveManager`, `GameManager`, `NetworkManager`, etc.) were ever placed in any scene before this sprint — `Boot.unity` contained only a Main Camera. This sprint adds the *first* manager GameObject to `Boot.unity` (`MultiplayerSystems`), but does not retroactively wire the other ten (out of scope for a multiplayer-focused sprint); `MatchTransportService.Current`'s self-initializing getter means the new Multiplayer managers work correctly regardless, but e.g. `SaveManager.Instance` is still `null` at runtime today (Sprint 3's `GameLoopController.CommitBestResults()` already null-checks defensively, so this is a silent no-op, not a crash).
3. **Netcode ADR-0001 is Proposed, not Accepted** — needs Principal Architect + Server Lead review per `docs/adr/README.md` process before a real transport implementation is started (§13).
4. **`LocalLoopbackTransport.JoinAsClient` is an intentional no-op** — there is no real remote host reachable in a single process; multi-machine Join Match testing requires a real transport (ADR-0001 follow-up).
5. **No Lobby/Waiting Room UI scene** — Match Flow is fully implemented and driven via `MultiplayerDebugView`'s buttons (Editor/dev-build only); a real menu UI (and a dedicated Lobby scene, if desired) is future work, same "Canvas + TextMeshPro HUD once the Editor is available" caveat as the Sprint 3 countdown UI.
6. **`ConnectionManager.PingSecondsFor` always returns 0** under the offline loopback transport — will reflect real measured RTT once a real transport is wired in.
7. **Race End (`MatchState.Finished`) has no results/placement UI or scoring hook yet** — the state is reachable and broadcastable, but nothing consumes it; a future sprint should decide how per-player race results map onto Sprint 3's single-player `ScoreController`/`SaveManager` equivalents for a multiplayer context.
8. Carries forward all unresolved Sprint 1/2/3 items (Unity 6 LTS install — still not present, only Hub; bundle IDs; UI framework ADR; Services scope; P020/P042 profile conflict; real placeholder animations/art; Input System package verification; moving-platform prefab/art).

## 19. Git Workflow

| Item | Value |
|---|---|
| Commit hash | `6945798` (`69457980062a92d5f45030581c38a8b00f24245d`) |
| Commit message | `Sprint 4 - Multiplayer Foundation` |
| Branch | `main` |
| Push status | Pushed to `origin/main`; verified via `git fetch` + `git log origin/main -1` matching the local hash |

Sprint 4 is complete within the constraints above. Stopping here. Waiting for Sprint 5.
