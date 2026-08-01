# Sprint 15 — Race HUD, Gameplay UI & In-Race Experience — Sprint Report

**Role:** Lead Gameplay / UI Engineer
**Scope:** Production-ready in-race HUD — countdown presentation, player chrome (position / lap / speed / shield / weapon / coins / gems / timer), trap proximity warning, active-effect duration bars, quick emotes, race-progress minimap, finish banner with fireworks/confetti, camera look-ahead + bob + impact-only shake, dust/speed-trail VFX hooks, gameplay audio director, accessibility-minded Gulf palette, responsive scaling, and a Race HUD debug panel — see §1–§10.
**Status:** Complete, under the same environment constraint documented in [SPRINT-01-PROJECT-FOUNDATION.md](SPRINT-01-PROJECT-FOUNDATION.md) §0 (no licensed Unity Editor on this machine — Unity Hub is installed but no Editor version is downloaded).

---

## 0. Continuation check

Sprints 1–14 are complete and were **not** rewritten. Concurrent uncommitted Matchmaking / Character-Locker work in the tree was left intact and **not** included in this commit. This sprint is additive: new `GulfRun.Features.RaceHud` assembly, Domain helpers, Core.Services seams, and small extensions on Countdown / Standings / Weapons / Status Effects / Camera / Transport. Legacy `CountdownView` remains on `Gameplay.unity` but is disabled; RaceHud owns countdown presentation.

> **Naming note:** Design docs sometimes labeled Matchmaking/Pre-Race Lobby or Brand Intro as “Sprint 15.” In this engineering index, Sprint 14 is Brand Intro and **this** report is Sprint 15 — Race HUD. Matchmaking/Locker continue as separate workstreams.

## 1. Architecture

| Layer | Type | Responsibility |
|---|---|---|
| Domain | `RacePositionFormatter`, `RaceLiveRanking`, `RaceProgressMarker` | Live place + minimap markers (pure). |
| Domain | `HudEffectKind` (+ resolver), `ActiveHudEffectSnapshot` | Readable effect chips from `WeaponEffectFlags`. |
| Domain | `WeaponHudSlotSnapshot`, `RaceEmoteId`, `CountdownHudAnimation`, `HudLayoutScale`, `FireworkSimulation` | HUD snapshots, emote glyphs, countdown/finish curves. |
| Core.Services | `ICountdownHudProvider`, `IRaceStandingsHudProvider`, `IWeaponHudProvider`, `IActiveEffectsHudProvider`, `ITrapProximityHudProvider`, `INetworkDiagnosticsProvider`, `IRaceTimerProvider` | Cross-feature seams — RaceHud never references EndlessRunner / RaceFinish / Weapons / Traps / Multiplayer / PlayerController assemblies. |
| Core.Networking | `IMatchTransport.SendRaceEmote` / `RaceEmoteReceived` | Fire-and-forget in-race emotes (same shape as Quick Chat). |
| `GulfRun.Features.RaceHud` | `RaceHudTheme`, `RaceHudConfig`, `RaceHudController`, views, audio, VFX, debug | Scene-scoped OnGUI HUD composition root. |
| Features.Traps | `TrapProximityWatcher` | Nearby-trap indicator only (no auto-avoid). |
| Features.Multiplayer | `NetworkDiagnosticsBridge` | Local ping for debug. |
| Features.CameraSystem | `SideScrollCameraFollow` + `CameraFollowConfig` | Look-ahead, vertical bob, impact-only shake. |

## 2. Countdown

`CountdownController` publishes `ICountdownHudProvider`. `CountdownHudView` punches scale on each whole second, plays optional tick/GO clips, then slides/fades **GO!** away over `RaceHudConfig.GoHoldSeconds` after the race starts. Legacy `CountdownView` stays for unwired scenes (disabled on Gameplay).

## 3. Player HUD

`RaceHudView` draws (all config/theme-driven, no magic layout numbers in views):

- Large ordinal place with punch on change; `LAP 1/{MaxLaps}` future-ready
- Speed (m/s), shield chip, weapon slot (name / uses / cooldown bar / pickup glow)
- Race coin counter + optional gem counter (`ILocalProfileProvider`)
- Race timer (`IRaceTimerProvider`)
- Active effect rows with duration bars + color-blind shape tags (▲◆◌▼■)
- Small pulsing trap warning (center-top) when `ITrapProximityHudProvider.IsTrapNearby`
- Bottom progress bar: local gold marker, opponent markers, red finish line

Hidden during ceremony phases so Podium/Reward screens own the screen.

## 4. Emotes / Finish / Camera / VFX / Audio

- **Emotes:** fixed 😀😂😎👏💪❤️ bar → `SendRaceEmote`; floating glyph above character briefly.
- **Finish:** `FinishBannerView` — FINISH banner, fireworks (`FireworkSimulation`), confetti (`ConfettiSimulation`), optional crowd/fanfare clips.
- **Camera:** look-ahead on forward motion, gentle Y bob, `TriggerImpactShake` for impacts only.
- **VFX:** fixed ring-buffer dust motes + speed-trail emphasis above configured speed.
- **Audio:** `RaceAudioDirector` for running ambient bed, jump/land, trap-warning one-shots (clips optional / unassigned).

## 5. Accessibility & responsive

- Large bold fonts via `RaceHudTheme` scaled by `HudLayoutScale` (phone→tablet)
- High-contrast gold/sand/desert-night palette (Features-local — never references MainMenu)
- Effect chips carry shape tags, not color alone

## 6. Debug

`RaceHudDebugView` at **`panelX: 4510`** (next free +450 after MainMenu `4060`): FPS, Player Speed, Position, Weapon ID, Trap ID, Network Ping, Race Timer, Shield/SpeedBoost.

## 7. Scene & asset wiring

- **`Gameplay.unity` / `RunnerHUD`:** `RaceHudController`, `RaceHudView`, `CountdownHudView`, `FinishBannerView`, `EmoteBarView`, `RaceAudioDirector`, `RaceVfxPresenter`, `RaceHudDebugView`; legacy `CountdownView` disabled
- **`TrapSystems`:** `TrapProximityWatcher`
- **`MultiplayerSpawning`:** `NetworkDiagnosticsBridge`
- **`RaceStandingsTracker`:** `RaceFinishConfig` for track length
- **`Settings/RaceHudConfig.asset`**, extended **`CameraFollowConfig.asset`**

## 8. Build verification

- **Offline compile:** all **394** project `.cs` files via `dotnet build .compile_check/CompileCheck.csproj` — **0 errors, 0 warnings**
- **YAML:** `Gameplay.unity` **OK** (91 objects); guid uniqueness **OK** (477 unique); skybox/spot-cookie built-in GUID false positives unchanged since Sprint 4

## 9. Remaining TODOs

1. No final HUD art / TMP Canvas / audio clips (OnGUI placeholders; clip fields unassigned).
2. No `Player.prefab` in Gameplay — status-effect HUD / jump-land audio idle until a local player exists.
3. Network ping remains 0 under loopback transport.
4. Live opponent minimap markers only appear once remote progress reports flow (debug can simulate).
5. Concurrent Matchmaking / Locker workstreams remain separate commits.
6. Carries forward unresolved Sprint 1–14 open items.

## 10. Git workflow

| Item | Value |
|---|---|
| Implementation commit | `1adf268` |
| Implementation commit message on git | `Sprint 16 report - verify push confirmation hashes` (mislabeled by a concurrent Sprint 16 docs pass that included the staged Race HUD tree; content is Sprint 15 Race HUD — see `git show 1adf268 --stat`) |
| Report confirmation commit | `(this commit)` |
| Intended feature message | `Sprint 15 - Race HUD, Gameplay UI & In-Race Experience` |
| Branch | `main` |
| Push status | Implementation already on `origin/main` as `1adf268`; report confirmation pushed with this commit |

Sprint 15 is complete within the constraints above.
