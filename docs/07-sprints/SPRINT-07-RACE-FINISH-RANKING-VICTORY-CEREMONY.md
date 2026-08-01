# Sprint 7 — Race Finish, Ranking & Victory Ceremony — Sprint Report

**Role:** Lead Gameplay Engineer
**Scope:** Complete Race Finish System — configurable-length races that run until every participant finishes or is eliminated, host-authoritative final ranking, a Podium Ceremony (top 3 only) with camera movement and victory music, a private per-player animated Reward Screen, automatic return to the same lobby, full finish/elimination/ceremony/lobby-return networking, and debug tooling. No final art/audio/animation assets and no networked `Player.prefab` instance (same running "no final gameplay logic without a real Editor" constraint as every prior sprint — see §11).
**Status:** Complete, under the same environment constraint documented in [SPRINT-01-PROJECT-FOUNDATION.md](SPRINT-01-PROJECT-FOUNDATION.md) §0 (no licensed Unity Editor on this machine — Unity Hub is installed but no Editor version is downloaded).

---

## 0. Continuation check

Sprints 1–6 (Project Foundation, Player Controller Foundation, Endless Runner Core, Multiplayer Foundation, Weapons/Item Boxes/Combat, Dynamic Trap System) are complete and were **not** rewritten. This sprint extends four existing files additively (`IMatchTransport`/`LocalLoopbackTransport`, `EconomyManager`, `AudioManager`, `GameLoopController`, plus the offline shim — see §7) to give the new RaceFinish feature the seams it needs, the same "extend the interface/vocabulary, never touch the implementation contract of unrelated features" pattern used since Sprint 4.

## 1. Architecture

A new, isolated **`GulfRun.Features.RaceFinish`** assembly (references only `GulfRun.Core` + `GulfRun.Domain`, same "Features never reference other Features" rule as every prior Features assembly) owns everything race-ending-related:

| Layer | Type | Responsibility |
|---|---|---|
| Domain (pure, no UnityEngine) | `FinishReason`, `EliminationStatus`, `RaceEndPhase` | Vocabulary enums for how/why a player's race ended, their live elimination standing, and the current post-race presentation phase. |
| Domain | `RaceProgressReport` | Client→host "how far along am I" struct (distance + coins), the sole input to finish/elimination detection. |
| Domain | `PlayerRaceResult` | One player's resolved outcome (reason, finish time, coins, distance, resolution order, final position — the last left at `-1` until ranking runs). |
| Domain | `EliminationStatusEvent` | Host→all broadcast of one player's live Safe/Warning/Eliminated standing plus a whole-seconds countdown. |
| Domain | `RaceRewardBreakdown` | One player's computed post-race reward (coins, bonus coins, rank points, experience, total). |
| Domain | `RaceRanking` | Pure final-ranking algorithm (§3). |
| Domain | `RaceElimination` | Pure elimination-gap predicates (`ShouldWarn`/`ShouldClearWarning`) with hysteresis (§4). |
| Domain | `RaceRewardCalculator` | Pure, placement-indexed reward formula (§6). |
| Domain | `RewardCounterAnimation` | Pure linear 0→target counter easing for the Reward Screen. |
| Core.Services | `IRaceProgressProvider` / `RaceProgressService` (new) | Decoupling seam so `Features.RaceFinish` can read the endless-runner's live distance/coins without referencing `Features.EndlessRunner` — same pattern as `IRunSpeedProvider`/`IDifficultyProvider`. |
| Configuration (ScriptableObject) | `RaceFinishConfig` | Every tunable value: track length, elimination gaps/countdown/safety-net timeout, progress-report rate, ceremony/reward-screen/counter timings, victory music clip, and the full reward table — the "no hardcoded values" source every class below reads from. |
| Authority | `RaceFinishAuthority` (persistent `Singleton`) | The host-only decision-maker for finish detection, elimination, final ranking, reward calculation, and the Podium→Reward→Lobby ceremony clock (§2–§6). |
| Reporting | `RaceProgressReporter` | Client-side: periodically reports local distance/coins to the host. |
| Standings | `RaceStandingsTracker` (`SceneSingleton`) | Single client-side source of truth for live results, elimination status, final results, rewards, and ceremony phase — every UI/debug view reads from here instead of independently subscribing to `IMatchTransport`. |
| Ceremony | `PodiumCeremonyView`, `RewardScreenView` | `OnGUI` presentation (§5, §6). |
| CameraSystem | `PodiumCameraDirector` | Camera movement for the ceremony (§5.1) — lives in `Features.CameraSystem`, not `Features.RaceFinish`, since it only reacts to `IMatchTransport`/`Domain` and must not create a RaceFinish→CameraSystem or CameraSystem→RaceFinish coupling either way. |
| Rewards | `RaceRewardApplier` | Applies the local player's confirmed reward to `EconomyManager.Coins` exactly once per race — kept separate from `RewardScreenView` so presentation never mutates game state. |
| Debug | `RaceFinishDebugView` | `OnGUI` panel (§8). |

This mirrors Sprints 5/6's layering exactly (Domain = rules, Core.Services = cross-feature seams, Features.RaceFinish = the feature itself, host-authoritative `IMatchTransport` = the only network path) — no new architectural pattern was invented for this sprint.

## 2. Race Length & Finish System

- **Configurable track length** (`RaceFinishConfig.TrackLengthMeters`, default 550m) targets the brief's 60–90s window for an average player against this project's existing `GameSpeedConfig` base/max run speed — tunable per-track without any code change.
- Every client's `RaceProgressReporter` reports `RaceProgressReport{ConnectionId, DistanceMeters, CoinsCollected, TimestampSeconds}` at a configurable rate (`ProgressReportIntervalSeconds`, default 0.5s) via the local `IRaceProgressProvider` (implemented by `GameLoopController`, exposing its existing distance tracker / score controller — no new distance-tracking system was built).
- The host-only `RaceFinishAuthority.HandleRaceProgressReported` detects a finish-line crossing (`DistanceMeters >= TrackLengthMeters`) and resolves that player with `FinishReason.Completed`, recording **Finish Position** (assigned only once the whole race ends — see §3), **Finish Time** (seconds since the shared race start), **Race Duration** (the same value), **Coins Collected**, and distance reached — the complete `PlayerRaceResult` — broadcast immediately via `BroadcastPlayerRaceResult` so every client sees live "who's finished so far" standings before the final ranking exists.

## 3. Race End & Final Ranking

**The race does not end at first place.** `RaceFinishAuthority` tracks every active participant in a `HashSet<int>` and only calls `FinalizeRace()` once that set is empty — i.e. once every participant has either finished or been eliminated. Two ways a player leaves the active set:

1. **Finishes** (§2).
2. **Eliminated** — either by the per-player elimination gap/countdown (§4), or by the race-wide `MaxRaceDurationSeconds` safety-net timeout (default 150s): any player still active when that elapses is force-eliminated with their last-known progress, **guaranteeing the race always ends** even if the gap threshold is never triggered (e.g. everyone stays close together).

**Final ranking** (`RaceRanking.ComputeFinalPositions`, pure Domain function, run exactly once by the host):

- Every finisher ranks above every eliminated player, regardless of timing (an elimination is not a finish-line crossing).
- Finishers are ordered by **Finish Time** ascending (the brief's "Finish Position, Finish Time" ranking criteria, resolving P010's open finish-line tie-break question).
- Eliminated players are ordered by how far they got (distance descending) before being cut, with resolution order as the final deterministic tie-break.
- The result is broadcast exactly once via `BroadcastRaceResultsFinalized(PlayerRaceResult[])` — this is the "backend validation" the brief allows for: the array is entirely server(host)-computed and clients only ever render it, never compute or submit their own ranking (P011 RES-001/002, "results cannot be modified by players").

## 4. Elimination

Pure predicates in `RaceElimination`, ticked every frame in `RaceFinishAuthority.TickRace` while `MatchState.Running`:

- **Leader distance** = the greatest currently-known distance among all participants (once someone finishes, their finish-line distance keeps counting toward this, so pressure on stragglers increases as the race winds down — an intentional design choice, not an oversight).
- **`ShouldWarn(leader, player, WarningGapMeters)`** (default 25m) → if true and the player has no active warning yet, start one: broadcast `EliminationStatusEvent{Warning, WarningSecondsRemaining}` (whole seconds, `CountdownMath.WholeSecondsRemaining` — the same countdown-rounding helper Sprint 4's shared countdown uses, so warning/elimination countdowns and match-start countdowns display identically).
- Every tick while warned, if the whole-seconds value changes, a fresh `Warning` event is broadcast — this is the **countdown**.
- **`ShouldClearWarning(leader, player, RecoveryGapMeters)`** (default 15m, deliberately lower than the warning gap for hysteresis so a player riding exactly on the threshold never flickers Safe/Warning every tick) → clears the warning and broadcasts `Safe`, if the player recovers in time.
- If the countdown reaches `EliminationCountdownSeconds` (default 5s) without recovery → `ResolvePlayer(..., FinishReason.Eliminated, ...)`, immediately assigning a final ranking slot once the whole race resolves (§3) — exactly "automatically eliminate, assign finishing position."

## 5. Podium Ceremony

Once `FinalizeRace()` runs, the host broadcasts `MatchState.Finished` then drives a two-phase ceremony clock (`RaceEndPhase.Podium` → `RaceEndPhase.Reward` → back to `MatchState.Waiting`), broadcasting `BroadcastRaceEndPhase` on every transition so every client's presentation stays in lockstep — the "Ceremony" networking requirement.

`PodiumCeremonyView` (`OnGUI`, reads only `RaceStandingsTracker`):

- **Top 3 together** — 1st centered, 2nd on the left platform, 3rd on the right platform; **4th place is never drawn** (the loop only ever looks up `FinishPosition` 1/2/3).
- **1st Place** — center position, labelled "CHAMPION (Large Trophy)", with a continuous vertical bounce (`Mathf.Sin(Time.time * 3f) * 10f`) standing in for the "Champion Animation" until real animation assets exist.
- **2nd Place** — "SILVER MEDAL", left platform. **3rd Place** — "BRONZE MEDAL", right platform.
- **Automatic playback** — `RaceFinishAuthority.TickCeremony` auto-advances after `PodiumCeremonySeconds` (default 6s).
- **Skip button** — any client's `RequestSkipRaceEndPhase()` sets a host-side flag that immediately advances the *current* phase for **everyone** (a single participant's skip is enough, matching "players stay together").
- **Victory music** — `AudioManager.PlayMusic`/`StopMusic` (new looping music `AudioSource`, separate from the existing one-shot SFX source so a track is never cut short by an unrelated SFX call), started on entering `Podium` and stopped on leaving it, driven by `RaceFinishConfig.VictoryMusicClip` (currently unassigned — see §11).

### 5.1 Camera movement

`PodiumCameraDirector` (new, `Features.CameraSystem`) listens for `RaceEndPhase.Podium` and smoothly pans (`Vector3.SmoothDamp`) the Main Camera to a configurable fixed podium framing, disabling the existing `SideScrollCameraFollow` for the duration and re-enabling it the instant the phase ends — satisfying "camera movement" without either feature referencing the other (both only depend on `Core`/`Domain`).

## 6. Player Rewards

`RaceFinishAuthority.FinalizeRace` computes every player's `RaceRewardBreakdown` via the pure `RaceRewardCalculator` (placement-indexed lookup into `RaceFinishConfig`'s bonus-coins/rank-points/experience tables, clamped to the table's last entry so a 4-entry table still degrades gracefully for larger/smaller lobbies, plus a flat participation Experience bonus) and broadcasts every player's breakdown via `BroadcastRaceReward`.

- **Privacy ("players do not see other players' reward totals")**: every client receives every player's breakdown (no unicast channel exists on `IMatchTransport` today — see §11), but `RewardScreenView` only ever looks up the entry matching `IMatchTransport.LocalConnectionId`, so the privacy requirement is enforced at the presentation layer. This is a documented, deliberate simplification, not an oversight.
- **Animated counters**: Coins Collected, Bonus Coins, Rank Points, Experience, and Total Reward each animate smoothly from 0 via `RewardCounterAnimation.EvaluateInt`, a pure linear-easing function over `RewardCounterAnimationSeconds` (default 1.5s) driven by the view's own local phase timer.
- **Skip button** — same `RequestSkipRaceEndPhase()` mechanism as the Podium phase.
- **Applying the reward**: `RaceRewardApplier` (presentation-free) subscribes to `RaceRewardCalculated`, filters to the local connection, and credits `EconomyManager.AddCoins(reward.TotalReward)` exactly once per race — `EconomyManager` gained a real (in-memory, non-persistent) `Coins` wallet + `AddCoins`/`CoinsChanged` this sprint specifically so this credit has somewhere real to go, reconciling Sprint 7 against P011's "reward amounts are placeholder-only, not yet defined" status: every number in `RaceFinishConfig`'s reward table is designer-editable data, never asserted as final game balance.

## 7. Return Flow (Lobby Return)

After the Reward phase's auto-advance (or a skip), `RaceFinishAuthority.AdvancePhase` sets `RaceEndPhase.None` and broadcasts `MatchState.Waiting` — reusing the exact same `IMatchTransport.BroadcastMatchState` mechanism Sprint 4's Match Flow already provides. No new "return to lobby" message was needed: `LobbyManager`'s participant roster is untouched by a `MatchState` transition (it only clears on an explicit leave/disconnect), so **every connected player is already back in the same lobby, together, the instant `Waiting` is broadcast** — ready for the next race without recreating the party, satisfying the Return Flow requirement with zero new networking surface.

## 8. Networking Summary

| Requirement | Mechanism |
|---|---|
| Finish Order / Finish Times | `PlayerRaceResultReported` (live, per-player, the instant each resolves) + `RaceResultsFinalized` (once, the complete 1..N ranking) |
| Eliminations | `EliminationStatusChanged` (Safe/Warning w/ countdown/Eliminated, broadcast on every state change) |
| Ceremony | `RaceEndPhaseChanged` (Podium/Reward/None, host-driven clock) + `SkipRaceEndPhaseRequested` (any client → host) + `RaceRewardCalculated` |
| Lobby Return | Reuses existing `MatchStateChanged` → `MatchState.Waiting` (§7) |

Every one of these is **host-authoritative**: clients only ever `ReportRaceProgress` or `RequestSkipRaceEndPhase`; every other race-finish fact originates from `RaceFinishAuthority` and is simply relayed by `LocalLoopbackTransport` today (swapping in a real transport per the Sprint 4 ADR requires no gameplay-facing code changes, same as every prior sprint's networking).

## 9. Debug Tools

`RaceFinishDebugView` (`OnGUI`, Editor/dev-build only, `panelX: 1810` — clear of `MultiplayerDebugView`(460)/`WeaponsDebugView`(910)/`TrapsDebugView`(1360), so all four plus `RunnerDebugView`/`PlayerDebugView` can be shown simultaneously):

- **Current Rank** — the local player's live `PlayerRaceResult.FinishPosition` (or "resolved, not yet ranked" / "racing" before that).
- **Finish Time** — the local player's recorded finish/elimination time, reason, and distance reached.
- **Elimination Status** — Safe/Warning (with live countdown seconds)/Eliminated.
- **Reward Calculation** — the local player's full `RaceRewardBreakdown` once computed.
- The complete final results table, once known.
- A host-only **"Simulate Remote Race Progress"** button that feeds synthetic incremental distance/coins for every non-local participant into `RaceFinishAuthority` via `LocalLoopbackTransport.SimulateRemoteRaceProgress` — this exercises the full finish/elimination/ranking/ceremony flow end-to-end under the offline loopback transport without any real remote client, tracking each simulated bot's cumulative progress locally in the debug view (bots stop advancing once resolved, and progress resets when a new race begins).

## 10. Code Quality

- **SOLID:** `RaceFinishAuthority` (host decision-making) is separate from `RaceProgressReporter` (client reporting), `RaceStandingsTracker` (client-side state cache), `PodiumCeremonyView`/`RewardScreenView` (presentation), and `RaceRewardApplier` (economy mutation) — five responsibilities, five classes. Dependency Inversion: `IRaceProgressProvider` means RaceFinish never references EndlessRunner directly, and `PodiumCameraDirector` never references RaceFinish directly (only `Core`/`Domain`). Open/Closed: every elimination/reward/ceremony-timing number is a `RaceFinishConfig` field — tuning the game never requires a code change.
- **No hardcoded values:** track length, elimination warning/recovery gaps, elimination countdown, max race duration, progress-report rate, podium/reward-screen durations, counter-animation duration, victory music clip, coin multiplier, and all three placement-indexed reward tables (bonus coins / rank points / experience) plus participation experience are all serialized `RaceFinishConfig` fields.
- **No duplicated logic:** the whole-seconds countdown display reuses Sprint 4's existing `CountdownMath.WholeSecondsRemaining` (no second countdown-rounding implementation); reward crediting reuses the existing `EconomyManager` singleton (extended, not duplicated).
- **Future tournament support:** `RaceRanking`/`RaceRewardCalculator`/`PlayerRaceResult` are pure, stateless Domain functions/structs with no assumption about "one race per session" baked in — a future tournament mode can call `RaceRanking.ComputeFinalPositions` across a differently-sourced results set (e.g. aggregated across heats) without modifying this sprint's code, and `RaceEndPhase`'s doc comment explicitly distinguishes it from the coarser `MatchState` lifecycle so a tournament-specific phase (e.g. "Standings") could be inserted without disturbing the existing Waiting/Countdown/Running/Finished contract.

## 11. Build Verification / Compiler Status

- **Offline compile:** all **152** project `.cs` files (up from 130 after Sprint 6) recompiled together via `dotnet build .compile_check/CompileCheck.csproj` against `.compile_check/Shims/UnityEngineShim.cs`, extended this sprint with `AudioSource.loop`/`AudioSource.clip`, `Color.black`/`Color.yellow`/`Color.gray`, `GUI.Box`, and `Mathf.Sin` — real gaps in the shim (the ceremony/reward UI and champion-bounce animation needed them), not workarounds for anything wrong with the actual game code. **Result: Build succeeded, 0 errors, 0 warnings.**
- **YAML/GUID validation:** `.compile_check/validate_yaml.py` (structural YAML parse) — all 3 new/changed files (`Boot.unity`, `Gameplay.unity`, `RaceFinishConfig.asset`) **OK**. `.compile_check/validate_yaml_refs.py` — **195** unique project `.meta` GUIDs (up from 171), **no duplicates**; the only two flagged references are the same pre-existing Unity built-in `RenderSettings` skybox/spot-cookie references documented as expected false positives since Sprint 4. `.compile_check/check_fileid_refs.py . Gameplay.unity Boot.unity RaceFinishConfig.asset` — **"ALL 3 FILES: fileID/guid references OK (195 known guids in project)."**

## 12. Scene & Asset Wiring

- **`Boot.unity`** — new `RaceFinishSystems` GameObject (root order 4) with `RaceFinishAuthority`, pointed at `RaceFinishConfig.asset`; placed alongside (not replacing) Sprints 4–6's `MultiplayerSystems`/`WeaponSystems`/`TrapSystems`.
- **`Gameplay.unity`** — new `RaceFinishSystems` GameObject with `RaceProgressReporter` + `RaceStandingsTracker` + `PodiumCeremonyView` + `RewardScreenView` + `RaceRewardApplier` + `RaceFinishDebugView`, all pointed at the same config asset; Main Camera gained a new `PodiumCameraDirector` component wired to its existing `SideScrollCameraFollow`.
- **New asset:** `Settings/RaceFinishConfig.asset` (single config for the whole feature, same "one ScriptableObject per feature" convention as `NetworkSyncConfig`/`WeaponCatalogConfig`/`TrapCatalogConfig`).
- As with every prior sprint, a networked `Player.prefab` instance is still **not** placed in `Gameplay.unity` — `RaceProgressReporter` always reports the local connection's progress (same limitation every prior client-input feature inherits); until a real remote avatar exists, only the local player's real progress drives finish detection (remote participants must be exercised via the debug view's simulate button, §9).

## 13. Remaining TODOs

1. **No real reward-balance authority yet** — `RaceFinishConfig`'s coin multiplier / bonus-coins / rank-points / experience tables are placeholder values pending a ratified Reward System spec (P011 marks these "not yet defined"); every number is designer-editable data today, not asserted as final balance.
2. **No unicast network channel** — `BroadcastRaceReward` sends every player's breakdown to every client; Reward Screen privacy is enforced only at the presentation layer (§6). A future real transport with per-connection send would let the host stop sending other players' breakdowns at all.
3. **`EconomyManager.Coins` is in-memory only** — it is not yet persisted to a save file or backend wallet (carried forward from Sprint 1's economy/inventory TODO); `RaceRewardApplier` credits it correctly today, but the credit does not survive an app restart until a real save/backend layer exists.
4. **No networked `Player.prefab` instance exists in any scene** (carried forward from Sprints 2–6) — only the local player's real distance/coins ever drive finish/elimination detection; remote participants must be exercised via `RaceFinishDebugView`'s simulate button (§9) until a real remote avatar exists.
5. **`RaceFinishAuthority` does not yet distinguish "why" a safety-net timeout fired from a per-player elimination** in its broadcast `PlayerRaceResult` (both use `FinishReason.Eliminated`) — acceptable per the brief (both are eliminations with a final ranking), but a future analytics/UX pass may want a third `FinishReason` value to tell them apart.
6. **No final art/audio/animation assets** — `RaceFinishConfig.VictoryMusicClip` is unassigned, the Champion Animation is a placeholder sine-wave bounce, and the Podium/Reward Screens are `OnGUI` placeholders per the project's established "functional now, UI Toolkit later" policy (see `docs/02-architecture/TECHNICAL_STACK.md`) — same status as every prior sprint's UI.
7. Carries forward all unresolved Sprint 1–6 items (Unity 6 LTS install still only Hub; ADR-0001 still Proposed, not Accepted; no Lobby/Waiting Room UI scene; ping always 0 under the loopback transport; bundle IDs; UI framework ADR; no real "use weapon" input binding; no lane-change axis).

## 14. Git Workflow

| Item | Value |
|---|---|
| Commit hash | `abba9e63dce938fc87256c1a2a0875e1a2c9ce1b` |
| Commit message | `Sprint 7 - Race Finish, Ranking & Victory Ceremony` |
| Branch | `main` |
| Files changed | 58 files changed, 2284 insertions(+), 4 deletions(-) |
| Push status | ✅ Pushed to `origin/main` (`https://github.com/q8wink1/gulf-run.git`), fast-forward `af5d86e..abba9e6`. Verified via `git status` ("Your branch is up to date with 'origin/main'", working tree clean) and `git log`. |

Sprint 7 is complete within the constraints above. Stopping here. Waiting for Sprint 8.
