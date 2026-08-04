# Sprint 23.13 — Race Scene Integration

**Scope:** Replace Quick Play placeholder (search → LobbyScreen) with a playable offline race prototype. Play Menu / Quick Play screens kept; multiplayer / matchmaking code retained as placeholders for other entry points.

**Status:** Complete.

## New Quick Play flow

```
PlayMenu → QuickPlay card
        → QuickPlay scene
              CreateLocalOfflinePrototype (1 local player stub)
              skip public search / LobbyScreen / Map Voting
        → LoadingScreen (auto-advance 2.5s, clamped 2–3s)
        → Gameplay (Race)
              OfflineRaceBootstrap
                ensure RunnerPlayer + camera + spawn pools
                RaceManager.StartRace()
                RaceProgressService ← runner Z + HUD coins
                MatchState.Running (finish-line systems)
```

## Race scene initialization

| System | Action on Gameplay enter |
|--------|--------------------------|
| `RunnerPlayerController` | Ensured active/enabled (already placed in scene) |
| `RunnerCameraFollow` | Target bound to player `FollowTarget` |
| `SpawnManager` | `WarmPools` — obstacles / coins spawn on segment activate |
| `RaceManager` | `StartRace()` → `RaceState.Running` |
| `RaceFinish` (if present) | Local stub match marked `Running`; progress reports distance |
| Coins | Existing Sprint 23.12 collectible plans (unchanged) |

## Key types

| Piece | Location |
|-------|----------|
| Offline entry flag | `Core.Services.OfflineRaceEntryService` |
| Local 1P stub | `SessionManager.CreateLocalOfflinePrototype` / `MarkOfflineRaceRunning` |
| Quick Play bypass | `Features.QuickPlay.QuickPlayController` |
| Loading auto-advance | `Features.LoadingScreen.LoadingScreenController` |
| Gameplay bootstrap | `Features.Gameplay.OfflineRaceBootstrap` |

LobbyScreen / public matchmaking / Invite Friends paths are **not** removed — only Quick Play skips them.

## Rebuild / patch

- Fallback: `_tools/patch_gameplay_sprint_23_13_offline_race.py`
- Menu: `GulfRun/Play Flow/Validate Gameplay Runner (Sprint 23.4–23.13)`

## Constraints honored

- No UI redesign; Play Menu + Quick Play scenes retained
- No real networking / online matchmaking on this path
- Multiplayer systems remain placeholders
- `DefaultNetworkPrefabs` / `Btn_*` left uncommitted

## Verification

- Gameplay scene YAML parse: OK (`validate_yaml.py`, 254 objects)
- Offline `dotnet build` `.compile_check`: Sprint 23.13 sources clean; remaining shim gaps are pre-existing Camera / PreRaceIntro / MapVoting UI APIs (`.compile_check` is gitignored and lagged behind 23.x play-flow)